using System;
using System.Linq;
using System.Windows.Controls;
using Alarm_Manager.ViewModel;

namespace Alarm_Manager
{
    class ExceptionValidation :ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return (new ViewModelLocator()).Exceptions.Exceptions.Any(z => Convert.ToDateTime(z.Date).Date == Convert.ToDateTime(value).Date) ? new ValidationResult(false, "Exceptions At This Date  Already Already Exists") : new ValidationResult(true, null);
            }

            return new ValidationResult(false, "Value Cannot be null ");
        }
    }
}
