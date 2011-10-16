using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Configuration
{
    public class ProjectElementValidator : AbstractValidator<ProjectElement>
    {
        public ProjectElementValidator()
        {
        	RuleFor(x => x.Name)
        		.NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageProjectElementNameMandatory)
				.Must((name) => CurrentConfiguration.CommanderConfiguration.Projects.Cast<ProjectElement>().Where(x=>x.Name == name).Count() < 2).WithLocalizedMessage(() => Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUses);

        }
    }
}
