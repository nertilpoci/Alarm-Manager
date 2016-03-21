using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Alarm_Manager
{
    internal class DuplicateScheduleValidation : ValidationRule
    {

        /// <summary>

        /// Validates the proposed value.

        /// </summary>

        /// <param name="value">The proposed value.</param>

        /// <param name="cultureInfo">A CultureInfo.</param>

        /// <returns>The result of the validation.</returns>

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null && value.ToString() != "")
            {
                return (new ASDatabaseEntities()).Schedules.Any(z => z.Name == (string)value) ? new ValidationResult(false, "Name Already Exists") : new ValidationResult(true, null);
            }
            return new ValidationResult(false, "Name Already Exists");
           

        }

    }
}
