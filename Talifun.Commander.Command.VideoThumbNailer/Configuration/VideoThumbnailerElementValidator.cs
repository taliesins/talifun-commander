using FluentValidation;
using Talifun.Commander.Command.VideoThumbNailer.Properties;

namespace Talifun.Commander.Command.VideoThumbNailer.Configuration
{
	public class VideoThumbnailerElementValidator : AbstractValidator<VideoThumbnailerElement>
	{
		public VideoThumbnailerElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageVideoThumbNailerElementNameMandatory);
		}
	}
}
