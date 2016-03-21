using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Alarm_Manager.ViewModel;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FirstFloor.ModernUI.Windows.Controls;

namespace Alarm_Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }


        protected override void OnStartup(StartupEventArgs e)      
        {
            //if (e.Args != null && e.Args.Count() > 0)
            //{
            //    //this.Properties["IsStartup"] = e.Args[0];
            //    MessageBox.Show(e.Args[0].ToString());
            //}
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            if(!File.Exists( Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ASDatabase.db") )
            {
                File.Copy(Directory.GetCurrentDirectory()+"\\App_Data\\ASDatabase.db",Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ASDatabase.db",true);
            
            }
          
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            var currentProcess = Process.GetCurrentProcess();
            var runningProcess = (from process in Process.GetProcesses()
                                  where
                                    process.Id != currentProcess.Id &&
                                    process.ProcessName.Equals(
                                      currentProcess.ProcessName,
                                      StringComparison.Ordinal)
                                  select process).FirstOrDefault();


            if (runningProcess != null)
            {

                runningProcess.Kill();
                runningProcess.Dispose();
            }

            SQLiteConnection conn = new SQLiteConnection("Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ASDatabase.db");

            conn.Open();
            using (SQLiteCommand command = conn.CreateCommand())
            {
                command.CommandText = "vacuum;";
                command.ExecuteNonQuery();
               
            }
            conn.Close();
            try
            {
                var entities = new ASDatabaseEntities();
                int i = entities.Alarms.Count();
            }
            catch (System.Exception)
            {

                RequirementsWindow w = new RequirementsWindow();
                w.ShowDialog();
            };
            base.OnStartup(e);

        }

        private void App_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
           e.Handled = true;
        }

        //private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        //{
        //    e.Handled = true;
        //    (new ViewModelLocator()).Main.DialogBox("Error", "Application has run into an error",
        //        MessageDialogStyle.Affirmative);  
        
   
      
        //}

    }
}
