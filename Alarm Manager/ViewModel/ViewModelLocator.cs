/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:Alarm_Manager.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Alarm_Manager.Model;

namespace Alarm_Manager.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SchedulesViewModel>();
            SimpleIoc.Default.Register<ExceptionsViewModel>();
            SimpleIoc.Default.Register<CalendarViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public MainViewModel Main
        {
         
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public SchedulesViewModel Schedules
        {

            get
            {
                return ServiceLocator.Current.GetInstance<SchedulesViewModel>();
            }
        }

        public ExceptionsViewModel Exceptions
        {

            get
            {
                return ServiceLocator.Current.GetInstance<ExceptionsViewModel>();
            }
        }

        public CalendarViewModel Calendar
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CalendarViewModel>();
            }
        }
        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}