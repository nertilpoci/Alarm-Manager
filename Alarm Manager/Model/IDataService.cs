using System.Collections.ObjectModel;

namespace Alarm_Manager.Model
{
    public interface IDataService
    {
        ObservableCollection<Schedule> GetSchedules();
        void AddSchedule(Schedule s);
        ObservableCollection<Alarm> GetAlarms();
        int AddAlarm(Alarm a);
        ObservableCollection<WeeklySchedule> GetWeeklySchedules();
        WeeklySchedule GetDefaultWeeklySchedule();
        int AddWeeklySchedule(WeeklySchedule s);
        ObservableCollection<Exception> GetExceptions();
        int AddException(Exception e);
        void DeleteException(Exception e);
        void DeleteAlarm(Alarm alarm);
        void DeleteSchedule(Schedule schedule);
        void DeleteWeeklySchedule(WeeklySchedule schedule);
        ObservableCollection<Sound> GetSounds();
        void AddSound(Sound sound);
        void ClearSoundDependencies(Sound s);
        void ClearScheduleDependencies(Schedule s);

        void DeleteSound(Sound sound);
       

        int SaveAll();

    }

}

           