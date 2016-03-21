using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Alarm_Manager.Model
{
    public class DataService : IDataService
    {
        private ASDatabaseEntities entities;
        public DataService()
        {
            entities=new ASDatabaseEntities();
            entities.Configuration.AutoDetectChangesEnabled = true;
          

        }
      


        public ObservableCollection<Schedule> GetSchedules()
        {
            //var schedules = new ObservableCollection<Schedule>();
            //foreach (var schedule in entities.Schedules)
            //{
            //    schedules.Add(schedule);

            //}
            //return schedules;
            entities.Schedules.Load();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(entities.Schedules.Local);
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    
            return entities.Schedules.Local;
            

        }

        public void AddSchedule(Schedule s)
        {
            entities.Schedules.Add(s);
             entities.SaveChanges();
             entities.Schedules.Load();
        }

        public ObservableCollection<Alarm> GetAlarms()
        {
            var alarms = new ObservableCollection<Alarm>();
            foreach (var alarm in entities.Alarms)
            {
                alarms.Add(alarm);

            }
            return alarms;
        }

        public int AddAlarm(Alarm a)
        {
            entities.Alarms.Add(a);
            entities.SaveChanges();
          

            entities.Schedules.Load();
            return 1;
        }

        public ObservableCollection<WeeklySchedule> GetWeeklySchedules()
        {
            entities.WeeklySchedules.Load();
            return entities.WeeklySchedules.Local;
        }

        public WeeklySchedule GetDefaultWeeklySchedule()
        {
            if (!entities.WeeklySchedules.Any())
            {
                entities.WeeklySchedules.Add(new WeeklySchedule {Name = "DefaultWeeklySchedule"});
                entities.SaveChanges();
               
            }
            entities.WeeklySchedules.Load();
            return entities.WeeklySchedules.Local.First();
        }

        public int AddWeeklySchedule(WeeklySchedule s)
        {
            entities.WeeklySchedules.Add(s);
            return entities.SaveChanges();
        }


        public ObservableCollection<Exception> GetExceptions()
        {
           entities.Exceptions.Load();
           try
           {
               CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(entities.Exceptions.Local);
               view.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
           }
           catch (System.Exception)
           {
               
           }
            return entities.Exceptions.Local;
        }

        public int AddException(Exception exception)
        {
           

            entities.Exceptions.Add(exception);
            return entities.SaveChanges();

        }

        public void DeleteException(Exception e)
        {
            entities.Exceptions.Remove(e);
            entities.SaveChanges();
            entities.Exceptions.Load();

        }

        public void DeleteAlarm(Alarm alarm)
        {
            entities.Alarms.Remove(alarm);
            entities.SaveChanges();
            entities.Schedules.Load();
          
        }
        
        public void DeleteSchedule(Schedule schedule)
        {
           
            
            ClearScheduleDependencies(schedule );
            entities.SaveChanges();
            entities.Schedules.Remove(schedule);
            entities.SaveChanges();
            entities.Schedules.Load();
        }

        public void DeleteWeeklySchedule(WeeklySchedule schedule)
        {
            entities.WeeklySchedules.Remove(schedule);
            entities.SaveChanges();
            entities.WeeklySchedules.Load();
        }


        public void ClearScheduleDependencies(Schedule schedule)
        {
            var list = new List<Alarm>(schedule.Alarms);
            foreach (var alarm in list)
            {
                entities.Alarms.Remove(alarm);
                entities.SaveChanges();
            }
            var list2 = new List<Exception>(schedule.Exceptions);
            foreach (var exception in list2)
            {
                entities.Exceptions.Remove(exception);
                entities.SaveChanges();
            }
            var dschedule = GetDefaultWeeklySchedule();
            if (dschedule.Schedule == schedule) dschedule.Schedule = null;
            if (dschedule.Schedule1 == schedule) dschedule.Schedule1 = null;
            if (dschedule.Schedule2 == schedule) dschedule.Schedule2 = null;
            if (dschedule.Schedule3 == schedule) dschedule.Schedule3 = null;
            if (dschedule.Schedule4 == schedule) dschedule.Schedule4 = null;
            if (dschedule.Schedule5 == schedule) dschedule.Schedule5 = null;
            if (dschedule.Schedule6 == schedule) dschedule.Schedule6 = null;
            entities.SaveChanges();
          
        }

        public void DeleteSound(Sound sound)
        {   ClearSoundDependencies(sound);
            entities.Sounds.Remove(sound);
            entities.SaveChanges();
            entities.Sounds.Load();
        }

        public int SaveAll()
        {
            entities.SaveChanges();
           return entities.SaveChanges();
        }


        public ObservableCollection<Sound> GetSounds()
        {
           entities.Sounds.Load();
         return   entities.Sounds.Local;
        }


        public void AddSound(Sound sound)
        {
            entities.Sounds.Add(sound);
            entities.SaveChanges();
            entities.Sounds.Load();

        }

        public void ClearSoundDependencies(Sound sound)
        {
            var list = new List<Alarm>(sound.Alarms);
            foreach (var alarm in list)
            {
                entities.Alarms.Remove(alarm);
            }
           
            entities.SaveChanges();
        }
    }
}