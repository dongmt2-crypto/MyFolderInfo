using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace MyFolderInfo.Model
{
    /// <summary>One colour row in Config.xml, including the two restore presets.</summary>
    internal sealed class ThemeEntry
    {
        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = "-16777216";

        public string Default { get; set; } = "-16777216";

        public string Default2 { get; set; } = "-16777216";
    }

    /// <summary>
    /// Reads and writes Config.xml.
    /// </summary>
    /// <remarks>
    /// The original <c>FrmColor</c> held the file in a <see cref="System.Data.DataSet"/> and
    /// called <c>ds.WriteXml</c> on save. Two problems came with that: the file was rewritten
    /// non-atomically, so an interrupted save left unparseable XML that then made
    /// <c>ParentForm</c>'s constructor throw on the next launch, and
    /// <c>Application.StartupPath</c> (a trailing-slash string built with <c>"\Config.xml"</c>)
    /// disagreed with the path the rest of the app used.
    /// </remarks>
    internal static class ThemeStore
    {
        /// <summary>
        /// Keys the colour editor knows about, in the order Config.xml stores them. Every key
        /// <see cref="ParentForm"/> reads has to be listed here: <see cref="Save"/> rewrites the
        /// file from this list, so a key that is missing below is deleted the first time the
        /// user presses Save in the colour dialog.
        /// </summary>
        public static readonly string[] KnownKeys =
        {
            "MauChuBT",
            "MauChuGrid",
            "MauChuLB",
            "MauChuTB",
            "MauChuTV",
            "MauChuLV",
            "MauNenBT",
            "MauNenForm",
            "MauNenGrid",
            "MauNenGroup",
            "MauNenLB",
            "MauNenTabpage",
            "MauNenTB",
            "MauNenTV",
            "MauNodeSelect",
            "MauNenLV",
            "MauNenLV2",
        };

        public static List<ThemeEntry> Load()
        {
            var entries = new List<ThemeEntry>();

            if (XmlStore.TryLoad(AppPaths.ConfigFile, out XmlDocument doc, ErrorLog.Current))
            {
                XmlNodeList rows = doc.DocumentElement?.SelectNodes("cauhinh");
                if (rows is not null)
                {
                    foreach (XmlNode row in rows)
                    {
                        string name = Text(row, "ThuocTinh");
                        if (name.Length == 0)
                        {
                            continue;
                        }

                        entries.Add(new ThemeEntry
                        {
                            Name = name,
                            Value = Text(row, "GiaTri"),
                            Default = Text(row, "GiaTriMacDinh"),
                            Default2 = Text(row, "GiaTriMacDinh2"),
                        });
                    }
                }
            }

            if (entries.Count == 0)
            {
                ErrorLog.Current?.Write(
                    $"{AppPaths.ConfigFile} had no usable colour rows; the colour editor will " +
                    "write a fresh one with default values.");
            }

            return entries;
        }

        /// <summary>
        /// Writes every known key, preserving any stored restore presets. Keys that the file
        /// contains but this class does not manage are written through unchanged, so a colour
        /// hand-added to Config.xml survives a save from the colour dialog.
        /// </summary>
        public static void Save(IEnumerable<ThemeEntry> entries)
        {
            var byName = new Dictionary<string, ThemeEntry>(StringComparer.OrdinalIgnoreCase);
            foreach (ThemeEntry entry in entries)
            {
                byName[entry.Name] = entry;
            }

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            };

            string temp = AppPaths.ConfigFile + ".tmp";

            using (var writer = XmlWriter.Create(temp, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("cauhinh");

                foreach (string key in KeysToWrite(byName))
                {
                    if (!byName.TryGetValue(key, out ThemeEntry entry))
                    {
                        // Keep the key present so the rest of the app never has to guess.
                        entry = new ThemeEntry { Name = key };
                    }

                    writer.WriteStartElement("cauhinh");
                    WriteRow(writer, "ThuocTinh", key);
                    WriteRow(writer, "GiaTri", entry.Value);
                    WriteRow(writer, "GiaTriMacDinh", entry.Default);
                    WriteRow(writer, "GiaTriMacDinh2", entry.Default2);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            if (File.Exists(AppPaths.ConfigFile))
            {
                File.Copy(AppPaths.ConfigFile, AppPaths.ConfigFile + ".bak", overwrite: true);
            }

            File.Move(temp, AppPaths.ConfigFile, overwrite: true);
        }

        /// <summary>
        /// The known keys in declaration order, followed by anything else the current file
        /// holds so unknown rows are not silently dropped.
        /// </summary>
        private static IEnumerable<string> KeysToWrite(Dictionary<string, ThemeEntry> byName)
        {
            var known = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var ordered = new List<string>(KnownKeys.Length);

            foreach (string key in KnownKeys)
            {
                if (known.Add(key))
                {
                    ordered.Add(key);
                }
            }

            // Preserve the file's own ordering for the leftovers.
            foreach (ThemeEntry entry in Load())
            {
                if (known.Add(entry.Name))
                {
                    ordered.Add(entry.Name);
                    byName.TryAdd(entry.Name, entry);
                }
            }

            return ordered;
        }

        private static void WriteRow(XmlWriter writer, string column, string value)
        {
            writer.WriteStartElement(column);
            writer.WriteString(value ?? string.Empty);
            writer.WriteEndElement();
        }

        private static string Text(XmlNode row, string child)
        {
            string raw = row.SelectSingleNode(child)?.InnerText;
            return raw is null ? string.Empty : raw.Trim();
        }
    }
}
