using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService
{
    public class ServiceSettings
    {
        [Required]
        public string CsvDirectory { get; set; } = default!;

        [Required]
        public string ProcessedDirectory { get; set; } = default!;

        [Required]
        public string ErrorDirectory { get; set; } = default!;

        [Required]
        public string LogDirectory { get; set; } = default!;

        [RequiredNotEmptyOrNull]
        public string CsvSeparator { get; set; } = default!;
    }

    public class EmailSettings
    {
        [Required]
        public string Host { get; set; } = default!;

        [Range(1, 65535)]
        public int Port { get; set; }

        [Required]
        public string DisplayName { get; set; } = default!;

        [Required]
        public string Username { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        public bool EnableSsl { get; set; }

        [Required]
        [EmailAddress]
        public string ToEmail { get; set; } = default!;
    }

    public class ColumnIndexSettings
    {
        [Range(0, int.MaxValue)]
        public int MonthColumnIndex { get; set; }

        [Range(0, int.MaxValue)]
        public int DayColumnIndex { get; set; }

        [Range(0, int.MaxValue)]
        public int HourColumnIndex { get; set; }

        [Range(0, int.MaxValue)]
        public int TempColumnIndex { get; set; }

        [Range(0, int.MaxValue)]
        public int SoilTempColumnIndex { get; set; }

        public bool HasSoilTempColumn { get; set; }
    }

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
}
