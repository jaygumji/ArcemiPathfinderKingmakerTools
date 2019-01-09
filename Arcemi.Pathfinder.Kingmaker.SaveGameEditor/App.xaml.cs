#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            //TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            try {
                var application = new App();
                //application.DispatcherUnhandledException += Application_DispatcherUnhandledException;
                application.InitializeComponent();
                application.Run();
            }
            catch (Exception exception) {
                DisplayAndLogError(exception);
            }
        }

        //private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        //{
        //}

        //private static void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        //{
        //}

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating) {
                DisplayAndLogError(e.ExceptionObject as Exception);
            }
        }

        private static void DisplayAndLogError(Exception e)
        {
            MessageBox.Show($"A critical error occurced.{Environment.NewLine}{e?.ToString()}", "Critical Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            try {
                var fileName = DateTime.UtcNow.Ticks + ".report.log";
                var directory = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = Path.Combine(directory, fileName);
                var dlg = new SaveFileDialog {
                    Filter = "Report log files|*.log",
                    FilterIndex = 1,
                    InitialDirectory = directory,
                    FileName = filePath
                };

                if (dlg.ShowDialog() == true) {
                    filePath = dlg.FileName;
                    File.WriteAllText(filePath, $"Message >> A critical error occurced.{Environment.NewLine}Exception >> {e?.ToString()}", Encoding.UTF8);
                }
            }
            catch (Exception e2) {
                MessageBox.Show($"Unable to save report due to an error. {e2.Message}.", "Critical Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
