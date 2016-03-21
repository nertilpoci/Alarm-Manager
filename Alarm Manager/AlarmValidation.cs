using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Alarm_Manager.ViewModel;

namespace Alarm_Manager
{
    class AlarmValidation :ValidationRule
    {


        /// <summary>

        /// Validates the proposed value.

        /// </summary>

        /// <param name="value">The proposed value.</param>

        /// <param name="cultureInfo">A CultureInfo.</param>

        /// <returns>The result of the validation.</returns>

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null)
            {
                return (new ViewModelLocator()).Schedules.SelectedItem.Alarms.Any(z => z.Time.TimeOfDay == Convert.ToDateTime(value).TimeOfDay) ? new ValidationResult(false, "Alarm Already Already Exists") : new ValidationResult(true, null);
            }
                return new ValidationResult(false, "Value Cannot be null ");
        }
    }
}
