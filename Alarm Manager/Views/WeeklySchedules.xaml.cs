using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Alarm_Manager
{
    /// <summary>
    /// Interaction logic for WeeklySchedules.xaml
    /// </summary>
    public partial class WeeklySchedules : UserControl
    {
        public WeeklySchedules()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            
            

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow yourParentWindow = Window.GetWindow(this) as MainWindow;
            //yourParentWindow.settingsFlyout.IsOpen = true;
        }
    }
}
