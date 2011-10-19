using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Configuration
{
    public class FolderElementValidator : AbstractValidator<FolderElement>
    {
        public FolderElementValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageFolderElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderSettings.Projects.Cast<ProjectElement>()
					.Where(x => x.Folders.Cast<FolderElement>()
						.Where(y => y.Name == name).Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);
        }
    }
}
