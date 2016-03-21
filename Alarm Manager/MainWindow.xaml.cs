using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using Alarm_Manager.Model;
using Alarm_Manager.ViewModel;
using CustomRenderingSample;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace Alarm_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        private ASDatabaseEntities entities;
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            entities=new ASDatabaseEntities();
           
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           Schedule s= new Schedule();
            s.Name = "schedule1";
            entities.Schedules.Add(s);
            entities.SaveChanges();
            
        }

        private void InterestingPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SingleAlarm_Loaded(object sender, RoutedEventArgs e)
        {
//               Dim ac As Accent = (From a In ThemeManager.DefaultAccents Where a.Name = The_Accent_Color_You_Want).FirstOrDefault
//ThemeManager.ChangeTheme(Application.Current, ac, The_Theme_You_Want)

            
            // Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);
            
                      
            //// now set the Green accent and dark theme
            //ThemeManager.ChangeAppStyle(Application.Current,
            //                            ThemeManager.Accents.First(),
            //                            ThemeManager.AppThemes.First());

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var dictionary = (new ViewModelLocator()).Main.NextPossibleAlarm();
            if (dictionary != null)
            {

                MessageBox.Show(dictionary.First().Key.Time + "---" + dictionary.First().Key.Sound1.Name + " date" + dictionary.First().Value.ToShortDateString());
            }

           
        }

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
           

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Quit",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync("Quit application?",
                "Sure you want to quit application?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                Properties.Settings.Default.IsStartup = (new ViewModelLocator()).Main.MainWindowState ==
                                                   WindowState.Minimized
               ? true
               : false;
                Properties.Settings.Default.Save();
                Application.Current.Shutdown();
            }

           

        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            (new ViewModelLocator()).Main.SettingsFlyoutStatus = false;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
          DataService service=new DataService();
            service.AddWeeklySchedule(new WeeklySchedule());
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void MetroAnimatedSingleRowTabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            SpiningElement.IsEnabled = false;
            SpiningElement.IsEnabled = true;

        }
    }
}