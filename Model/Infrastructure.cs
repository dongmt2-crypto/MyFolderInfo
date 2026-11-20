using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace MyFolderInfo.Model
{
    /// <summary>
    /// Append-only error log. The old code had dozens of empty <c>catch</c> blocks that
    /// silently swallowed every failure; this gives those places somewhere to report to.
    /// Writes are best-effort and never throw.
    /// </summary>
    internal sealed class ErrorLog : IDisposable
    {
        private static readonly object Gate = new();
        private static ErrorLog _current;

        private readonly string _path;
        private bool _disposed;

        private ErrorLog(string path) => _path = path;

        public static ErrorLog Current => _current;

        public string FilePath => _path;

        public static ErrorLog Open()
        {
            if (_current != null)
            {
                return _current;
            }

            string dir;
            try
            {
                dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MyFolderInfo");
                Directory.CreateDirectory(dir);
            }
            catch
            {
                // Fall back to the temp folder; if even that fails we run without a log.
                dir = Path.GetTempPath();
            }

            var log = new ErrorLog(Path.Combine(dir, "error.log"));
            _current = log;
            return log;
        }

        public void Write(string message) => Write(message, null);

        public void Write(string message, Exception exception)
        {
            lock (Gate)
            {
                if (_disposed)
                {
                    return;
                }

                try
                {
                    var sb = new StringBuilder();
                    sb.Append('[')
                      .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                      .Append("] ")
                      .AppendLine(message);

                    if (exception != null)
                    {
                        sb.AppendLine(exception.ToString());
                    }

                    sb.AppendLine(new string('-', 60));

                    File.AppendAllText(_path, sb.ToString(), Encoding.UTF8);
                }
                catch
                {
                    // Logging must never be the reason the app dies.
                }
            }
        }

        public void Dispose()
        {
            lock (Gate)
            {
                _disposed = true;
            }
        }
    }

    /// <summary>Well-known locations of the runtime data files that sit beside the executable.</summary>
    internal static class AppPaths
    {
        public static string BaseDirectory => AppContext.BaseDirectory;

        public static string ConfigFile => Path.Combine(BaseDirectory, "Config.xml");

        public static string OptionsFile => Path.Combine(BaseDirectory, "Options.xml");

        public static string EventsFile => Path.Combine(BaseDirectory, "Events.xml");

        public static string LanguageFolder => Path.Combine(BaseDirectory, "Languages");

        public static string ImageFolder => Path.Combine(BaseDirectory, "Images");

        /// <summary>Resolves a relative name inside a folder, tolerating a missing folder.</summary>
        public static string InFolder(string folder, params string[] parts)
        {
            string path = folder;
            foreach (string part in parts)
            {
                path = Path.Combine(path, part);
            }

            return path;
        }
    }

    /// <summary>
    /// XML helpers that never throw on a missing/hand-edited file. The app keeps its
    /// configuration and event data in hand-edited XML next to the .exe, so every read
    /// has to survive a truncated or absent file, and every write has to be atomic or a
    /// crash mid-save bricks the app on next launch.
    /// </summary>
    internal static class XmlStore
    {
        public static bool TryLoad(string path, out System.Xml.XmlDocument document, ErrorLog log = null)
        {
            document = null;

            if (!File.Exists(path))
            {
                log?.Write($"XML file not found: {path}");
                return false;
            }

            try
            {
                var doc = new System.Xml.XmlDocument { XmlResolver = null };
                doc.Load(path);
                document = doc;
                return true;
            }
            catch (Exception ex)
            {
                log?.Write($"Failed to parse {path}.", ex);
                return false;
            }
        }

        /// <summary>
        /// Saves via a temp file and an atomic replace, so an interrupted write can never
        /// leave a half-written Config.xml / Options.xml / Events.xml behind.
        /// </summary>
        public static void Save(System.Xml.XmlDocument document, string path, ErrorLog log = null)
        {
            string temp = path + ".tmp";
            string backup = path + ".bak";

            try
            {
                document.Save(temp);

                if (File.Exists(path))
                {
                    File.Copy(path, backup, overwrite: true);
                }

                File.Move(temp, path, overwrite: true);
            }
            catch (Exception ex)
            {
                log?.Write($"Failed to save {path}.", ex);

                try
                {
                    if (File.Exists(temp))
                    {
                        File.Delete(temp);
                    }
                }
                catch
                {
                    // Nothing useful left to do.
                }

                throw;
            }
        }
    }
}
