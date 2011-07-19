using System;
using System.Linq;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace Talifun.Commander.Command.Configuration
{
    public class ValidationHelper
    {
        public static ValidationResult Validate<T, K>(K entity)
            where T : IValidator<K>, new()
            where K : class
        {
            IValidator<K> validator = new T();
            return validator.Validate(entity);
        }

        public static string GetError(ValidationResult result)
        {
            var validationErrors = result.Errors
                .Aggregate(new StringBuilder(), (current, next) => current.Append(next.ErrorMessage).Append(Environment.NewLine));
            return validationErrors.ToString();
        }
    }
}
