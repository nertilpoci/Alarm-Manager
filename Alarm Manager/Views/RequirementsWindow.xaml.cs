using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Alarm_Manager
{
    /// <summary>
    /// Interaction logic for RequirementsWindow.xaml
    /// </summary>
    public partial class RequirementsWindow 
    {
        public RequirementsWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var sqliteProcess = new Process();
            sqliteProcess.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\sqlite-netFx451.exe";

            sqliteProcess.Start();
            TerminateProcess();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TerminateProcess();
        }

        void TerminateProcess()
        {
            var currentProcess = Process.GetCurrentProcess();
           

                currentProcess.Kill();
                currentProcess.Dispose();
            }
        }
    }

