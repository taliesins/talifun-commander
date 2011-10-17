using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.VideoThumbNailer.Properties;

namespace Talifun.Commander.Command.VideoThumbNailer.Configuration
{
	public class VideoThumbnailerElementValidator : AbstractValidator<VideoThumbnailerElement>
	{
		public VideoThumbnailerElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageVideoThumbNailerElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderConfiguration.Projects.Cast<ProjectElement>()
					.Where(x => x.CommandPlugins
						.Where(y => y.Setting.ElementType == typeof(VideoThumbnailerElement)).Cast<VideoThumbnailerElement>()
						.Where(y => y.Name == name).Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);
		}
	}
}
