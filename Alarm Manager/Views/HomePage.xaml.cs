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
using System.Windows.Threading;
using Alarm_Manager.Model;
using Alarm_Manager.ViewModel;

namespace Alarm_Manager
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();
            var main = Application.Current.MainWindow as MainWindow;
            main.metroAnimatedSingleRowTabControl.SelectionChanged+=new SelectionChangedEventHandler(Target);
        }
        private static Action EmptyDelegate = delegate() { };
        private void Target(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            //SpiningElement.IsEnabled = false;
            //SpiningElement.IsEnabled = true;
            
        }
        public static void Refresh(UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void HomePage_OnLoaded(object sender, RoutedEventArgs e)
        {
            
            //SpiningElement.IsEnabled = true;
            
        }
    }
}
