using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.Configuration
{
    public class FileMatchElementValidator : AbstractValidator<FileMatchElement>
    {
        public FileMatchElementValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageFileMatchElementNameMandatory)
				.Must((name) => !CurrentConfiguration.Current.Projects.Cast<ProjectElement>()
			                   	.Where(x => x.Folders.Cast<FolderElement>()
			                   	            	.SelectMany(y => y.FileMatches.Cast<FileMatchElement>())
			                   	            	.Where(y => y.Name == name)
			                   	            	.Count() > 1)
			                   	.Any())
				.WithLocalizedMessage(() => Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUses);
        }
    }
}
