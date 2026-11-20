using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MyFolderInfo.Model;

namespace MyFolderInfo
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetDefaultFont(SystemFonts.MessageBoxFont);

            if (Array.IndexOf(args, "--selftest") >= 0)
            {
                Environment.ExitCode = SelfTest.Run();
                return;
            }

            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            using var singleInstance = new Mutex(true, @"Local\MyFolderInfo.SingleInstance", out bool isFirstInstance);
            if (!isFirstInstance)
            {
                MessageBox.Show(
                    "My Folder Info is already running.",
                    "My Folder Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using var log = ErrorLog.Open();
            LogStartupProblems(log);

            Application.Run(new FolderInfo());
        }

        /// <summary>
        /// The app previously died with a raw .NET crash dialog when a runtime data file
        /// next to the executable was missing. Report it once, then keep running degraded.
        /// </summary>
        private static void LogStartupProblems(ErrorLog log)
        {
            foreach (string name in new[] { "Config.xml", "Options.xml" })
            {
                string path = Path.Combine(AppContext.BaseDirectory, name);
                if (!File.Exists(path))
                {
                    log.Write($"Startup: required data file '{name}' is missing from {AppContext.BaseDirectory}.");
                }
            }

            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "Languages")))
            {
                log.Write("Startup: the 'Languages' folder is missing; falling back to built-in English labels.");
            }
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ErrorLog.Current?.Write("UI thread exception.", e.Exception);
            ShowFatalError(e.Exception);
        }

        private static void OnDomainException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                ErrorLog.Current?.Write("Unhandled exception.", ex);
            }

            MessageBox.Show(
                "An unexpected error occurred. Details were written to the log file.",
                "My Folder Info",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void ShowFatalError(Exception ex)
        {
            string logPath = ErrorLog.Current?.FilePath ?? "(log unavailable)";

            MessageBox.Show(
                $"{ex.GetType().Name}: {ex.Message}{Environment.NewLine}{Environment.NewLine}" +
                $"Details: {logPath}",
                "My Folder Info - unexpected error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
