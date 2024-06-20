using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService
{
    public class RequiredNotEmptyOrNullAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string str && string.IsNullOrWhiteSpace(str))
            {
                if (string.IsNullOrEmpty(str))
                {
                    return new ValidationResult($"{validationContext.DisplayName} is required.");
                }

                return ValidationResult.Success;
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }

    public class UniqueIndexesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var settings = (ColumnIndexSettings)validationContext.ObjectInstance;
            var indexes = new List<int>
            {
                settings.MonthColumnIndex,
                settings.DayColumnIndex,
                settings.HourColumnIndex,
                settings.TempColumnIndex
            };

            if (settings.HasSoilTempColumn)
            {
                indexes.Add(settings.SoilTempColumnIndex);
            }

            if (indexes.Count != indexes.Distinct().Count())
            {
                return new ValidationResult("All column indexes must be unique.");
            }

            return ValidationResult.Success;
        }
    }
}
