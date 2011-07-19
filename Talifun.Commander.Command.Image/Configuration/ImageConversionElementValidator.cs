using FluentValidation;
using Talifun.Commander.Command.Image.Properties;

namespace Talifun.Commander.Command.Image.Configuration
{
	public class ImageConversionElementValidator : AbstractValidator<ImageConversionElement>
	{
		public ImageConversionElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageImageConversionElementNameMandatory);
		}
	}
}
