using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.YouTubeUploader.Properties;

namespace Talifun.Commander.Command.YouTubeUploader.Configuration
{
	public class YouTubeUploaderElementValidator : AbstractValidator<YouTubeUploaderElement>
	{
		public YouTubeUploaderElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageYouTubeUploaderElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderSettings.Projects.Cast<ProjectElement>()
					.Where(x => x.CommandPlugins
						.Where(y => y.Setting.ElementType == typeof(YouTubeUploaderElement))
						.Cast<YouTubeUploaderElementCollection>()
						.SelectMany(y => y.Cast<YouTubeUploaderElement>())
						.Where(y=>y.Name == name)
						.Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);
		}
	}
}
