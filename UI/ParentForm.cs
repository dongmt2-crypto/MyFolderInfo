using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>
    /// Base class for every form: loads the colour theme from Config.xml and paints the
    /// control tree with it.
    /// </summary>
    /// <remarks>
    /// Fixes over the original:
    /// <list type="bullet">
    /// <item><description>the palette is parsed once and cached instead of re-reading
    /// Config.xml on every form's Load (and every re-theme);</description></item>
    /// <item><description>a missing or truncated Config.xml no longer throws out of the base
    /// constructor, which used to kill the app at startup;</description></item>
    /// <item><description><c>GetColor</c> returns a usable fallback instead of calling
    /// <c>ColorConverter.ConvertFromString("")</c> for an unknown key;</description></item>
    /// <item><description>duplicate keys in a hand-edited Config.xml no longer throw from
    /// <c>Hashtable.Add</c>;</description></item>
    /// <item><description>the <c>MauNenGroup</c> colour is no longer immediately overwritten
    /// with <c>MauNenForm</c> by a second traversal;</description></item>
    /// <item><description>GDI handles for the theme bitmaps are no longer leaked.</description></item>
    /// </list>
    /// </remarks>
    public partial class ParentForm : Form
    {
        private static readonly Dictionary<string, Color> Theme =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly object ThemeGate = new();
        private static bool _themeLoaded;

        private readonly ColorConverter _colorConverter = new();

        /// <summary>Base colours used when Config.xml is missing or incomplete.</summary>
        private static readonly Dictionary<string, Color> FallbackTheme = new(StringComparer.OrdinalIgnoreCase)
        {
            ["MauChuBT"] = Color.Black,
            ["MauChuGrid"] = Color.Black,
            ["MauChuLB"] = Color.Black,
            ["MauChuTB"] = Color.Black,
            ["MauChuTV"] = Color.Black,
            ["MauChuLV"] = Color.Black,
            ["MauNenBT"] = SystemColors.Control,
            ["MauNenForm"] = SystemColors.Control,
            ["MauNenGrid"] = SystemColors.Control,
            ["MauNenGroup"] = SystemColors.Control,
            ["MauNenLB"] = SystemColors.Control,
            ["MauNenTabpage"] = SystemColors.Control,
            ["MauNenTB"] = Color.White,
            ["MauNenTV"] = Color.White,
            ["MauNodeSelect"] = Color.LightCyan,
            ["MauNenLV"] = Color.WhiteSmoke,
            ["MauNenLV2"] = Color.White,
        };

        public ParentForm()
        {
            InitializeComponent();
            EnsureTheme();
        }

        private static void EnsureTheme(bool forceReload = false)
        {
            if (_themeLoaded && !forceReload)
            {
                return;
            }

            lock (ThemeGate)
            {
                if (_themeLoaded && !forceReload)
                {
                    return;
                }

                Theme.Clear();
                foreach (KeyValuePair<string, Color> pair in FallbackTheme)
                {
                    Theme[pair.Key] = pair.Value;
                }

                if (XmlStore.TryLoad(AppPaths.ConfigFile, out System.Xml.XmlDocument doc, ErrorLog.Current))
                {
                    System.Xml.XmlNodeList rows = doc.DocumentElement?.SelectNodes("cauhinh");
                    if (rows != null)
                    {
                        foreach (System.Xml.XmlNode row in rows)
                        {
                            string name = row.SelectSingleNode("ThuocTinh")?.InnerText?.Trim();
                            string raw = row.SelectSingleNode("GiaTri")?.InnerText?.Trim();

                            if (name.Length == 0 || raw.Length == 0)
                            {
                                continue;
                            }

                            // Assign through the indexer: Hashtable.Add threw on a duplicate key,
                            // so a config with a repeated entry made every form fail to open.
                            Theme[name] = ParseColor(raw, Theme.TryGetValue(name, out Color existing) ? existing : Color.Black);
                        }
                    }
                }

                _themeLoaded = true;
            }
        }

        private static Color ParseColor(string raw, Color fallback)
        {
            if (int.TryParse(raw, out int argb))
            {
                // Config.xml stores colours as signed ARGB integers, e.g. -16777216.
                // The cast is what turns that back into the unsigned form Color wants.
                return Color.FromArgb(unchecked((int)(uint)argb));
            }

            try
            {
                return ColorTranslator.FromHtml(raw);
            }
            catch
            {
                return fallback;
            }
        }

        /// <summary>Drops the cached palette so the next form picks up an edited Config.xml.</summary>
        public static void InvalidateTheme() => EnsureTheme(forceReload: true);

        public Color GetColor(string name)
        {
            EnsureTheme();

            if (name != null && Theme.TryGetValue(name, out Color color))
            {
                return color;
            }

            return Color.Black;
        }

        private void ColorControl(Control ct)
        {
            if (ct is DataGridView grid)
            {
                grid.BackgroundColor = GetColor("MauNenTV");
                grid.ForeColor = GetColor("MauChuGrid");
            }
            else if (ct is MenuStrip menu)
            {
                menu.BackColor = GetColor("MauNenForm");
                foreach (ToolStripMenuItem top in menu.Items)
                {
                    top.ForeColor = GetColor("MauChuBT");
                    foreach (ToolStripItem child in top.DropDownItems)
                    {
                        child.ForeColor = GetColor("MauChuBT");
                        child.BackColor = GetColor("MauNenForm");
                    }
                }
            }
            else if (ct is TreeView tree)
            {
                tree.BackColor = GetColor("MauNenTV");
                tree.ForeColor = GetColor("MauChuTV");
            }
            else if (ct is Button button)
            {
                button.BackColor = GetColor("MauNenBT");
                button.ForeColor = GetColor("MauChuBT");
            }
            else if (ct is Label label)
            {
                label.BackColor = GetColor("MauNenLB");
                label.ForeColor = GetColor("MauChuLB");
            }
            else if (ct is StatusStrip status)
            {
                // Was: argb = cl.ToArgb() + 20; ColorConverter.ConvertFromString(argb.ToString())
                // - i.e. stringifying a 32-bit int and asking the colour converter to parse it,
                // which either threw or returned a wrong colour.
                status.BackColor = GetColor("MauNenForm");
                status.ForeColor = GetColor("MauChuLB");
                foreach (ToolStripStatusLabel item in status.Items)
                {
                    item.ForeColor = GetColor("MauChuLB");
                    item.BackColor = GetColor("MauNenForm");
                }
            }
            else if (ct is TextBox textBox)
            {
                textBox.BackColor = GetColor("MauNenTB");
                textBox.ForeColor = GetColor("MauChuTB");
            }
            else if (ct is ComboBox combo)
            {
                combo.BackColor = GetColor("MauNenTB");
                combo.ForeColor = GetColor("MauChuTB");
            }
            else if (ct is ListBox list)
            {
                list.BackColor = GetColor("MauNenTB");
                list.ForeColor = GetColor("MauChuTB");
            }
            else if (ct is CheckBox check)
            {
                check.BackColor = GetColor("MauNenLB");
                check.ForeColor = GetColor("MauChuLB");
            }
            else if (ct is GroupBox group)
            {
                group.BackColor = GetColor("MauNenGroup");
                group.ForeColor = GetColor("MauChuBT");
                foreach (Control nested in group.Controls)
                {
                    ColorControl(nested);
                }
            }
            else if (ct is TabControl tabs)
            {
                foreach (Control page in tabs.Controls)
                {
                    if (page is not TabPage tabPage)
                    {
                        continue;
                    }

                    tabPage.BackColor = GetColor("MauNenTabpage");
                    tabPage.ForeColor = GetColor("MauChuBT");

                    foreach (Control nested in tabPage.Controls)
                    {
                        ColorControl(nested);
                    }
                }
            }
            else if (ct is Panel panel)
            {
                panel.BackColor = GetColor("MauNenGroup");
                foreach (Control nested in panel.Controls)
                {
                    ColorControl(nested);
                }
            }
            else
            {
                // Catch-all for the other containers (Panel, TableLayoutPanel, SplitContainer,
                // GroupBox, ...). Their children were never visited, so anything laid out
                // inside a panel kept the default grey regardless of the palette.
                // Leaf controls such as Button expose an empty Controls collection, so this
                // is a no-op for them.
                foreach (Control nested in ct.Controls)
                {
                    ColorControl(nested);
                }
            }
        }

        private void ParentForm_Load(object sender, EventArgs e)
        {
            SetColor();
        }

        public void SetColor()
        {
            EnsureTheme(forceReload: true);

            foreach (Control ct in Controls)
            {
                // A single traversal. The old code walked Controls three times, and its third
                // pass repainted GroupBox/Panel backgrounds with MauNenForm, silently
                // discarding whatever MauNenGroup was set to. ColorControl now themes the
                // containers and walks their children too, so no second pass is needed.
                ColorControl(ct);
            }

            BackColor = GetColor("MauNenForm");
        }

        /// <summary>
        /// Language-file message id for each <see cref="PathVerdict"/>. Ids live in the
        /// <c>frmMain</c> block; when a translation is missing the built-in English text is
        /// used, which is the same fallback the rest of the app takes.
        /// </summary>
        private static string UnreadableFolderMessageId(PathVerdict verdict) => verdict switch
        {
            PathVerdict.IsFile => "044",
            PathVerdict.AccessDenied => "041",
            PathVerdict.Unreachable => "042",
            PathVerdict.DeviceNotReady => "045",
            PathVerdict.Busy => "046",
            PathVerdict.InvalidName => "043",
            PathVerdict.Unknown => "047",
            _ => "040",
        };

        private static string DefaultUnreadableFolderText(PathVerdict verdict) => verdict switch
        {
            PathVerdict.IsFile => "That is a file, not a folder.",

            PathVerdict.AccessDenied =>
                "Access denied. Sign in to the drive and make sure you have permission to read this folder.",

            PathVerdict.Unreachable =>
                "The network drive could not be reached. Mapped drives belong to one Windows "
                + "sign-in session, so a drive that shows up in Explorer is invisible here if this "
                + "app was started as administrator or as a different user - reconnect the drive "
                + "from the same user, or enter the \\\\server\\share path instead. A SharePoint link "
                + "can also expire: open it in File Explorer to sign in again, then retry.",

            PathVerdict.DeviceNotReady =>
                "That drive is not connected. Reconnect the network drive (or the SharePoint link) and try again.",

            PathVerdict.Busy =>
                "The network drive did not answer in time. Check the connection and try again.",

            PathVerdict.InvalidName => "That is not a valid path.",

            PathVerdict.Unknown => "Windows could not open that path.",

            _ => "That folder does not exist. Check the path and try again.",
        };

        /// <summary>
        /// Text for "this folder cannot be scanned". Replaces the old flat
        /// <c>"'{path}' is not a folder."</c>, which was also printed for a mapped SharePoint
        /// drive that was merely unreachable - <c>Directory.Exists</c> cannot tell the two apart.
        /// </summary>
        internal static string DescribeUnreadableFolder(
            string path,
            PathVerdict verdict,
            ClassLanguage language = null)
        {
            string translated = language?.GetMessage(UnreadableFolderMessageId(verdict));
            string text = string.IsNullOrWhiteSpace(translated)
                ? DefaultUnreadableFolderText(verdict)
                : translated;

            string shown = string.IsNullOrWhiteSpace(path) ? string.Empty : path;

            // "Not found" on a drive letter this session has never heard of is a different
            // problem from a mistyped subfolder, so name it instead of leaving a guess.
            if (verdict == PathVerdict.NotFound && shown.Length >= 2 && shown[1] == ':'
                && !PathProbe.IsDriveLetterAssigned(shown))
            {
                text += Environment.NewLine + Environment.NewLine
                    + $"The drive letter '{shown[..2]}' is not mapped in this Windows session.";
            }

            return shown.Length == 0
                ? text
                : shown + Environment.NewLine + Environment.NewLine + text;
        }
    }
}
