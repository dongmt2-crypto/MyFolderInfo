using System;
using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;

namespace MyFolderInfo.Model
{
    /// <summary>
    /// Process-wide runtime state. Historically this was a pile of loose statics assigned
    /// in half a dozen places; it is now initialised once and the mutable options are
    /// applied by <see cref="ApplyOptions"/> so there is a single code path.
    /// </summary>
    internal static class ClassData
    {
        public static string XmlOptions { get; private set; } = AppPaths.OptionsFile;

        public static string SortNo { get; set; } = "0";

        public static string Load2level { get; set; } = "1";

        public static string ImageDir { get; set; } = AppPaths.ImageFolder;

        public static Color SelectNodeColor { get; set; } = Color.FromArgb(0x50, 0xFF, 0xFF);

        public static Color NodeColor { get; set; } = Color.Black;

        public static string PleaseWait { get; set; } = string.Empty;

        /// <summary>Number of nodes discovered by the most recent scan; drives the progress display.</summary>
        public static int Count { get; set; }

        /// <summary>
        /// Applies the loaded option list. Previously this lived in FolderInfo.LoadOptions and
        /// silently ignored <c>Image_Dir</c> (it assigned the value to <c>Load2level</c> instead),
        /// which left <see cref="ImageDir"/> null and crashed the picture browser.
        /// </summary>
        public static void ApplyOptions(System.Collections.Generic.IEnumerable<ClassOptions> options)
        {
            foreach (ClassOptions op in options)
            {
                switch (op.ThuocTinh)
                {
                    case "Sort_No":
                        SortNo = op.GiaTri == "0" ? "0" : "1";
                        break;
                    case "Load2level":
                        Load2level = op.GiaTri;
                        break;
                    case "Image_Dir":
                        // Was: ClassData.Load2level = op.GiaTri;  (copy/paste bug)
                        ImageDir = ResolveImageDir(op.GiaTri);
                        break;
                }
            }
        }

        /// <summary>
        /// Options.xml is hand-edited and shipped between machines, so the stored Image_Dir is
        /// often an absolute path from the author's build box (this repo's checked-in value
        /// points at D:\tmp\MyFolderInfo2\...). Falling back to the folder next to the
        /// executable keeps the picture browser and banner working instead of failing on a
        /// directory that only ever existed on another PC.
        /// </summary>
        private static string ResolveImageDir(string configured)
        {
            if (!string.IsNullOrWhiteSpace(configured))
            {
                try
                {
                    if (System.IO.Directory.Exists(configured))
                    {
                        return configured;
                    }
                }
                catch (ArgumentException)
                {
                    // Malformed path characters; fall through to the default.
                }
                catch (System.IO.IOException)
                {
                    // Unreachable path; fall through to the default.
                }
            }

            return AppPaths.ImageFolder;
        }
    }

    /// <summary>Thin wrapper over the optional SQL Server "menu" feature.</summary>
    internal sealed class ClassDataDb
    {
        private readonly ErrorLog _log;

        public ClassDataDb(ErrorLog log = null) => _log = log;

        public static SqlConnectionStringBuilder BuildConnectionString(
            string server, string uid, string pass, string dbname)
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = server,
                UserID = uid,
                Password = pass,
                InitialCatalog = dbname,
                ConnectTimeout = 15,
                // The old raw string used the server's own defaults, which on a modern
                // unpatched box means TLS 1.0 and a long negotiation hang.
                Encrypt = SqlConnectionEncryptOption.Mandatory,
                TrustServerCertificate = true,
                ApplicationIntent = ApplicationIntent.ReadOnly,
            };
        }

        public bool TestConnection(string server, string uid, string pass, string dbname)
        {
            using var connection = new SqlConnection(BuildConnectionString(server, uid, pass, dbname).ConnectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.CommandTimeout = 15;

            try
            {
                connection.Open();
                command.ExecuteScalar();
                return true;
            }
            catch (Exception ex)
            {
                // IsRightConn opened a connection and only closed it on the success path,
                // so every failed login leaked an open SqlConnection.
                _log?.Write($"SQL connection test failed for server '{server}'.", ex);
                return false;
            }
        }

        public DataSet Query(string sql, SqlConnection connection, string tableName)
        {
            if (connection == null)
            {
                throw new InvalidOperationException("No SQL connection was supplied.");
            }

            using var adapter = new SqlDataAdapter(sql, connection);
            adapter.SelectCommand.CommandTimeout = 60;

            var ds = new DataSet();
            adapter.Fill(ds, tableName);
            return ds;
        }
    }
}
