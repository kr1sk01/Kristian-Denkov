using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService.Attributes
{
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
