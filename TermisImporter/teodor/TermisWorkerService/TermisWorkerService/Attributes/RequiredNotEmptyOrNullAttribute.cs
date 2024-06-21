using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService.Attributes
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
}
