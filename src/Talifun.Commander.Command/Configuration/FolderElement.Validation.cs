using System;
using System.ComponentModel;
using System.Linq;
using FluentValidation.Results;

namespace Talifun.Commander.Command.Configuration
{
    public partial class FolderElement : IDataErrorInfo
    {
        public bool IsValid
        {
            get { return SelfValidate().IsValid; }
        }

        private ValidationResult SelfValidate()
        {
            return ValidationHelper.Validate<FolderElementValidator, FolderElement>(this);
        }

        string IDataErrorInfo.Error
        {
            get { return ValidationHelper.GetError(SelfValidate()); }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                var validationResults = SelfValidate();
                if (validationResults == null) return String.Empty;
                var columnResults = validationResults.Errors.FirstOrDefault<ValidationFailure>(x => String.Compare(x.PropertyName, columnName, true) == 0);
                return columnResults != null ? columnResults.ErrorMessage : String.Empty;
            }
        }
    }
}
