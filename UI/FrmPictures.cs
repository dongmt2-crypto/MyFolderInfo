using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>Browses the configured image folder and can use a picture as wallpaper.</summary>
    /// <remarks>
    /// GDI/file-handle fix. The original had four leaks:
    /// <list type="bullet">
    /// <item><description><c>Image.FromFile</c> keeps a native handle on the file open until
    /// the image is disposed, so every picture visited stayed locked on disk and the folder
    /// could never be deleted or overwritten;</description></item>
    /// <item><description>neither the source image nor the <c>Graphics</c> used to scale it was
    /// disposed, so a long browsing session exhausted the GDI handle quota and the whole
    /// process started painting blank rectangles;</description></item>
    /// <item><description><c>viewImage</c> replaced <c>pictureBox1.Image</c> without disposing
    /// the previous bitmap;</description></item>
    /// <item><description><c>RotateImage</c> never disposed its <c>Graphics</c>.</description></item>
    /// </list>
    /// It also reset the shared <c>strImageList</c> never, so reopening the window listed every
    /// image twice, and <c>addSubDirImage</c> overwrote the <c>fil</c> field, so the
    /// "load the first picture" check at the end saw the last subfolder's files rather than
    /// whether anything had been found at all.
    /// </remarks>
    public partial class FrmPictures : ParentForm
    {
        private const int SPI_SETDESKWALLPAPER = 0x14;
        private const int SPIF_UPDATEINIFILE = 0x1;
        private const int SPIF_SENDWININICHANGE = 0x2;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(
            int uAction, int uParam, string lpvParam, int fuWinIni);

        private readonly List<string> _images = new();
        private ClassLanguage _clsLan;

        public FrmPictures()
        {
            InitializeComponent();
        }

        private void FrmPictures_Load(object sender, EventArgs e)
        {
            _clsLan = new ClassLanguage(this, "frmPictures");
            _clsLan.SetControlLanguage(panel1);

            listBox1.ForeColor = lblMemory.ForeColor;
            listBox1.BackColor = GetColor("MauNenTV");

            string folder = ClassData.ImageDir;
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                MessageBox.Show(this, $"Image folder not found:\r\n{folder}", "Images",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            _images.Clear();
            listBox1.Items.Clear();

            CollectImages(folder);

            if (_images.Count == 0)
            {
                MessageBox.Show(this, "No images found in the image folder.", "Images",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ViewImage(0);
        }

        /// <summary>
        /// Depth-first collect. Uses an explicit stack rather than the old recursion, which
        /// recursed once per subfolder and stored intermediate results in fields shared with
        /// its own caller.
        /// </summary>
        private void CollectImages(string root)
        {
            var comparer = new FileInfoComparer2();
            var pending = new Stack<string>();
            pending.Push(root);

            while (pending.Count > 0)
            {
                string current = pending.Pop();

                FileInfo[] files;
                DirectoryInfo[] dirs;

                try
                {
                    files = new DirectoryInfo(current).GetFiles();
                    dirs = new DirectoryInfo(current).GetDirectories();
                }
                catch (Exception ex)
                {
                    // A single unreadable subfolder should not lose the whole listing.
                    ErrorLog.Current?.Write($"Could not list images in '{current}'.", ex);
                    continue;
                }

                Array.Sort(files, comparer);

                foreach (FileInfo file in files)
                {
                    _images.Add(file.FullName);
                    listBox1.Items.Add(file.Name);
                }

                foreach (DirectoryInfo dir in dirs)
                {
                    pending.Push(dir.FullName);
                }
            }
        }

        private void ViewImage(int index)
        {
            if (index < 0 || index >= _images.Count)
            {
                return;
            }

            string path = _images[index];
            lblMemory.Text = path;

            try
            {
                Image thumbnail = GenerateThumbnail(path, pictureBox1.Size);

                // Dispose the outgoing image before installing the new one, or every browse
                // step leaked a bitmap.
                Image previous = pictureBox1.Image;
                pictureBox1.Image = thumbnail;
                previous?.Dispose();
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Could not display '{path}'.", ex);
            }
        }

        public Image GenerateThumbnail(string filePath, Size thumbnailSize)
        {
            // from-file first, then clone: FromFile alone holds the file open for the lifetime
            // of the image, which locked the picture out of any later delete or overwrite.
            using var fromFile = Image.FromFile(filePath);
            using var origImage = new Bitmap(fromFile);

            int newWidth;
            int newHeight;

            if (origImage.Width > origImage.Height)
            {
                newWidth = thumbnailSize.Width;
                newHeight = Math.Max(1, (int)(origImage.Height * (float)thumbnailSize.Width / origImage.Width));
            }
            else
            {
                newHeight = thumbnailSize.Height;
                newWidth = Math.Max(1, (int)(origImage.Width * (float)thumbnailSize.Height / origImage.Height));
            }

            var thumbnail = new Bitmap(newWidth, newHeight, origImage.PixelFormat);

            using (Graphics graphics = Graphics.FromImage(thumbnail))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(origImage, new Rectangle(0, 0, newWidth, newHeight));
            }

            return thumbnail;
        }

        #region Wallpaper

        internal void SetWallpaper(Image image, string style)
        {
            if (image is null)
            {
                return;
            }

            using var key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", writable: true);
            if (key is null)
            {
                return;
            }

            // Win8/Server2012 introduced 6 ("fit"), 7 ("fill") and 10 ("fill"). The old code
            // only ever wrote the pre-Win8 codes, so "stretched" silently letterboxed
            // instead. 10 (fill) is the closest match for "stretched".
            switch (style)
            {
                case "1": // stretched
                    key.SetValue("WallpaperStyle", "10");
                    key.SetValue("TileWallpaper", "0");
                    break;
                case "2": // centred
                    key.SetValue("WallpaperStyle", "6");
                    key.SetValue("TileWallpaper", "0");
                    break;
                case "3": // tiled
                    key.SetValue("WallpaperStyle", "0");
                    key.SetValue("TileWallpaper", "1");
                    break;
            }

            // Wallpaper must be a .bmp. The original passed a Bitmap to Image.Save with an
            // explicit Bmp format, which fails for indexed or 32-bppARGB source bitmaps.
            string location = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "MyWallPaper.bmp");

            try
            {
                using var bmp = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
                }

                bmp.Save(location, System.Drawing.Imaging.ImageFormat.Bmp);

                SystemParametersInfo(
                    SPI_SETDESKWALLPAPER, 0, location, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write("Could not set the desktop wallpaper.", ex);
                MessageBox.Show(this, ex.Message, "Wallpaper",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetWallpaperToolStripMenuItem_Click(object sender, EventArgs e) =>
            SetWallpaper(pictureBox1.Image, "1");

        private void SetWallpaper2ToolStripMenuItem_Click(object sender, EventArgs e) =>
            SetWallpaper(pictureBox1.Image, "2");

        private void SetWallpaper3ToolStripMenuItem_Click(object sender, EventArgs e) =>
            SetWallpaper(pictureBox1.Image, "3");

        #endregion

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewImage(listBox1.SelectedIndex);
        }

        private void CmdOpen_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(ClassData.ImageDir) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Could not open '{ClassData.ImageDir}'.", ex);
            }
        }

        private void CmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RotateMenu_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image is not Bitmap source)
            {
                return;
            }

            Bitmap rotated = RotateImage(source, 90);

            Image previous = pictureBox1.Image;
            pictureBox1.Image = rotated;
            previous?.Dispose();
        }

        private static Bitmap RotateImage(Bitmap source, float angle)
        {
            var rotated = new Bitmap(source.Width, source.Height);

            // Was: Graphics g = Graphics.FromImage(returnBitmap);  never disposed, so every
            // rotation leaked a GDI handle.
            using (Graphics graphics = Graphics.FromImage(rotated))
            {
                graphics.TranslateTransform(source.Width / 2f, source.Height / 2f);
                graphics.RotateTransform(angle);
                graphics.TranslateTransform(-source.Width / 2f, -source.Height / 2f);
                graphics.DrawImage(source, new Point(0, 0));
            }

            return rotated;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = null;
            base.OnFormClosed(e);
        }
    }
}
