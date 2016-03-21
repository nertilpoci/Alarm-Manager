using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Alarm_Manager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Xceed.Wpf.Toolkit;

namespace Alarm_Manager.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CalendarViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the Calendar class.
        /// </summary>
        /// 
        private readonly IDataService _dataService;
        private DateTime _selecteDate ;
        private WeeklySchedule _weeklySchedule;
        private ObservableCollection<Schedule> _schedules; 

        private Schedule _currentDaySchedule;


        public DateTime SelectedDate
        {

            get { return _selecteDate; }
            set
            {
                _selecteDate = value;

                RaisePropertyChanged("SelectedDate");
                CurrentDaySchedule = (new ViewModelLocator()).Main.GetScheduleFromDate(SelectedDate);
            }
        }

        public Schedule CurrentDaySchedule
        {
            get { return _currentDaySchedule; }
            set
            {
                _currentDaySchedule=value;
                RaisePropertyChanged("CurrentDaySchedule");
                
            }
        }

        public ObservableCollection<Schedule> Schedules
        {
            get { return _schedules; }
            set
            {
                _schedules = value;
                RaisePropertyChanged("Schedules");
            }
        } 
        public WeeklySchedule DefaultWeeklySchedule
        {

            get { return _weeklySchedule; }
            set
            {
                _weeklySchedule = value;
                RaisePropertyChanged("DefaultWeeklySchedule");
            }
        }
        public RelayCommand SelectinChangedCommand { get; set; }
        public RelayCommand ShowDefaultScheduleFlyoutCommand { get; set; }
        public RelayCommand HideDefaultScheduleFlyoutCommand { get; set; }
        public RelayCommand DeleteDefaultWeeklyScheduleCommand { get; set; }
        public CalendarViewModel(IDataService dataService)
        {
            _dataService = dataService;
            SelectedDate = DateTime.Today;
          //  DefaultWeeklySchedule=
            ShowDefaultScheduleFlyoutCommand=new RelayCommand(ShowDefaultSchedule);
            HideDefaultScheduleFlyoutCommand=new RelayCommand(HideDefaultSchedule);
            DeleteDefaultWeeklyScheduleCommand=new RelayCommand(DeleteDefaultWeeklySchedule);
            Schedules = _dataService.GetSchedules();
            CurrentDaySchedule =(new ViewModelLocator()).Main.GetScheduleFromDate(SelectedDate);
            DefaultWeeklySchedule = dataService.GetDefaultWeeklySchedule();


        }

     
     

        private void ShowDefaultSchedule()
        {
            (new ViewModelLocator()).Main.DefaultScheduleFlyoutStatus = true;
        }

        private void HideDefaultSchedule()
        {
            var mainview = new ViewModelLocator();
            mainview.Main.SaveAll();
            mainview.Main.DefaultWeeklySchedule = _dataService.GetDefaultWeeklySchedule();
            mainview.Main.DefaultScheduleFlyoutStatus = false;
            
            

        }

        private void DeleteDefaultWeeklySchedule()
        {
            var mainview = new ViewModelLocator();
            _dataService.DeleteWeeklySchedule(_dataService.GetDefaultWeeklySchedule());
            DefaultWeeklySchedule = _dataService.GetDefaultWeeklySchedule();
            mainview.Main.DefaultWeeklySchedule = _dataService.GetDefaultWeeklySchedule();
            mainview.Main.StartScheduler();
            SelectedDate = DateTime.Now;
        }
    }
}