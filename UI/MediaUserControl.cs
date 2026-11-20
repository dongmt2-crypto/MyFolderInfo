using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>Banner strip that shows the current event message and image.</summary>
    /// <remarks>
    /// Two fixes over the original:
    /// <list type="bullet">
    /// <item><description><c>UsrMedia_Resize</c> called <c>LoadEvent</c>, which re-decoded the
    /// image from disk on every single resize event. <c>Image.FromFile</c> holds a native file
    /// handle for the life of the image and neither the source image, the scaling
    /// <c>Graphics</c> nor the previous <c>PictureBox.Image</c> was ever disposed - so simply
    /// dragging the window wider exhausted the GDI handle quota. The picture is now decoded once
    /// and only the label reflows on resize;</description></item>
    /// <item><description><c>CheckDate</c> compared <c>day + month*30 + year*365</c>, which is
    /// not a date ordering. An event scheduled 1-31 January compared as 31 vs 41, and every
    /// month boundary was skewed. It now parses the dates and compares them properly.</description></item>
    /// </list>
    /// </remarks>
    public partial class MediaUserControl : UserControl
    {
        private DataTable _events;

        /// <summary>Row currently displayed, or -1.</summary>
        public int Index { get; private set; } = -1;

        /// <summary>Suffix for the message column; "2" for every language except Vietnamese.</summary>
        private string MessageSuffix =>
            ClassLanguage.StrLanguage.StartsWith("V", StringComparison.Ordinal) ? string.Empty : "2";

        public MediaUserControl()
        {
            InitializeComponent();
        }

        public void LoadData()
        {
            Index = -1;
            _events = ReadEvents();

            foreach (DataRow row in _events.Rows)
            {
                if (IsCurrent(row))
                {
                    Index = _events.Rows.IndexOf(row);
                    break;
                }
            }

            LoadEvent();
        }

        private static DataTable ReadEvents()
        {
            var table = CreateTable();

            if (!File.Exists(AppPaths.EventsFile))
            {
                return table;
            }

            try
            {
                using var reader = new DataSet();
                reader.ReadXml(AppPaths.EventsFile, XmlReadMode.Auto);

                if (reader.Tables.Count > 0 && reader.Tables[0].Rows.Count > 0)
                {
                    table = reader.Tables[0];
                }
            }
            catch (Exception ex)
            {
                // Was a bare `catch {}`, so a malformed Events.xml silently left the previous
                // banner text on screen with no indication anything was wrong.
                ErrorLog.Current?.Write($"Could not read {AppPaths.EventsFile} for the banner.", ex);
            }

            return table;
        }

        private bool IsCurrent(DataRow row)
        {
            return TryParseDate(row["Ngay1"], out DateTime from)
                && TryParseDate(row["Ngay2"], out DateTime to)
                && DateTime.Today >= from.Date
                && DateTime.Today <= to.Date;
        }

        /// <summary>
        /// The stored format is d/M/yyyy, but hand-edited files use all sorts of things, so
        /// several shapes are accepted and an unparseable date is simply "not current".
        /// </summary>
        private static bool TryParseDate(object value, out DateTime result)
        {
            string raw = value?.ToString()?.Trim();

            if (raw is not null && DateTime.TryParseExact(
                    raw,
                    new[] { "d/M/yyyy", "dd/MM/yyyy", "d/M/yy", "M/d/yyyy", "yyyy-MM-dd" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out result))
            {
                return true;
            }

            return DateTime.TryParse(raw, CultureInfo.GetCultureInfo("vi-VN"), out result)
                || DateTime.TryParse(raw, CultureInfo.InvariantCulture, out result);
        }

        private void UsrMedia_Load(object sender, EventArgs e)
        {
        }

        public void LoadEvent()
        {
            if (_events is null || Index < 0 || Index >= _events.Rows.Count)
            {
                return;
            }

            DataRow row = _events.Rows[Index];

            lblMessage.Text = row["Message" + MessageSuffix].ToString().Trim();

            string imageName = row["image_url"].ToString().Trim();
            if (imageName.Length == 0)
            {
                return;
            }

            string folder = ClassData.ImageDir;
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                folder = AppPaths.ImageFolder;
            }

            string path = Path.Combine(folder, imageName);
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                Image thumbnail = GenerateThumbnail(path, pictureBox1.Size);

                Image previous = pictureBox1.Image;
                pictureBox1.Image = thumbnail;
                previous?.Dispose();
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write($"Could not display the banner image '{path}'.", ex);
            }
        }

        public Image GenerateThumbnail(string filePath, Size thumbnailSize)
        {
            // from-file + clone: FromFile on its own keeps the file locked until the image is
            // disposed, so the banner image could never be replaced while the app was open.
            using var fromFile = Image.FromFile(filePath);
            using var origImage = new Bitmap(fromFile);

            int newWidth;
            int newHeight;

            if (origImage.Width > origImage.Height)
            {
                newWidth = Math.Max(1, thumbnailSize.Width);
                newHeight = Math.Max(1, (int)(origImage.Height * (float)thumbnailSize.Width / origImage.Width));
            }
            else
            {
                newHeight = Math.Max(1, thumbnailSize.Height);
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

        private static DataTable CreateTable()
        {
            var table = new DataTable("event");

            foreach (string name in new[] { "Stt", "Ngay1", "Ngay2", "Message", "Message2", "Image_url" })
            {
                table.Columns.Add(name, typeof(string));
            }

            return table;
        }

        private void LblClose_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void UsrMedia_Resize(object sender, EventArgs e)
        {
            // Deliberately does not reload. The old handler called LoadEvent() here, which
            // re-read the image from disk on every resize tick and leaked a bitmap each time.
            // Only the text needs to react to a width change.
            lblMessage.MaximumSize = new Size(Width - 40, 0);
        }
    }
}
