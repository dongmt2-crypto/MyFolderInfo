using System;
using System.IO;

namespace MyFolderInfo.Model
{
    /// <summary>Why a path could or could not be opened as a folder.</summary>
    internal enum PathVerdict
    {
        /// <summary>Readable, and it is a directory.</summary>
        Ok,

        /// <summary>Readable, but it is a file.</summary>
        IsFile,

        /// <summary>Nothing at that path - or the drive letter is not assigned at all.</summary>
        NotFound,

        /// <summary>The server said no (ERROR_ACCESS_DENIED).</summary>
        AccessDenied,

        /// <summary>The share behind the drive could not be reached or authenticated.</summary>
        Unreachable,

        /// <summary>No such drive, or the removable/network device is not connected.</summary>
        DeviceNotReady,

        /// <summary>The share is busy or the connection timed out.</summary>
        Busy,

        /// <summary>Windows rejected the path itself (bad characters, reserved name).</summary>
        InvalidName,

        /// <summary>Something else - the Win32 code is reported as-is.</summary>
        Unknown,
    }

    /// <summary>
    /// Checks that a path really is a readable folder, and says why when it is not.
    /// </summary>
    /// <remarks>
    /// This exists because of the mapped-network-drive case. Both scan entry points gated on
    /// <c>Directory.Exists(path)</c>, and that API <b>never</b> throws: it catches every
    /// failure and returns <c>false</c>. A drive that is mapped but momentarily unreachable -
    /// the normal state for a SharePoint/WebDAV mapping that has dropped, that was mapped in a
    /// different Windows logon session (mapped drives do not exist for an elevated or
    /// different-user process), or whose SharePoint cookie has expired - therefore produced
    /// the flat statement <c>"'G:\Docs' is not a folder."</c>, which is both false and gives
    /// the user nothing to act on.
    /// <para>
    /// <see cref="File.GetAttributes"/> does the opposite: it surfaces the underlying Win32
    /// error code, which <see cref="Classify"/> maps onto <see cref="PathVerdict"/> so the
    /// caller can report "the drive is not connected" instead of "that is not a folder".
    /// </para>
    /// </remarks>
    internal static class PathProbe
    {
        private const int ErrorFileNotFound = 2;
        private const int ErrorPathNotFound = 3;
        private const int ErrorAccessDenied = 5;
        private const int ErrorInvalidDrive = 15;
        private const int ErrorNotReady = 21;
        private const int ErrorBadNetPath = 53;
        private const int ErrorNetworkBusy = 54;
        private const int ErrorNetBusy2 = 55;
        private const int ErrorBadNetName = 67;
        private const int ErrorNetworkNameDeleted = 64;
        private const int ErrorInvalidName = 123;
        private const int ErrorLogonFailure = 1326;
        private const int ErrorNoSuchDevice = 433;
        private const int ErrorNoConnections = 1203;

        /// <summary>
        /// Cleans up what the folder picker (or a hand-typed path) hands back, so a path that
        /// Explorer opens is a path this app can also open:
        /// <list type="bullet">
        /// <item><description>surrounding whitespace and quotes - a name typed into the dialog's
        /// file-name box keeps its quotes, which <c>Directory.Exists</c> rejects;</description></item>
        /// <item><description>the shell namespace form <c>::{GUID}\G:\Docs</c> the shell
        /// sometimes returns for "This PC" entries;</description></item>
        /// <item><description>the extended-length prefix <c>\\?\</c>, which would otherwise
        /// make a long SharePoint path look like it does not exist.</description></item>
        /// </list>
        /// Returns an empty string for null/blank input; never returns null.
        /// </summary>
        public static string Normalize(string raw)
        {
            if (raw == null)
            {
                return string.Empty;
            }

            string path = raw.Trim().Trim('"').Trim();
            if (path.Length == 0)
            {
                return string.Empty;
            }

            // "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}\G:\Docs" -> "G:\Docs"
            if (path.StartsWith("::", StringComparison.Ordinal))
            {
                int close = path.IndexOf('}');
                if (close >= 0 && close + 1 < path.Length)
                {
                    string tail = path[(close + 1)..].TrimStart('\\');
                    if (tail.Length > 0)
                    {
                        path = tail;
                    }
                }
            }

            // \\?\UNC\server\share -> \\server\share,  \\?\G:\Docs -> G:\Docs
            if (path.StartsWith(@"\\?\UNC\", StringComparison.OrdinalIgnoreCase))
            {
                path = @"\\" + path[8..];
            }
            else if (path.StartsWith(@"\\?\", StringComparison.Ordinal))
            {
                path = path[4..];
            }

            return path;
        }

        /// <summary>
        /// Probes <paramref name="rawPath"/> and reports what it is. On failure
        /// <paramref name="win32Code"/> carries the underlying OS error (0 when the path was
        /// simply blank or malformed).
        /// </summary>
        public static PathVerdict Probe(string rawPath, out int win32Code)
        {
            win32Code = 0;

            string path = Normalize(rawPath);
            if (path.Length == 0)
            {
                return PathVerdict.NotFound;
            }

            try
            {
                FileAttributes attributes = File.GetAttributes(path);
                return (attributes & FileAttributes.Directory) != 0
                    ? PathVerdict.Ok
                    : PathVerdict.IsFile;
            }
            catch (Exception ex) when (ex is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or ArgumentException
                or NotSupportedException)
            {
                win32Code = ex.HResult & 0xFFFF;
                return Classify(win32Code);
            }
        }

        /// <summary>
        /// True when <paramref name="rawPath"/> is a readable directory. Convenience for the
        /// callers that only need the yes/no; use <see cref="Probe"/> when the reason matters.
        /// </summary>
        public static bool IsReadableFolder(string rawPath) =>
            Probe(rawPath, out _) == PathVerdict.Ok;

        private static PathVerdict Classify(int win32Code) => win32Code switch
        {
            ErrorAccessDenied => PathVerdict.AccessDenied,

            ErrorFileNotFound or ErrorPathNotFound or ErrorInvalidDrive => PathVerdict.NotFound,

            ErrorBadNetPath or ErrorBadNetName or ErrorLogonFailure
                or ErrorNetworkNameDeleted or ErrorNoConnections => PathVerdict.Unreachable,

            ErrorNotReady or ErrorNoSuchDevice => PathVerdict.DeviceNotReady,

            ErrorNetworkBusy or ErrorNetBusy2 => PathVerdict.Busy,

            ErrorInvalidName => PathVerdict.InvalidName,

            _ => PathVerdict.Unknown,
        };

        /// <summary>
        /// True when the drive letter in <paramref name="rawPath"/> exists in <i>this</i>
        /// session. A drive letter that is absent here but present in Explorer's "This PC" is
        /// the signature of a mapping made under a different Windows account, or of an
        /// elevated process, and is worth saying out loud.
        /// </summary>
        public static bool IsDriveLetterAssigned(string rawPath)
        {
            string path = Normalize(rawPath);
            if (path.Length < 2 || path[1] != ':' || !char.IsLetter(path[0]))
            {
                return true;   // not a drive-letter path, so the question does not apply
            }

            string letter = path[..2];
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.Name.StartsWith(letter, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Label for a folder node. <see cref="DirectoryInfo.Name"/> is an empty string for a
        /// volume or share root - <c>"G:\"</c>, <c>"\\server\share\"</c> - so scanning a mapped
        /// network drive root produced a blank tree root that looked like a rendering bug.
        /// </summary>
        public static string DisplayName(DirectoryInfo dir)
        {
            if (dir == null)
            {
                return string.Empty;
            }

            return dir.Name.Length > 0 ? dir.Name : dir.FullName ?? string.Empty;
        }
    }
}