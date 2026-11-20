using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace MyFolderInfo.Model
{
    /// <summary>
    /// Per-form label lookup. Every string comes from <c>Languages\*.xml</c>, keyed by the
    /// form's tag plus a message id, and controls opt in by setting their <c>Tag</c> to
    /// <c>"L"</c> + message id.
    /// </summary>
    /// <remarks>
    /// Defensive by design: the old version indexed its <see cref="Hashtable"/> directly
    /// (<c>OLanHastable[tag]</c>), so a single menu item missing from a translation file threw
    /// <see cref="KeyNotFoundException"/> and the form could not be opened at all. It also
    /// called <c>doc.Load()</c> on a *directory* path when the Language option was absent.
    /// </remarks>
    internal sealed class ClassLanguage
    {
        private const string TagPrefix = "L";

        private readonly Form _frm;
        private readonly MenuStrip _menuStrip;
        private readonly string _maFrm;
        private readonly Dictionary<string, string> _messages = new(StringComparer.Ordinal);

        public static string StrLanguage { get; private set; } = "English.xml";

        public ClassLanguage(Form frm, string maFrm)
        {
            _frm = frm;
            _maFrm = maFrm;
            LoadLanguage();
        }

        public ClassLanguage(Form frm, string maFrm, MenuStrip menuStrip)
        {
            _frm = frm;
            _maFrm = maFrm;
            _menuStrip = menuStrip;
            LoadLanguage();
            LoadMenuLanguage();
        }

        public string GetMessage(string messId)
        {
            if (string.IsNullOrEmpty(messId))
            {
                return string.Empty;
            }

            return _messages.TryGetValue(messId, out string text) ? text : string.Empty;
        }

        private void LoadLanguage()
        {
            string fileName = ResolveLanguageFile();
            StrLanguage = fileName;

            if (!XmlStore.TryLoad(fileName, out XmlDocument doc, ErrorLog.Current))
            {
                // Missing or broken language file: keep the designer text rather than crash.
                return;
            }

            XmlNodeList rows = doc.DocumentElement?.SelectNodes("dsname");
            if (rows == null)
            {
                return;
            }

            foreach (XmlNode row in rows)
            {
                if (row.ChildNodes.Count < 3)
                {
                    continue;
                }

                if (row.ChildNodes[0].InnerText != _maFrm)
                {
                    continue;
                }

                string id = row.ChildNodes[1].InnerText;
                if (id.Length > 0 && !_messages.ContainsKey(id))
                {
                    // InnerXml (not InnerText) so that "&amp;" in the file becomes "&" properly
                    // instead of relying on the old "?" -> "&" string replace hack.
                    _messages[id] = row.ChildNodes[2].InnerXml;
                }
            }

            SetControlLanguageInForm();
        }

        private static string ResolveLanguageFile()
        {
            string configured = null;

            foreach (ClassOptions op in FolderInfo.ClsOptions)
            {
                if (string.Equals(op.ThuocTinh, "Language", StringComparison.Ordinal))
                {
                    configured = op.GiaTri;
                    break;
                }
            }

            // The old code did: path = StartupPath + "\Languages\"; then, only if the Language
            // option existed, appended the file name - and handed the bare directory to
            // XmlDocument.Load(), which threw.
            if (!string.IsNullOrWhiteSpace(configured))
            {
                string candidate = Path.IsPathRooted(configured)
                    ? configured
                    : Path.Combine(AppPaths.LanguageFolder, configured);

                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            string english = Path.Combine(AppPaths.LanguageFolder, "English.xml");
            if (File.Exists(english))
            {
                return english;
            }

            return null;
        }

        /// <summary>Applies <c>Tag = "Lxxx"</c> labels to a control and all of its children.</summary>
        public void SetControlLanguage(Control ct)
        {
            if (ct == null)
            {
                return;
            }

            foreach (Control child in ct.Controls)
            {
                ApplyTag(child);
            }

            foreach (Control child in ct.Controls)
            {
                SetControlLanguage(child);
            }

            ApplyTag(ct);
        }

        private void SetControlLanguageInForm() => SetControlLanguage(_frm);

        private void ApplyTag(Control control)
        {
            string tag = control.Tag as string;
            if (tag == null || tag.Length < 2 || tag[0] != TagPrefix[0])
            {
                return;
            }

            string message = GetMessage(tag[1..]);
            if (message.Length > 0)
            {
                control.Text = message;
            }
        }

        public void LoadMenuLanguage()
        {
            if (_menuStrip == null)
            {
                return;
            }

            foreach (ToolStripItem item in _menuStrip.Items)
            {
                SetMenuLanguage(item);
            }
        }

        private void SetMenuLanguage(ToolStripItem item)
        {
            if (item is ToolStripMenuItem menuItem)
            {
                string tag = menuItem.Tag as string;
                if (tag != null && tag.Length > 1 && tag[0] == TagPrefix[0])
                {
                    string message = GetMessage(tag[1..]);
                    if (message.Length > 0)
                    {
                        menuItem.Text = message;
                    }
                }
            }

            // Only ToolStripDropDownItem has children; a plain ToolStripItem (separator,
            // text box) does not, so this has to be a pattern match, not a plain call.
            if (item is ToolStripDropDownItem dropDown)
            {
                foreach (ToolStripItem child in dropDown.DropDownItems)
                {
                    SetMenuLanguage(child);
                }
            }
        }

        public void SetContextMenuLanguage(ContextMenuStrip contextMenuStrip)
        {
            if (contextMenuStrip == null)
            {
                return;
            }

            foreach (ToolStripItem item in contextMenuStrip.Items)
            {
                SetContextMenuItem(item);
            }
        }

        private void SetContextMenuItem(ToolStripItem item)
        {
            if (item is ToolStripMenuItem menuItem)
            {
                string tag = menuItem.Tag as string;
                if (tag != null && tag.Length > 1 && tag[0] == TagPrefix[0])
                {
                    string message = GetMessage(tag[1..]);
                    if (message.Length > 0)
                    {
                        menuItem.Text = message;
                    }
                }
            }

            // The old version only ever translated the top level of a context menu.
            if (item is ToolStripDropDownItem dropDown)
            {
                foreach (ToolStripItem child in dropDown.DropDownItems)
                {
                    SetContextMenuItem(child);
                }
            }
        }
    }
}
