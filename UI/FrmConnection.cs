using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using MyFolderInfo.Model;

namespace MyFolderInfo.UI
{
    /// <summary>Collects SQL Server credentials and verifies the "menu" table exists.</summary>
    public partial class FrmConnection : ParentForm
    {
        private readonly ClassDataDb _db = new(ErrorLog.Current);
        private ClassLanguage _clsLan;

        /// <summary>Open connection handed to <see cref="FrmMenu"/> once login succeeds.</summary>
        public SqlConnection Conn { get; private set; }

        public FrmConnection()
        {
            InitializeComponent();
        }

        private void FrmConnectionLoad(object sender, EventArgs e)
        {
            _clsLan = new ClassLanguage(this, "frmConnection");
        }

        private void CmdCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CmdLoginClick(object sender, EventArgs e)
        {
            string server = txtServer.Text.Trim();
            string user = txtuser.Text.Trim();
            string pass = txtPass.Text;
            string database = txtDatabase.Text.Trim();

            if (server.Length == 0 || database.Length == 0)
            {
                MessageBox.Show(this, _clsLan.GetMessage("008"), "Login",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_db.TestConnection(server, user, pass, database))
            {
                MessageBox.Show(this, _clsLan.GetMessage("008"), "Login",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var connection = new SqlConnection(
                ClassDataDb.BuildConnectionString(server, user, pass, database).ConnectionString);

            try
            {
                connection.Open();

                // The old code checked for the command/exe columns with a hand-rolled string
                // and then read Rows[0]["val"] with no regard for whether the query returned
                // anything at all.
                if (!HasMenuSchema(connection))
                {
                    MessageBox.Show(this, _clsLan.GetMessage("007"), "Login",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    connection.Dispose();
                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Current?.Write("Schema check failed after a successful login.", ex);
                MessageBox.Show(this, ex.Message, "Login",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                connection.Dispose();
                return;
            }

            Conn = connection;
            DialogResult = DialogResult.OK;
            Close();
        }

        private static bool HasMenuSchema(SqlConnection connection)
        {
            const string sql = @"
if exists (
    select 1
    from sys.columns
    where object_id = object_id('command')
      and name in ('exe', 'menu_id0', 'bar', 'bar2')
)
select 1 as val
else
select 0 as val";

            using var command = new SqlCommand(sql, connection) { CommandTimeout = 15 };
            object result = command.ExecuteScalar();
            return result is not null && result.ToString() == "1";
        }
    }
}
