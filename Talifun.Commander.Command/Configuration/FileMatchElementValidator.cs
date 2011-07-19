using FluentValidation;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Configuration
{
    public class FileMatchElementValidator : AbstractValidator<FileMatchElement>
    {
        public FileMatchElementValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageFileMatchElementNameMandatory);
        }
    }
}
