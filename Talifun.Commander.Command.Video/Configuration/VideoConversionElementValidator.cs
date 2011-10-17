using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Video.Properties;

namespace Talifun.Commander.Command.Video.Configuration
{
	public class VideoConversionElementValidator : AbstractValidator<VideoConversionElement>
	{
		public VideoConversionElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageVideoConversionElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderConfiguration.Projects.Cast<ProjectElement>()
					.Where(x => x.CommandPlugins
						.Where(y => y.Setting.ElementType == typeof(VideoConversionElement)).Cast<VideoConversionElement>()
						.Where(y => y.Name == name).Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);
		}
	}
}
