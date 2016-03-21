using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using Alarm_Manager.Model;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;


namespace Alarm_Manager.ViewModel
{
   
    public class SchedulesViewModel : ViewModelBase, IDataErrorInfo
    {
        /// <summary>
        /// Initializes a new instance of the SchedulesViewModel class.
        /// </summary>
      
        public readonly IDataService _dataService;
        private ObservableCollection<Schedule> schedules ;
        private ObservableCollection<Sound> sounds;
        private string _teststring;
        private DateTime _multipleAlamrsStartTime;
        private DateTime _multipleAlamrsEndTime;
        private int _interval;
        private DateTime _singleAlarmTime;
        private String _singleAlarmDescriptions;
        private Schedule _singleAlarmSchedule;
        private Alarm _selectedAlarm;


        
        public RelayCommand ReadAllCommand { get; set; }
        public RelayCommand AddNewScheduleCommand { get; set; }
        public RelayCommand AddNewAlarmCommand { get; set; }
        public RelayCommand ShowNewAlarmCommand { get; set; }
        public RelayCommand DeleteScheduleCommand { get; set; }
        public RelayCommand DeleteAlarmCommand { get; set; }
        public RelayCommand AddNewSoundCommand { get; set; }
        public RelayCommand AddMultipleAlarmsCommand { get; set; }
        public RelayCommand AlarmGridRowUpdateRelayCommand { get; set; }


     
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        private Schedule _selecteditem;

        private Sound _selectedSound;

        public Schedule SelectedItem
        {
            get { return _selecteditem; }
            set
            {
                _selecteditem = value;
                RaisePropertyChanged("SelectedItem");
                if (_selecteditem != null)
                {
                    var view = (CollectionView)CollectionViewSource.GetDefaultView(_selecteditem.Alarms);
                    view.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Ascending));
                }
            }
        }

        public DateTime SingleAlarmTime
        {
            get
            {
                return _singleAlarmTime;
            }
            set
            {
                _singleAlarmTime = value;
                RaisePropertyChanged("SingleAlarmTime");
            }
        }

        public string SingleAlarmDescription
        {
            get
            {
                return _singleAlarmDescriptions;
            }
            set
            {
                _singleAlarmDescriptions = value;
                RaisePropertyChanged("SingleAlarmDescription");

            }
        }

        public Sound SelectedSound
        {
            get { return _selectedSound; }
            set {
                _selectedSound = value;
                RaisePropertyChanged("SelectedSound");
            }
        }

        public int SoundComboSelectedIndex { get; set; }

        public Alarm SelectedAlarm
        {
            get { return _selectedAlarm; }
            set
            {
                _selectedAlarm = value;
                RaisePropertyChanged("SelectedAlarm");
            }
        }

        private string _newschedulename="" ;
        public string NewScheduleName {
            get { return _newschedulename; }


            set
            {
                _newschedulename = value;
                RaisePropertyChanged("NewScheduleName");
                
            }
        }
        public DateTime MultipleAlarmsStartTime
        {
            get { return _multipleAlamrsStartTime; }


            set
            {
                _multipleAlamrsStartTime = value;
                RaisePropertyChanged("MultipleAlarmsStartTime");

            }
        }
        public DateTime MultipleAlarmsEndTime
        {
            get { return  _multipleAlamrsEndTime; }


            set
            {
                _multipleAlamrsEndTime = value;
                RaisePropertyChanged("MultipleAlarmsEndTime");

            }
        }
        public int Interval
        {
            get { return _interval; }


            set
            {
                _interval = value;
                RaisePropertyChanged("Interval");

            }
        }

        void AddNewSound()
        {

           
            (new ViewModelLocator()).Main.NewSoundFlyoutStatus = true;
        }


        public string Teststring
        {
            get { return _teststring; }
            set
            {
                _teststring = value;
                RaisePropertyChanged("Teststring");
            }
        }
      
        
        public SchedulesViewModel(IDataService dataService)
        {
            _dataService = dataService;
            //schedules = new ObservableCollection<Schedule>(_dataService.GetSchedules());
            schedules = _dataService.GetSchedules();
            sounds = _dataService.GetSounds();
            try
            {
               SelectedItem = schedules.First();                                                                                         
            }
            catch (System.Exception)
            {
                
              
            }
           
            
            ReadAllCommand = new RelayCommand(GetSchedules);
            AddNewScheduleCommand = new RelayCommand(AddNewSchedule);
            AddNewAlarmCommand = new RelayCommand(AddNewAlarm);
            ShowNewAlarmCommand=new RelayCommand(ShowNewAlarm);
            SoundComboSelectedIndex = -1;
           
            DeleteAlarmCommand=new RelayCommand(DeleteAlarm);
            DeleteScheduleCommand=new RelayCommand(DeleteSchedule);
            AddNewSoundCommand=new RelayCommand(AddNewSound);
            AddMultipleAlarmsCommand=new RelayCommand(AddMultipleAlarms);
            SingleAlarmTime = DateTime.Now;
            AlarmGridRowUpdateRelayCommand=new RelayCommand(UpdateAlarmGridRow);
        }


        
        private async  void  DeleteSchedule()
         {
             int  index = Schedules.IndexOf(SelectedItem);
            if (SelectedItem.Exceptions.Count > 0)
            {
                if (await (new ViewModelLocator()).Main.DialogBox("In Use Exception",
                    "The Schedule Is Being Used In The Default Weekly Schedule Or Exceptions. If you choose to continue exceptions contaning this schedule will be deleted too",
                    MessageDialogStyle.AffirmativeAndNegative) ==
                    MessageDialogResult.Affirmative)
                {

                    _dataService.DeleteSchedule(SelectedItem);
                }

                if (index > 0)
                {
                    SelectedItem = Schedules.ElementAt(index - 1);
                }
                else
                {
                    if (Schedules.Count > 0)
                    {
                        SelectedItem = Schedules.FirstOrDefault();
                    }
                }
            }
            else
            {
                _dataService.DeleteSchedule(SelectedItem);
                if (index > 0)
                {
                    SelectedItem = Schedules.ElementAt(index - 1);
                }
                else
                {
                    if (Schedules.Count > 0)
                    {
                        SelectedItem = Schedules.FirstOrDefault();
                    }
                }
            }


            (new ViewModelLocator()).Main.StartScheduler();
            

        }
        private void DeleteAlarm()
        {
           
            _dataService.DeleteAlarm(SelectedAlarm);
             (new ViewModelLocator()).Main.StartScheduler();
           
             
        }

        private void GetSchedules()
        {
          //Schedules.Clear();
          //  foreach (var item in _dataService.GetSchedules())
          //  {
          //    Schedules.Add(item);  
          //  }
            (new ViewModelLocator()).Main.NewScheduleFlyoutStatus = true;
            MessageBox.Show("called");
          
        }

        private void ShowNewAlarm()
        {

            (new ViewModelLocator()).Main.NewAlarmFlyoutStatus = true;
        }
        private async void AddNewSchedule()
        {
            var s = new Schedule {Name = NewScheduleName};
            var ex=new System.Exception();
            
            bool error = false;
            try
            {
               
             schedules.Add(s);
                NewScheduleName = "";
                _dataService.SaveAll();
            }
            catch ( SystemException e)
            {
                error = true;
                ex = e;
            }
            if (error)
            {
          
                var mainwindow = (Application.Current.MainWindow as MainWindow);
               

              
                await  mainwindow.ShowMessageAsync("Sorry!", "A problem has accured trying to add the new schedule. Problem:"+ ex.Message);
                       

                
                
            }

            (new ViewModelLocator()).Main.NewScheduleFlyoutStatus = false;
            SelectedItem = s;

        }

        private async void AddNewAlarm()

        {
            var time = SingleAlarmTime.AddSeconds(- SingleAlarmTime.Second);
            var alarm = new Alarm
            {

                Sound1 = SelectedSound ,
                Time = new DateTime(time.TimeOfDay.Ticks),
                
                Description = SingleAlarmDescription
            };

          
                SelectedItem.Alarms.Add(alarm);
                _dataService.SaveAll();


            SingleAlarmTime = SingleAlarmTime.AddHours(1);
                SingleAlarmDescription = "";
           
            (new ViewModelLocator()).Main.NewAlarmFlyoutStatus = false;
            (new ViewModelLocator()).Main.StartScheduler();
        }
      

        void UpdateAlarmGridRow()
        {
            SelectedAlarm = null;
            (new ViewModelLocator()).Main.SaveAll();
            MessageBox.Show("update alarm grid row");
        }
        private async void AddMultipleAlarms()

        {

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No" ,
                FirstAuxiliaryButtonText = "Cancel",
                

                AnimateHide = true,
                AnimateShow = true,


                ColorScheme = MetroDialogColorScheme.Theme
            };
            var result =(new ViewModelLocator()).Main.DialogBox("Duplication","Replace if there are alarms at the same time exist",mySettings,MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary);
            if (await result!=MessageDialogResult.FirstAuxiliary)
            {
                if (await result == MessageDialogResult.Affirmative)
                {
                    while (MultipleAlarmsStartTime.TimeOfDay <= MultipleAlarmsEndTime.TimeOfDay)
                    {
                          var alarm = new Alarm();
                          alarm.Time = new DateTime(MultipleAlarmsStartTime.Ticks);
                    alarm.Sound1 = SelectedSound;
                    if (SelectedItem.Alarms.Any(z => Convert.ToDateTime(z.Time) == Convert.ToDateTime(alarm.Time)))
                    {
                        var alarmtodelete =
                            SelectedItem.Alarms.First(z => Convert.ToDateTime(z.Time) == Convert.ToDateTime(alarm.Time));
                        _dataService.DeleteAlarm(alarmtodelete);
                        
                           
                            
                    }
                   SelectedItem.Alarms.Add(alarm);
                        MultipleAlarmsStartTime = MultipleAlarmsStartTime.AddMinutes(Interval);
                    }
                }
                else
                {
                    while (MultipleAlarmsStartTime.TimeOfDay <= MultipleAlarmsEndTime.TimeOfDay)
                    {
                        var alarm = new Alarm();
                        alarm.Time = new DateTime(MultipleAlarmsStartTime.Ticks);
                        alarm.Sound1 = SelectedSound;
                        if (!SelectedItem.Alarms.Any(z => Convert.ToDateTime(z.Time) == Convert.ToDateTime(alarm.Time)))
                        {
                            SelectedItem.Alarms.Add(alarm);
                        }
                        MultipleAlarmsStartTime = MultipleAlarmsStartTime.AddMinutes(Interval);

                    }
                }
            }
           
           
            _dataService.SaveAll();
        


            (new ViewModelLocator()).Main.NewAlarmFlyoutStatus = false;
            (new ViewModelLocator()).Main.StartScheduler();
        }
        
        public ObservableCollection<Schedule> Schedules
        {
            get
            {

                return schedules;
            }
            set
            {
                schedules = value;
                RaisePropertyChanged("Schedules");
            }

        }
        public ObservableCollection<Sound> Sounds
        {
            get
            {

                return sounds;
            }
            set
            {
                sounds = value;
                RaisePropertyChanged("Sounds");
            }

        } 
        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
      
        private string selectedPath = string.Empty;

        public string SelectedPath
        {
            get { return selectedPath; }
            set
            {
                selectedPath = value;
                RaisePropertyChanged("SelectedPath");
            }
        }
        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "SingleAlarmTime")
                {
                   if (SelectedItem != null && SelectedItem.Alarms.Any(z=>z.Time.TimeOfDay==SingleAlarmTime.TimeOfDay))
                    {

                        return "Alarm At This Time Already Exists";
                    }

                }
                return string.Empty;
            }
        }
    }
}