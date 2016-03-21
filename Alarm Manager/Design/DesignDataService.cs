using System;
using System.Collections.ObjectModel;
using Alarm_Manager.Model;

namespace Alarm_Manager.Design
{
    public class DesignDataService :IDataService
    {
        public ObservableCollection<Schedule> GetSchedules()
        {
            return new ObservableCollection<Schedule>{new Schedule{Name = "Schedule"}};
        }

        public void AddSchedule(Schedule s)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<Alarm> GetAlarms()
        {
            throw new NotImplementedException();
        }

        public int AddAlarm(Alarm a)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<WeeklySchedule> GetWeeklySchedules()
        {
            throw new NotImplementedException();
        }

        public WeeklySchedule GetDefaultWeeklySchedule()
        {
            throw new NotImplementedException();
        }

        public int AddWeeklySchedule(WeeklySchedule s)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<Exception> GetExceptions()
        {
            throw new NotImplementedException();
        }

        public int AddException(Exception e)
        {
            throw new NotImplementedException();
        }

        public void DeleteException(Exception e)
        {
            throw new NotImplementedException();
        }

        public void DeleteAlarm(Alarm alarm)
        {
            throw new NotImplementedException();
        }

        public void DeleteSchedule(Schedule schedule)
        {
            throw new NotImplementedException();
        }

        public void DeleteWeeklySchedule(WeeklySchedule schedule)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<Sound> GetSounds()
        {
            throw new NotImplementedException();
        }

        public void AddSound(Sound sound)
        {
            throw new NotImplementedException();
        }

        public void ClearSoundDependencies(Sound s)
        {
            throw new NotImplementedException();
        }

        public void ClearScheduleDependencies(Schedule s)
        {
            throw new NotImplementedException();
        }

        public void DeleteSound(Sound sound)
        {
            throw new NotImplementedException();
        }

        public int SaveAll()
        {
            throw new NotImplementedException();
        }
    }
}