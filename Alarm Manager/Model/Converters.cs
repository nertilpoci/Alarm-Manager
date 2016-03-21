using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Alarm_Manager.ViewModel;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace Alarm_Manager.Model
{
    public class IndexValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (ListViewItem)value;
            var listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item) + 1;
            return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

      
    }
    public class SliderValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(System.Convert.ToDouble(value.ToString()) * 1.53) + 100;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class RemainingToPercent : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value != null)
            {

                var main = (new ViewModelLocator()).Main;
                //if (main.CurrentAlarm != null)
                //{

                //    var totalTimeSpan = main.CurrentAlarm.Time.TimeOfDay - main.PreviousAlarmTime.TimeOfDay;
                //    var passedTimeSpan = DateTime.Now.TimeOfDay - main.PreviousAlarmTime.TimeOfDay;
                //    int remaining = (int)((passedTimeSpan.TotalMilliseconds / totalTimeSpan.TotalMilliseconds) * 100);

                //    return remaining <= 1 || remaining > 100 ? 2 : remaining;

                //}
                //else
                //{
                //    var totalTimeSpan = main.NextAlarm.First().Value.Add(main.NextAlarm.First().Key.Time.TimeOfDay) - main.PreviousAlarmTime;
                //    var passedTimeSpan = DateTime.Now - main.PreviousAlarmTime;
                //    int remaining = (int)((passedTimeSpan.TotalMilliseconds / totalTimeSpan.TotalMilliseconds) * 100);

                //    return remaining <= 1 || remaining > 100 ? 2 : remaining;   
                //}
                //MessageBox.Show(System.Convert.ToInt32(value).ToString());
                //int seconds = System.Convert.ToInt32(value);
                //MessageBox.Show(seconds.ToString());
                decimal remaining = (int)(( (60m - main.RTime.Seconds) / 60m) * 100m);
               
                return remaining;

            }
                     return 50;
             
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class SoundFileShortener : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value.ToString().Split('/').Last().ToString();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class ZeroTuNull : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value) == 0 ? null : value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class DateTimeToTime : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return System.Convert.ToDateTime(value).ToShortTimeString();

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class FalseToHidden : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            
            return   value != null && (System.Convert.ToBoolean(value)) ? Visibility.Visible : Visibility.Collapsed;
           

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
 
  
    public class ZeroToHidden : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (System.Convert.ToInt32(value) <= 0) return Visibility.Collapsed;
            return Visibility.Visible;

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ZeroToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (System.Convert.ToInt32(value) <= 0) return Visibility.Visible;
            else return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class SelectedIndexToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (System.Convert.ToInt32(value) != -1) return Visibility.Visible;
             return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class StatusToColor : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return Brushes.WhiteSmoke;


        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class IsCurrentAlarm : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var main = (new ViewModelLocator()).Main;
            var schedules = (new ViewModelLocator()).Schedules;
            if (value != null &&  main.CurrentAlarm!=null && main.CurrentAlarm.Schedule1==schedules.SelectedItem)
            {
                return (System.Convert.ToDateTime(value).TimeOfDay ==
                        main.CurrentAlarm.Time.TimeOfDay);
            }

             return false;

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class DateShortener : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return System.Convert.ToDateTime(value).ToShortDateString();


        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return System.Convert.ToDateTime(value);
        }
    }
    public class StatusToLogo : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == "Not Running - You havent set up a default weekly schedule") return "F1 M 41.1667,44.3333L 55.4167,44.3333L 55.4167,58.5833L 41.1667,58.5833L 41.1667,44.3333 Z M 50.6667,38C 50.6667,39.0845 50.5576,40.1435 50.3499,41.1667L 46.2814,41.1667L 46.6036,39.5834L 42.75,39.5834L 42.75,36.4167L 46.6036,36.4167C 45.8986,31.1249 41.7085,26.9348 36.4166,26.2297L 36.4166,30.0834L 33.25,30.0834L 33.25,26.2297C 27.9581,26.9348 23.768,31.1249 23.0629,36.4167L 26.9166,36.4167L 26.9166,39.5834L 23.0629,39.5834C 23.768,44.8752 27.9581,49.0653 33.25,49.7704L 33.25,45.9167L 36.4166,45.9167L 36.4166,49.7704L 38,49.4481L 38,53.5166C 36.9768,53.7243 35.9178,53.8333 34.8333,53.8333C 26.0888,53.8333 19,46.7445 19,38C 19,29.7899 25.2489,23.0392 33.25,22.2449L 33.25,20.5833L 28.5,20.5833L 28.5,15.8333L 41.1667,15.8333L 41.1667,20.5833L 36.4167,20.5833L 36.4167,22.2449C 39.6011,22.561 42.508,23.8207 44.8538,25.7403L 45.9962,24.5979L 43.757,22.3588L 47.1158,19L 53.8333,25.7175L 50.4746,29.0763L 48.2354,26.8371L 47.093,27.9795C 49.3266,30.709 50.6667,34.198 50.6667,38 Z M 34.8333,34.8334C 36.5822,34.8334 37.9999,36.2512 37.9999,38.0001C 37.9999,39.7489 36.5822,41.1667 34.8333,41.1667L 28.5,45.9167L 31.6666,38.0001C 31.6666,36.2512 33.0844,34.8334 34.8333,34.8334 Z ";
            else return "F1 M 53.8333,41.1667C 53.8333,49.9112 46.7445,57 38,57C 29.2555,57 22.1667,49.9112 22.1667,41.1667C 22.1667,32.9565 28.4156,26.2059 36.4167,25.4115L 36.4167,23.75L 31.6667,23.75L 31.6667,19L 44.3333,19L 44.3333,23.75L 39.5833,23.75L 39.5833,25.4115C 42.7678,25.7277 45.6747,26.9874 48.0205,28.907L 49.1629,27.7646L 46.9237,25.5254L 50.2825,22.1667L 57,28.8842L 53.6412,32.2429L 51.4021,30.0038L 50.2597,31.1462C 52.4933,33.8756 53.8333,37.3647 53.8333,41.1667 Z M 26.2296,39.5834L 30.0833,39.5834L 30.0833,42.75L 26.2296,42.75C 26.9347,48.0419 31.1248,52.232 36.4166,52.9371L 36.4166,49.0833L 39.5833,49.0833L 39.5833,52.937C 44.8752,52.232 49.0653,48.0419 49.7703,42.75L 45.9166,42.75L 45.9166,39.5834L 49.7703,39.5834C 49.0652,34.2915 44.8751,30.1014 39.5833,29.3964L 39.5833,33.25L 36.4166,33.25L 36.4166,29.3963C 31.1248,30.1014 26.9347,34.2915 26.2296,39.5834 Z M 38,38C 39.7489,38 41.1666,39.4178 41.1666,41.1667C 41.1666,42.9156 39.7489,44.3334 38,44.3334L 31.6666,49.0834L 34.8333,41.1667C 34.8333,39.4178 36.2511,38 38,38 Z ";


        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class DateTimeToShortTime : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {



            return System.Convert.ToDateTime(value).ToString();

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
          return System.Convert.ToDateTime(value).TimeOfDay;
           
        }
    }
    public class ShortDateFormat : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return String.Format("{0:ddd, MMM d, yyyy}", System.Convert.ToDateTime(value)); 

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
         return null; 
        }
    }
    public class TimeTo12HourFormat : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return String.Format("{0:t}", System.Convert.ToDateTime(value));

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class EmptyToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value.ToString());

        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class EmptyDateToFalse : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {

            return value!=null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class SlectedIndexNegativeToFalse : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            
            return System.Convert.ToInt32(value) != -1;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class InverseBooleanValue : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class DefaultWeeklyScheduleStatus : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            //var defaultweeklyschedule = (value as WeeklySchedule);
            //if (defaultweeklyschedule != null && (defaultweeklyschedule.Schedule != null && defaultweeklyschedule.Schedule.Alarms.Count >= 0)) return Visibility.Visible;
            //if (defaultweeklyschedule != null && (defaultweeklyschedule.Schedule1 != null && defaultweeklyschedule.Schedule1.Alarms.Count >= 0)) return Visibility.Visible;
            //if (defaultweeklyschedule != null && (defaultweeklyschedule.Schedule2 != null && defaultweeklyschedule.Schedule2.Alarms.Count >= 0)) return Visibility.Visible;
            //if (defaultweeklyschedule != null && (defaultweeklyschedule.Schedule3 != null && defaultweeklyschedule.Schedule3.Alarms.Count >= 0)) return Visibility.Visible;
            //if (defaultweeklyschedule != null && (defaultweeklyschedule.Schedule4 != null && defaultweeklyschedule.Schedule4.Alarms.Count >= 0)) return Visibility.Visible;
            //if (defaultweeklyschedule != null && (defaultweeklyschedule.Schedule5 != null && defaultweeklyschedule.Schedule5.Alarms.Count >= 0)) return Visibility.Visible;
            //if (defaultweeklyschedule != null && (defaultweeklyschedule.Schedule6 != null && defaultweeklyschedule.Schedule6.Alarms.Count >= 0)) return Visibility.Visible;
            if ((new ViewModelLocator()).Main.NextPossibleAlarm() != null) return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
           
                return null;
           
        }
    }
    public class OrderAlarms : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            

            return (value as Schedule).Alarms.OrderBy(z=>System.Convert.ToDateTime(z.Time).TimeOfDay);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {

            return null;

        }
    }

  
}
