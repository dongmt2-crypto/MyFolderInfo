using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace MyFolderInfo.Model
{
    /// <summary>One <c>&lt;thuoc_tinh&gt; / &lt;gia_tri&gt;</c> pair from Options.xml.</summary>
    internal sealed class ClassOptions
    {
        public string ThuocTinh { get; private set; }

        public string TenTt { get; set; }

        public string GiaTri { get; set; }

        public ClassOptions(string thuocTinh, string tenTt, string giaTri)
        {
            ThuocTinh = thuocTinh ?? string.Empty;
            TenTt = tenTt ?? string.Empty;
            GiaTri = giaTri ?? string.Empty;
        }
    }

    /// <summary>
    /// Reads and writes Options.xml by element *name*.
    /// <para>
    /// The old code addressed the option values positionally (<c>ChildNodes[0]</c> /
    /// <c>ChildNodes[2]</c>), so adding a comment or any whitespace/formatting change in the
    /// hand-edited file caused the value to be written into the wrong element - or to throw.
    /// </para>
    /// </summary>
    internal static class OptionsStore
    {
        public static List<ClassOptions> Load(ErrorLog log = null)
        {
            var options = new List<ClassOptions>();

            if (!XmlStore.TryLoad(AppPaths.OptionsFile, out XmlDocument doc, log))
            {
                return options;
            }

            XmlNodeList rows = doc.DocumentElement?.SelectNodes("dsname");
            if (rows == null)
            {
                return options;
            }

            foreach (XmlNode row in rows)
            {
                string name = ChildText(row, "thuoc_tinh");
                if (name.Length == 0)
                {
                    continue;
                }

                options.Add(new ClassOptions(
                    name,
                    ChildText(row, "ten_tt"),
                    ChildText(row, "gia_tri")));
            }

            return options;
        }

        /// <summary>
        /// Writes the supplied values back. Returns false (without touching the file) if the
        /// document is unreadable, so a malformed Options.xml is never overwritten with blanks.
        /// </summary>
        public static bool Save(IReadOnlyDictionary<string, string> values, ErrorLog log = null)
        {
            if (!XmlStore.TryLoad(AppPaths.OptionsFile, out XmlDocument doc, log))
            {
                return false;
            }

            XmlNodeList rows = doc.DocumentElement?.SelectNodes("dsname");
            if (rows == null)
            {
                return false;
            }

            var written = new HashSet<string>(StringComparer.Ordinal);

            foreach (XmlNode row in rows)
            {
                string name = ChildText(row, "thuoc_tinh");
                if (name.Length == 0 || !values.TryGetValue(name, out string value))
                {
                    continue;
                }

                XmlNode valueNode = row.SelectSingleNode("gia_tri");
                if (valueNode == null)
                {
                    XmlNode created = doc.CreateElement("gia_tri");
                    row.AppendChild(created);
                    valueNode = created;
                }

                valueNode.InnerText = value ?? string.Empty;
                written.Add(name);
            }

            // Append anything the user gained through a future version of the app.
            foreach (KeyValuePair<string, string> pair in values)
            {
                if (written.Contains(pair.Key))
                {
                    continue;
                }

                XmlElement row = doc.CreateElement("dsname");
                AppendChild(doc, row, "thuoc_tinh", pair.Key);
                AppendChild(doc, row, "ten_tt", string.Empty);
                AppendChild(doc, row, "gia_tri", pair.Value);
                doc.DocumentElement.AppendChild(row);
            }

            try
            {
                XmlStore.Save(doc, AppPaths.OptionsFile, log);
                return true;
            }
            catch (Exception ex)
            {
                log?.Write("Could not save Options.xml.", ex);
                return false;
            }
        }

        private static void AppendChild(XmlDocument doc, XmlElement parent, string name, string text)
        {
            XmlElement child = doc.CreateElement(name);
            child.InnerText = text;
            parent.AppendChild(child);
        }

        private static string ChildText(XmlNode row, string elementName)
        {
            XmlNode node = row.SelectSingleNode(elementName);
            return node?.InnerText?.Trim() ?? string.Empty;
        }

        /// <summary>Reads a 0/1 option flag without throwing on a hand-edited value.</summary>
        public static int ReadFlag(string value, int fallback = 0)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed)
                ? parsed
                : fallback;
        }
    }
}
