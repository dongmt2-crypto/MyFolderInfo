using System;
using System.Collections.Generic;
using System.IO;

namespace MyFolderInfo.Model
{
    /// <summary>
    /// Directory enumeration for the folder scanner.
    /// <para>
    /// This replaces a hand-rolled <c>FindFirstFile</c>/<c>FindNextFile</c> P/Invoke layer
    /// (with hand-declared WIN32_FIND_DATA structs) that carried three serious defects:
    /// <list type="bullet">
    /// <item><description>it never set <c>Extenstion</c>, and the whole UI uses
    /// "extension is empty" to mean "this node is a folder" - so with the
    /// "include files" option ticked, every file was treated as a folder;</description></item>
    /// <item><description>it wrote the raw marshalled <c>FILETIME</c> struct's
    /// <c>ToString()</c> into the date fields, i.e. the literal text
    /// "System.Runtime.InteropServices.FILETIME" instead of a date;</description></item>
    /// <item><description>it truncated the file size to <c>nFileSizeLow / 1024</c>,
    /// losing the high word and rounding every sub-kilobyte file down to 0.</description></item>
    /// </list>
    /// It also leaked the find handle when the directory did not exist, and could not
    /// cope with paths past MAX_PATH. The BCL handles all of that.
    /// </para>
    /// </summary>
    internal static class GetMyFiles
    {
        /// <summary>
        /// Files directly inside <paramref name="dirName"/>. Unreadable directories yield an
        /// empty list rather than throwing, so a single denied subfolder cannot abort a scan.
        /// </summary>
        public static List<FileInfo> GetFiles(string dirName, SkipReport skips = null)
        {
            var results = new List<FileInfo>();

            if (!TryCreateDirectory(dirName, out DirectoryInfo dir, out Exception error))
            {
                skips?.Folder(dirName, error);
                return results;
            }

            try
            {
                // EnumerateFiles streams and defers stat calls, so a folder with a million
                // entries no longer materialises a million FileInfo objects at once.
                foreach (FileInfo file in dir.EnumerateFiles())
                {
                    // A server that reports a name the Win32 path parser rejects must not
                    // reach the tree: the walk would throw on it, and the XML writer cannot
                    // even encode it (a NUL is not a legal XML 1.0 character).
                    if (!IsUsablePath(file.FullName))
                    {
                        skips?.Item("file", file.FullName, null);
                        continue;
                    }

                    results.Add(file);
                }
            }
            catch (Exception ex) when (IsRecoverable(ex))
            {
                // Whatever was enumerated before the failure is still returned.
                skips?.Folder(dirName, ex);
            }

            return results;
        }

        /// <summary>Sub-directories of <paramref name="dirName"/>, unreadable ones skipped.</summary>
        public static List<DirectoryInfo> GetDirectories(string dirName, SkipReport skips = null)
        {
            var results = new List<DirectoryInfo>();

            if (!TryCreateDirectory(dirName, out DirectoryInfo dir, out Exception error))
            {
                skips?.Folder(dirName, error);
                return results;
            }

            try
            {
                foreach (DirectoryInfo sub in dir.EnumerateDirectories())
                {
                    if (!IsUsablePath(sub.FullName))
                    {
                        skips?.Item("folder", sub.FullName, null);
                        continue;
                    }

                    results.Add(sub);
                }
            }
            catch (Exception ex) when (IsRecoverable(ex))
            {
                skips?.Folder(dirName, ex);
            }

            return results;
        }

        /// <summary>
        /// Builds the <see cref="DirectoryInfo"/> for a path, or reports why it could not be
        /// built. Every scan entry point went through <c>new DirectoryInfo(path)</c> unguarded,
        /// and a mapped SharePoint/WebDAV drive really does return names that the Win32 path
        /// parser rejects - an embedded NUL is the one seen in the field, which surfaced as
        /// <c>ArgumentException: Null character in path</c> and killed the whole scan over a
        /// single phantom entry.
        /// </summary>
        public static bool TryCreateDirectory(string path, out DirectoryInfo dir, out Exception error)
        {
            dir = null;
            error = null;

            if (!IsUsablePath(path))
            {
                error = new ArgumentException("The path contains characters that Windows does not allow.", nameof(path));
                return false;
            }

            try
            {
                dir = new DirectoryInfo(path);
                return true;
            }
            catch (Exception ex) when (IsRecoverable(ex))
            {
                error = ex;
                return false;
            }
        }

        /// <summary>
        /// True when <paramref name="path"/> is non-empty and free of characters that would
        /// make it unusable later in the pipeline.
        /// </summary>
        /// <remarks>
        /// Deliberately a cheap, allocation-free syntactic test: it runs on every entry of
        /// every folder in the scan, so it must not call <c>Path.GetFullPath</c> per entry.
        /// NUL is the character that actually turns up in the field - some SMB and WebDAV
        /// servers report entry names with an embedded NUL - and it cannot be encoded into
        /// the XML tree file either, so an entry carrying one is dropped at the source
        /// instead of being stored and blowing up on every later access to
        /// <see cref="FileSystemInfo.FullName"/>. The other control characters are rejected
        /// for the same reason: Windows itself never puts one in a name, the Win32 path
        /// parser or the XML writer chokes on them, and a name that survives the scan only
        /// to kill the save of a 100 MB tree is worse than a skipped entry. Everything else
        /// the path parser objects to is caught by the guarded construction in
        /// <see cref="TryCreateDirectory"/>.
        /// </remarks>
        public static bool IsUsablePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            foreach (char c in path)
            {
                // Tab, line feed and carriage return are the only control characters that
                // are legal in XML 1.0, and they cannot occur in a path the OS created.
                if (c < ' ' && c != '\t' && c != '\n' && c != '\r')
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Maps a file onto a <see cref="ClassTree"/> node. A file with no extension still
        /// has to be distinguishable from a folder, so it gets the explicit "noext"
        /// sentinel that the non-P/Invoke code path in <see cref="TreeLib"/> already used.
        /// </summary>
        public static ClassTree ToClassTree(FileInfo file, int nodeCode, int parentCode)
        {
            string extension = file.Extension;
            if (string.IsNullOrEmpty(extension))
            {
                extension = "noext";
            }

            long bytes;
            DateTime created, written, accessed;
            try
            {
                bytes = file.Length;
                created = file.CreationTime;
                written = file.LastWriteTime;
                accessed = file.LastAccessTime;
            }
            catch (Exception ex) when (IsRecoverable(ex))
            {
                // A file deleted between enumeration and stat is not a scan failure.
                bytes = 0;
                created = written = accessed = default;
            }

            return new ClassTree(
                file.Name,
                nodeCode,
                parentCode,
                extension,
                file.FullName,
                FormatSize(bytes),
                created == default ? string.Empty : created.ToString("yyyy-MM-dd"),
                written == default ? string.Empty : written.ToString("yyyy-MM-dd"),
                accessed == default ? string.Empty : accessed.ToString("yyyy-MM-dd"),
                written == default ? string.Empty : written.ToString("yyyyMMdd HH:mm:ss"),
                file.FullName,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);
        }

        /// <summary>
        /// Human-readable size. The app stored sizes in KB as a string and the search dialog
        /// parses them back with <c>Convert.ToInt32</c>, so keep the KB value integral but
        /// never lose the whole size for large files.
        /// </summary>
        public static string FormatSize(long bytes)
        {
            const double Kilobyte = 1024d;
            double kilobytes = bytes / Kilobyte;

            return kilobytes >= long.MaxValue
                ? long.MaxValue.ToString()
                : ((long)Math.Round(kilobytes, MidpointRounding.AwayFromZero)).ToString();
        }

        /// <summary>
        /// True for the failures that mean "this one entry is unusable", as opposed to a bug.
        /// </summary>
        /// <remarks>
        /// Every one of these is routine on a mapped network drive: <c>C:\Windows\WUModels</c>
        /// is denied to a normal user, a SharePoint drive disconnects mid-walk, a folder is
        /// deleted between enumeration and stat. None of them should abort a scan.
        /// <see cref="ArgumentException"/> is here because it is what the path parser raises
        /// for a server-reported name containing a NUL.
        /// </remarks>
        internal static bool IsRecoverable(Exception ex)
        {
            return ex is UnauthorizedAccessException
                or IOException
                or System.Security.SecurityException
                or NotSupportedException
                or ArgumentException
                or System.Xml.XmlException;
        }
    }

    /// <summary>
    /// Records the entries a walk had to skip, so that skipping is visible without drowning
    /// the user.
    /// </summary>
    /// <remarks>
    /// A mapped SharePoint/WebDAV drive routinely reports a handful of protected, phantom or
    /// malformed entries per drive, and the same folder can fail on every pass. Logging a full
    /// exception per occurrence filled the log with stack traces that all said the same thing
    /// and gave the user no way to act on it - so each distinct path is logged once, on a single
    /// line carrying the reason, and repeats are counted rather than written.
    /// </remarks>
    internal sealed class SkipReport
    {
        /// <summary>Distinct paths written to the log before the rest are only counted.</summary>
        private const int MaxDistinctLogged = 200;

        private readonly ErrorLog _log;
        private readonly HashSet<string> _seen = new(StringComparer.OrdinalIgnoreCase);
        private int _count;

        public SkipReport(ErrorLog log) => _log = log;

        /// <summary>Total entries skipped, whether or not they were written to the log.</summary>
        public int Count => _count;

        /// <summary>How many distinct paths were written to the log.</summary>
        public int DistinctLogged => _seen.Count;

        public void Folder(string path, Exception error = null) => Item("folder", path, error);

        public void Item(string kind, string path, Exception error = null)
        {
            _count++;

            if (_log is null)
            {
                return;
            }

            if (!_seen.Add(kind + "|" + (path ?? string.Empty)) || _seen.Count > MaxDistinctLogged)
            {
                return;
            }

            _log.Write(error is null
                ? $"Skipped {kind} '{path}': the path contains characters Windows does not allow."
                : $"Skipped {kind} '{path}': {error.Message.Trim()}");
        }

        /// <summary>Writes the end-of-walk tally. Called once the walk is finished.</summary>
        public void WriteSummary()
        {
            if (_log is null || _count == 0)
            {
                return;
            }

            _log.Write(_count == 1
                ? "1 entry was skipped during the walk."
                : $"{_count} entries were skipped during the walk ({_seen.Count} distinct).");
        }
    }
}
