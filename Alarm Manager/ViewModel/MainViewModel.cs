using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using Alarm_Manager.Model;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using NAudio.Wave;
using NodaTime;
using Microsoft.Win32.TaskScheduler;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Windows.Forms.Timer;
using IWshRuntimeLibrary;
using System.Diagnostics;
using System.Security.Permissions;

namespace Alarm_Manager.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase ,IDataErrorInfo
    {
        //RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
        //               ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private Timer alarmTimer;
        private System.Threading.Timer _alTimer;
        private Timer newDayTimer;
      
        private WaveOut waveOut;
        private MemoryStream soundStream;
        private readonly IDataService _dataService;
        private ObservableCollection<Schedule> schedules ;
        private TimeSpan? _remainingTime;
        private Sound _selectedSound;
        private string _nextalarmifnotalrmsfortoday;
        private bool _startOnStartup;
        private ObservableCollection<string> _deviceCollection;
        private int _selecteddeviceid;
        private int _tabControlSelectedIndex;
        private NotifyIcon notifyIcon;
        private WindowState mainWindowState;
        private bool showInTaskBar;
        private AppTheme currentTheme;
        private Accent currentAccent;
        private bool isSpiningCircleEnable;
        private Period _rTime;
        private Dictionary<Alarm, DateTime> _nextPossibleAlarm;
        private int _asideLeft;
        private int _asideTop;
        private bool _asideVisibility;
 
     

        public Timer EverySecondTimer { get; set; }
        public Timer MidnighTimer { get; set; }

        private Alarm _currentAlarm;
        private bool _newalarmflyoutopen;
        private bool _newscheduleflyoutopen;
        private bool _newexceptionflyoutopen;
        private bool _newsoundflyoutopen;
        private bool _defaultscheduleflyoutopen;
        private bool _settingsFlyoutStatus;
        private WeeklySchedule _defaultWeeklySchedule;
        private bool _isPlaying;
        private bool _isSaved;

        public RelayCommand ReadAllCommand { get; set; }
        public RelayCommand SaveAllCommand { get; set; }
        public RelayCommand AddNewSoundCommand { get; set; }
        public RelayCommand BrowseFileCommand { get; set; }
        public RelayCommand CloseSoundCommand { get; set; }
        public RelayCommand CloseAlarmCommand { get; set; }
        public RelayCommand<Sound> PlayCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand<Sound> DeleteSoundRelayCommand { get; set; }
        public RelayCommand RowEditEndingRelayCommand { get; set; }
        public RelayCommand StartUpChangeRelayCommand { get; set; }
        public RelayCommand ShowSettingsRelayCommand { get; set; }
        public RelayCommand ApplicationExitRelayCommand { get; set; }
        public RelayCommand ShowNewScheduleRelayCommand { get; set; }
        public RelayCommand BackupCommand { get; set; }
        public RelayCommand RestoreCommand { get; set; }
        public MainViewModel(IDataService dataService)
        {

            _dataService = dataService;
            schedules = new ObservableCollection<Schedule>();
            ReadAllCommand = new RelayCommand(GetSchedules);
            SaveAllCommand = new RelayCommand(SaveAll);
            AddNewSoundCommand = new RelayCommand(AddNewSound);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            CloseSoundCommand = new RelayCommand(CloseSoundFlyout);
            CloseAlarmCommand=new RelayCommand(CloseAlarmFlyout);
            PlayCommand = new RelayCommand<Sound>(Play);
            StopCommand = new RelayCommand(Stop);
            ApplicationExitRelayCommand=new RelayCommand(OnAppExit);
            DeleteSoundRelayCommand=new RelayCommand<Sound>(DeleleteSound);
            RowEditEndingRelayCommand=new RelayCommand(row_editEnding);
            ShowNewScheduleRelayCommand=new RelayCommand(ShowNewSchedule);
            RestoreCommand=new RelayCommand(Restore);
            BackupCommand=new RelayCommand(BackUp);
            alarmTimer = new Timer { Enabled = false };
            Microsoft.Win32.SystemEvents.TimeChanged += TimeHandler;
            alarmTimer.Tick += alarmTimer_Tick;
            EverySecondTimer = new Timer();
            EverySecondTimer.Tick += EverySecondTimer_Tick;
            EverySecondTimer.Interval = 1000;
            MidnighTimer = new Timer();
            MidnighTimer.Tick += MidnighTimer_Tick;
            newDayTimer = new Timer();
            waveOut = new WaveOut();
            
            soundStream = new MemoryStream();
            waveOut.PlaybackStopped += new EventHandler<StoppedEventArgs>(waveOut_PlaybackStopped);
          
            DefaultWeeklySchedule = _dataService.GetDefaultWeeklySchedule();
          
            GetSoundDevices();
           
            ShowSettingsRelayCommand=new RelayCommand(ShowSettings);
            notifyIcon=new NotifyIcon {Icon = new Icon("Resources\\n_icon.ico")};
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            ShowInTaskBar = true;
            MainWindowState = WindowState.Normal;
            if (System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Alarm Manager.lnk")) StartOnStartup = true;
            Accents = new ObservableCollection<Accent>(ThemeManager.Accents);
            Themes = new ObservableCollection<AppTheme>(ThemeManager.AppThemes);
            if (Properties.Settings.Default.IsStartup == true)
            {
                MainWindowState = WindowState.Minimized;
            }
            if (Properties.Settings.Default.SoundCard < DeviceCollection.Count)
            {
                SelectedDevice = Properties.Settings.Default.SoundCard;
            }
            else
            {
                Properties.Settings.Default.SoundCard = 0;
                Properties.Settings.Default.Save();
            }
            CurrentAccent = Accents.First(z=>z.Name ==Properties.Settings.Default.Acent);
            
            CurrentTheme = Themes.First(z => z.Name == Properties.Settings.Default.Theme);
            ThemeManager.ChangeAppStyle(Application.Current,
                                       CurrentAccent,
                                      currentTheme);
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            AsideLeft = Convert.ToInt32( desktopWorkingArea.Right - 280);
            AsideTop = 0;


            IsSpinningCircleEnable = true;
            StartScheduler();
        }

        void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            MainWindowState = WindowState.Normal;
        }


        public  async void OnAppExit()
        {
            if (await DialogBox("Confirmation", "Are you sure you want to exit the application, if you do so the scheduled alarms won't work!", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
            {
                System.Windows.Application.Current.Shutdown();
            }

         
        }
        private void TimeHandler(object sender, EventArgs e)
        {
            StartScheduler();
           
        }

        void MidnighTimer_Tick(object sender, EventArgs e)
        {
           
            MidnighTimer.Stop();
            StartScheduler();
            MidnighTimer.Interval = (int)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds;
            MidnighTimer.Start();
            
        }
       
        void EverySecondTimer_Tick(object sender, EventArgs e)
        {
            RemainingTime = null;
            RTime = null;

            if (CurrentAlarm != null)
            {
                RemainingTime = Convert.ToDateTime(CurrentAlarm.Time).TimeOfDay - DateTime.Now.TimeOfDay;


                var endTime = new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, CurrentAlarm.Time.Hour, CurrentAlarm.Time.Minute, CurrentAlarm.Time.Second);
                var startTime = new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);


                RTime = Period.Between(startTime, endTime,
                                               PeriodUnits.Years | PeriodUnits.Months | PeriodUnits.Days | PeriodUnits.Hours | PeriodUnits.Minutes | PeriodUnits.Seconds);
            }
            else
            {

               
               
                    if (NextAlarm != null && NextAlarm.Any())
                    {
                        var endTime = new LocalDateTime(NextAlarm.First().Value.Year, NextAlarm.First().Value.Month,
                            NextAlarm.First().Value.Day, NextAlarm.First().Key.Time.Hour, NextAlarm.First().Key.Time.Minute,
                          NextAlarm.First().Key.Time.Second);
                        var startTime = new LocalDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                            DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);


                        RTime = Period.Between(startTime, endTime,
                            PeriodUnits.Years | PeriodUnits.Months | PeriodUnits.Days | PeriodUnits.Hours |
                            PeriodUnits.Minutes | PeriodUnits.Seconds);
                    }
            }

          
            //if (RemainingTime == null) return;
            //var date = new DateTime(NextPossibleAlarm().First().Value.AddTicks(Convert.ToDateTime(NextPossibleAlarm().First().Key.Time).TimeOfDay.Ticks).Ticks);             
            //TimeSpan = string.Format("{0:### Y;-## ; }  {1:### M;-## ; }  {2:### D;-## ; } {3:### H;-## ; }  {4:### M;-## ; } {5:### S;-## ; }", date.Year,date.Month,date.Day,date.Hour,date.Minute,date.Second);
       
        }

        void ShowSettings()
        {
            SettingsFlyoutStatus = true;
        }

        void waveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            IsPlaying = false;
            
        }

        private void BackUp()
        {
       
                       
              System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = @"Database (*.db)|*.db";
            DialogResult dresult = saveFileDialog1.ShowDialog();
            if (dresult == DialogResult.OK)
            {
               System.IO.File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ASDatabase.db",saveFileDialog1.FileName,true) ;
                }
               
                
                  
        }

        private void Restore()
        {
            var openFileDialog1 = new System.Windows.Forms.OpenFileDialog {Filter = @"Database (*.db)|*.db"};
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                System.IO.File.Copy(openFileDialog1.FileName, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ASDatabase.db", true);


                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        void alarmTimer_Tick(object sender, EventArgs e)
        {   EverySecondTimer.Stop();
            alarmTimer.Stop();
            RemainingTime = null;
            
            Play(CurrentAlarm.Sound1);
           
           
            PreviousAlarmTime = CurrentAlarm.Time;
            CurrentAlarm = null;
            StartScheduler();
        }

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

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                RaisePropertyChanged("IsPlaying");
            }
        }
        public bool ShowInTaskBar
        {
            get { return showInTaskBar; }
            set
            {
                showInTaskBar = value;
                RaisePropertyChanged("ShowInTaskBar");
            }
        }
        public int TabControlSelectedIndex
        {
            get { return _tabControlSelectedIndex; }
            set
            {
                _tabControlSelectedIndex = value;
                IsSpinningCircleEnable = true;
             
                RaisePropertyChanged("TabControlSelectedIndex");
            }
        }
        public DateTime PreviousAlarmTime { get; set; }
        public void DeleleteSound(Sound s)
        {
            if (_dataService.GetSchedules().Any(schedule => schedule.Alarms.Any(alarm => alarm.Sound1 == s)))

            {
                DialogBox("Notice",
                    "This audio file is being used and cannot be delete please make sure to remove all alarms and exceptions referencing this sound",
                    MessageDialogStyle.Affirmative);
            }
            else
            {
              _dataService.DeleteSound(s);
            }
        }
        public void BrowseFile()
        {
            var openfiledialog = new System.Windows.Forms.OpenFileDialog();
            openfiledialog.Filter = "Mp3 (*.mp3)|*.mp3";
            DialogResult result = openfiledialog.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                SelectedPath = openfiledialog.FileName;

            }
        }
        public string Error
        {
            get { return string.Empty; }
        }
        public int AsideLeft
        {

            get { return _asideLeft; }
            set {
                _asideLeft = value;
                RaisePropertyChanged("AsideLeft");
            }
        }
        public int AsideTop
        {

            get { return _asideTop; }
            set
            {
                _asideTop = value;
                RaisePropertyChanged("AsideTop");
            }
        }
        public bool  AsideVisibility
        {

            get { return _asideVisibility; }
            set
            {
                _asideVisibility = value;
                RaisePropertyChanged("AsideVisibility");
            }
        }
        public Dictionary<Alarm, DateTime> NextAlarm
        {
            get { return _nextPossibleAlarm; }
            set
            {
                _nextPossibleAlarm = value;
                RaisePropertyChanged("NextAlarm");
            }
        }
        public string this[string columnName]
        {
            get
            {
                if (columnName == "SelectedPath")
                {
                    if (string.IsNullOrEmpty(this.SelectedPath))
                    {
                        return "You must select a file";
                    }
                    if (!System.IO.File.Exists(this.SelectedPath))
                    {
                        return "File must exists";
                    }
                    if (Path.GetExtension(this.SelectedPath).ToLower() != ".mp3")
                    {
                        return "mp3 file required";
                    }
                }
                return string.Empty;
            }
        }
        public void AddNewSound()
        {
            var fileName = Path.GetFileNameWithoutExtension(SelectedPath);
            if (fileName != null)
            {
                string name = fileName.Normalize();
                if ((new ViewModelLocator()).Schedules.Sounds.Count(z => z.Name == name) == 0)
                {
                    var fs = new FileStream(SelectedPath, FileMode.Open, FileAccess.Read);
                    var reader = new BinaryReader(fs);
                    byte[] BlobValue = reader.ReadBytes((int)fs.Length);
                    fs.Close();
                    reader.Close();

                    _dataService.AddSound( new Sound { Name =name , Data = BlobValue });
                    fs.Dispose();
                    reader.Dispose();
                  
                   NewSoundFlyoutStatus = false;
                          
                
                    
                }
                else
                {
                    MessageBox.Show("File Already Exists");
                }
            }
            else
            {
                MessageBox.Show("File Doesn't  Exists");
            }
        }

        public void CloseAlarmFlyout()
        {
            NewAlarmFlyoutStatus = false;

          
        }
        public void CloseSoundFlyout()
        {
            NewSoundFlyoutStatus = false;

          
        }
        private void GetSchedules()
        {
            Schedules.Clear();
            foreach (var item in _dataService.GetSchedules())
            {
              Schedules.Add(item);  
            }
        }
        

        public WeeklySchedule DefaultWeeklySchedule
        {
            get { return _defaultWeeklySchedule; }
            set
            {
                _defaultWeeklySchedule = value;
                RaisePropertyChanged("DefaultWeeklySchedule");
               
            }
        }
        public bool  NewAlarmFlyoutStatus
        {
            get
            {

                return _newalarmflyoutopen;
            }
            set
            {
                _newalarmflyoutopen = value;
                RaisePropertyChanged("NewAlarmFlyoutStatus");
            }

        }
        public bool NewScheduleFlyoutStatus
        {
            get
            {

                return _newscheduleflyoutopen;
            }
            set
            {
                _newscheduleflyoutopen = value;
                RaisePropertyChanged("NewScheduleFlyoutStatus");
               
            }

        }
        public bool NewExceptionFlyoutStatus
        {
            get
            {

                return _newexceptionflyoutopen;
            }
            set
            {
                _newexceptionflyoutopen = value;
                RaisePropertyChanged("NewExceptionFlyoutStatus");
            }

        }
        public bool NewSoundFlyoutStatus
        {
            get
            {

                return _newsoundflyoutopen;
            }
            set
            {
                _newsoundflyoutopen = value;
                RaisePropertyChanged("NewSoundFlyoutStatus");
            }

        }
        public bool DefaultScheduleFlyoutStatus
        {
            get
            {

                return _defaultscheduleflyoutopen;
            }
            set
            {
                _defaultscheduleflyoutopen = value;
                RaisePropertyChanged("DefaultScheduleFlyoutStatus");
            }

        }

        public bool IsSpinningCircleEnable
        {
            get { return isSpiningCircleEnable; }
            set
            {
                isSpiningCircleEnable = value;
            
                RaisePropertyChanged("IsSpinningCircleEnable");
            }
        }
        public bool IsSave
        {
            get { return _isSaved; }
            set
            {
                _isSaved = value;
                RaisePropertyChanged("IsSave");
            }
        }

        public Accent CurrentAccent
        {
            get { return currentAccent; }
            set
            {
                currentAccent = value; 
                RaisePropertyChanged("CurrentAccent");

                if (CurrentTheme != null)
                    ThemeManager.ChangeAppStyle(Application.Current,
                        currentAccent,
                        CurrentTheme);
                Properties.Settings.Default.Acent = currentAccent.Name;
                Properties.Settings.Default.Save();
              

            }
        }

        public AppTheme CurrentTheme
        {
            get { return currentTheme; }
            set
            {
                currentTheme = value; 
                RaisePropertyChanged("CurrentTheme");
                if (CurrentAccent != null)
                    ThemeManager.ChangeAppStyle(Application.Current,
                        CurrentAccent,
                        currentTheme);
                Properties.Settings.Default.Theme = currentTheme.Name;
                Properties.Settings.Default.Save();
              
            }
        }
        public ObservableCollection<Accent> Accents { get; set; }
        public ObservableCollection<AppTheme> Themes { get; set; }

        public Period RTime
        {
            get { return _rTime; }
            set
            {
                _rTime = value; 
                RaisePropertyChanged("RTime");
            }
        }

       
        public bool SettingsFlyoutStatus
        {
            get { return _settingsFlyoutStatus; }
            set
            {
                _settingsFlyoutStatus = value;
                RaisePropertyChanged("SettingsFlyoutStatus");
            }
        }

        public WindowState MainWindowState
        {
            get { return mainWindowState; }
            set
            {
                mainWindowState = value;
                if (mainWindowState == WindowState.Minimized)
                {

                    ShowInTaskBar = false;
                    notifyIcon.Visible = true;
                    notifyIcon.BalloonTipTitle = @"Alarm Mananger";
                    notifyIcon.BalloonTipText = @"The application has been minimized to the taskbar";
                    notifyIcon.ShowBalloonTip(3000);

                }
                else
                {

                    notifyIcon.Visible = false;
                    ShowInTaskBar = true;

                }
                RaisePropertyChanged("MainWindowState");
            }
        }
        public ObservableCollection<string> DeviceCollection
        {
            get { return _deviceCollection; }
            set
            {
                _deviceCollection = value;
                RaisePropertyChanged("DeviceCollection");
            }
        }
        public Sound SelectedSound
        {
            get { return _selectedSound; }
            set
            {
                _selectedSound = value; 
                RaisePropertyChanged("SelectedSound");
            }
        }

        public int SelectedDevice
        {
            get
            {
               
                    return _selecteddeviceid;
                
            }
            set
            {
                _selecteddeviceid = value;
                Properties.Settings.Default.SoundCard = _selecteddeviceid;
                Properties.Settings.Default.Save();
                RaisePropertyChanged("SelectedDevice");
             
            }
        }
        public Timer AlarmTimer
        {
            get { return alarmTimer; }
            set
            {
                alarmTimer = value;
                RaisePropertyChanged("AlarmTimer");
            }
        }

        public Alarm CurrentAlarm
        {
            get { return _currentAlarm; }
            set
            {
                _currentAlarm = value;
                RaisePropertyChanged("CurrentAlarm");
            }
        }

        public TimeSpan? RemainingTime
        {    
           get{
               return _remainingTime;
           }
            set
            {
                _remainingTime = value;
                RaisePropertyChanged("RemainingTime");
            }
        }

        public bool StartOnStartup
        {
            get { return _startOnStartup; }
            set
            {
                _startOnStartup = value;
                StartupStatusChange();
                RaisePropertyChanged("StartOnStartup");
            }
        }
        public string NextAlarmIfNoAlarmsForToday
        {
            get { return _nextalarmifnotalrmsfortoday; }
            set
            {
                _nextalarmifnotalrmsfortoday = value;
                RaisePropertyChanged("NextAlarmIfNoAlarmsForToday");
            }
        }
        public void SaveAll()
        {   
            _dataService.SaveAll();
             StartScheduler();
            IsSave = false;


          
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
        public Schedule GetScheduleFromDate(DateTime date)
        {

            if (_dataService.GetExceptions().Any(z => z.Date == date.Date))
                    
            {
                return _dataService.GetExceptions().First(z => z.Date == date.Date).Schedule1;
               
            }

         
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return _dataService.GetDefaultWeeklySchedule().Schedule;

                    case DayOfWeek.Tuesday:
                        return _dataService.GetDefaultWeeklySchedule().Schedule1;
                    case DayOfWeek.Wednesday:
                        return _dataService.GetDefaultWeeklySchedule().Schedule2;
                    case DayOfWeek.Thursday:
                        return _dataService.GetDefaultWeeklySchedule().Schedule3;
                    case DayOfWeek.Friday:
                        return _dataService.GetDefaultWeeklySchedule().Schedule4;
                    case DayOfWeek.Saturday:
                        return _dataService.GetDefaultWeeklySchedule().Schedule5;
                    case DayOfWeek.Sunday:
                        return _dataService.GetDefaultWeeklySchedule().Schedule6;
                    default: return null;
                }
            

        }
        public Alarm GetAlarm(Schedule schedule)
        {  
            if(schedule!=null)
        { 
            if (schedule.Alarms.Any(z => Convert.ToDateTime(z.Time).TimeOfDay > DateTime.Now.TimeOfDay))
            {
                return schedule.Alarms.OrderBy(z=>Convert.ToDateTime(z.Time).TimeOfDay).First(z => Convert.ToDateTime(z.Time).TimeOfDay > DateTime.Now.TimeOfDay);   
            }
        }
            
         return null;
            
        }

        public Dictionary<Alarm,DateTime> NextPossibleAlarm()
        {

           
            var daysfromtoday = 1;
            var exceptionIndex = 0;
            while (daysfromtoday <= 7 )
            {
                var schedule = GetScheduleFromDate(DateTime.Today.AddDays(daysfromtoday));
                    if (schedule!=null && schedule.Alarms.Any()) return new Dictionary<Alarm, DateTime>{{ schedule.Alarms.OrderBy(z=>z.Time).First(),DateTime.Today.AddDays(daysfromtoday)}};
                    daysfromtoday=daysfromtoday + 1;
             }
            var exceptions = (new DataService()).GetExceptions().Where(z=>z.Date>DateTime.Now.Date.AddDays(1)).OrderBy(s=>s.Date);

          
            while (exceptionIndex<exceptions.Count())
                {
                    var exception = exceptions.ElementAt(exceptionIndex);
                    if (exception.Schedule1.Alarms.Count > 0)
                    {
                        return new Dictionary<Alarm, DateTime> { {exception.Schedule1.Alarms.OrderBy(z=>z.Time).First(), Convert.ToDateTime(exception.Date) } }; 
                    }
                    exceptionIndex++;
                }   
           
                   
            return null;
        }

        public void StartScheduler()
        {
            RemainingTime = null;
            CurrentAlarm = null;
            NextAlarmIfNoAlarmsForToday = "";
            NextAlarm = null;
            RTime = null;
            var alarm = GetAlarm(GetScheduleFromDate(DateTime.Now));
            if (alarm != null)
            {

                CurrentAlarm = alarm;
                TimeSpan alarmTimespan = Convert.ToDateTime(CurrentAlarm.Time).TimeOfDay - DateTime.Now.TimeOfDay;
                alarmTimer.Interval = Convert.ToInt32(alarmTimespan.TotalMilliseconds);
                var index =
                  CurrentAlarm.Schedule1.Alarms.OrderBy(c => c.Time)
                        .ToList()
                        .IndexOf(CurrentAlarm);
                PreviousAlarmTime = DateTime.Now;
               
                if (index >= 1)
                {
                    PreviousAlarmTime = CurrentAlarm.Schedule1.Alarms.OrderBy(z=>z.Time).ElementAt(index - 1).Time;
                }
                alarmTimer.Start();
                EverySecondTimer.Start();
            }
            else
            {
               NextAlarm = NextPossibleAlarm();

               if (NextAlarm != null && NextAlarm.Any())
                {
                   
                    NextAlarmIfNoAlarmsForToday = "Next Alarm At : " +
                        NextPossibleAlarm()
                            .First()
                            .Value
                            .ToString("ddd, MMM d, yyyy")  +  " "  +
                   NextAlarm.First().Key.Time.ToString("h:mm:ss tt");
                    PreviousAlarmTime = DateTime.Now;
                    EverySecondTimer.Start();
                }
            }
            
            MidnighTimer.Interval = (int)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds;

            MidnighTimer.Start();
            var schedule = DefaultWeeklySchedule;
            DefaultWeeklySchedule = null;
            DefaultWeeklySchedule = schedule;


        }

        public void Play(Sound sound)
        {

          
           waveOut.Stop();

            //BinaryWriter bw;                        // Streams the BLOB to the FileStream object.
            //int bufferSize = 100;                   // Size of the BLOB buffer.
            //byte[] outbyte = new byte[bufferSize];  // The BLOB byte[] buffer to be filled by GetBytes.
            //long retval;                            // The bytes returned from GetBytes.
            //long startIndex = 0;                    // The starting position in the BLOB output.
            //string emp_id = "";                     // The employee id to use in the file name.

            //// Open the connection and read data into the DataReader.




            //bw = new BinaryWriter(fs);
            //bw.Write(entities.Sounds.First().Data);
            try
            {
                soundStream.Dispose();
                soundStream = new MemoryStream(sound.Data);

                var rdr = new Mp3FileReader(soundStream);
                var wavStream = WaveFormatConversionStream.CreatePcmStream(rdr);

                var baStream = new BlockAlignReductionStream(wavStream);

                if (SelectedDevice != DeviceCollection.Count - 1 && SelectedDevice<DeviceCollection.Count) waveOut.DeviceNumber = SelectedDevice;
                    


                // 
                waveOut.Init(baStream);
                waveOut.Play();
                IsPlaying = true;
                if (CurrentAlarm != null)
                {
                    notifyIcon.BalloonTipTitle = @"Alarm Mananger";
                    notifyIcon.BalloonTipText = @"The" + CurrentAlarm.Time + @" is playing. " + CurrentAlarm.Description;
                    notifyIcon.ShowBalloonTip(5000);
                }
            }
            catch (System.Exception exception)
            {

                MessageBox.Show(exception.Message);
            }

        }

        public void Stop()
        {
            waveOut.Stop();
            
        }

        private void ShowNewSchedule()
        {

            NewScheduleFlyoutStatus = true;
        }

        public  async Task<MessageDialogResult> DialogBox(string title,string body,MessageDialogStyle style)
        {

            var mainwindow = (Application.Current.MainWindow as MainWindow);

            mainwindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

            var mySettings = new MetroDialogSettings()
            {
                 AffirmativeButtonText = "Continue",
                 NegativeButtonText = "Cancel",
               
                AnimateHide = true,
                AnimateShow = true,
           

                ColorScheme = MetroDialogColorScheme.Theme
            };
            

            MessageDialogResult result = await mainwindow.ShowMessageAsync(title, body,
                style, mySettings);

            return result;
        }
        public async Task<MessageDialogResult> DialogBox(string title, string body,MetroDialogSettings settings, MessageDialogStyle style)
        {

            var mainwindow = (Application.Current.MainWindow as MainWindow);

            mainwindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;



            MessageDialogResult result = await mainwindow.ShowMessageAsync(title, body,
                style, settings);

            return result;
        }
        private  void row_editEnding()
        {

            IsSave = true;

        }

        void StartupStatusChange()
        {
            string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Alarm Manager.lnk";


            if (StartOnStartup)
            {
                if (System.IO.File.Exists(shortcutAddress))
                {
                    System.IO.File.Delete(shortcutAddress);
                }

                WshShell shell = new WshShell();
                //  string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Alarm Manager.lnk";
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
                shortcut.Description = "A startup shortcut. If you delete this shortcut from your computer, LaunchOnStartup.exe will not launch on Windows Startup"; // set the description of the shortcut
                shortcut.WorkingDirectory = Directory.GetCurrentDirectory();/* working directory */
                shortcut.TargetPath = Directory.GetCurrentDirectory() + "\\Alarm Manager.exe";
                shortcut.Save();

            }
            else
            {
                if (System.IO.File.Exists(shortcutAddress))
                {
                    System.IO.File.Delete(shortcutAddress);
                }
            }
        }
       
        void GetSoundDevices()
        {
            var i = 1;
            DeviceCollection=new ObservableCollection<string>();
         
            for (int deviceId = 0; deviceId < WaveOut.DeviceCount; deviceId++)
            {
                var capabilities = WaveOut.GetCapabilities(deviceId);
               if(DeviceCollection.Contains(capabilities.ProductName)){  DeviceCollection.Add(capabilities.ProductName + i );
                   i++;
               }
               else
               {
                  DeviceCollection.Add(capabilities.ProductName );  
               }
               
            }
            DeviceCollection.Add("Default Sound Card");
        }
    }
}