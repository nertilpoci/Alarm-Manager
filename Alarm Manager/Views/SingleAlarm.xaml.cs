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
using Alarm_Manager.ViewModel;

namespace Alarm_Manager
{
    /// <summary>
    /// Interaction logic for SingleAlarm.xaml
    /// </summary>
    public partial class SingleAlarm 
    {
        public SingleAlarm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm1 = (new ViewModelLocator()).Main;
            vm1.NewAlarmFlyoutStatus = false;
            //(new ViewModelLocator()).Schedules.SelectedSound = (new AlarmManagerEntities()).Sounds.First();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (TimeControl.Value != null) TimeControl.Value.Value.ToShortTimeString();
        }
    }
}
