using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Classes.Helpers
{
    public static class ValidationHelper
    {
        public static string Errors { get; set; }
        public static bool IsModelValid(Object? obj)
        {
            if (obj == null)
                return false;

            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
            if (!isValid)
            {
                Errors = string.Join("\n", validationResults.Select(v => v.ErrorMessage));
            }
            return isValid;
        }
    }
}
