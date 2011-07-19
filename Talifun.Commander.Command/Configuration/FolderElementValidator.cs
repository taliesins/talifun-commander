using FluentValidation;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Configuration
{
    public class FolderElementValidator : AbstractValidator<FolderElement>
    {
        public FolderElementValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageFolderElementNameMandatory);
        }
    }
}
