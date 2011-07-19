using FluentValidation;
using Talifun.Commander.Command.Video.Properties;

namespace Talifun.Commander.Command.Video.Configuration
{
	public class VideoConversionElementValidator : AbstractValidator<VideoConversionElement>
	{
		public VideoConversionElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageVideoConversionElementNameMandatory);
		}
	}
}
