using System.Linq;
using System.Text.RegularExpressions;
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

			RuleFor(x => x.Expression).Must(x =>
			                                	{
													if (string.IsNullOrEmpty(x))
													{
														return true;
													}

													try
													{
														var regex = new Regex(x);
														var result = regex.IsMatch("test.txt");
														return true;
													}
													catch
													{
														return false;
													}
			                                	})
			.WithLocalizedMessage(() => Resource.ValidatorMessageFolderElementExpressionNotAValidRegularExpression);
        }
    }
}
