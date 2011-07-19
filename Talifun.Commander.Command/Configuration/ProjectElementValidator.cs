using FluentValidation;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Configuration
{
    public class ProjectElementValidator : AbstractValidator<ProjectElement>
    {
        public ProjectElementValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageProjectElementNameMandatory);
        }
    }
}
