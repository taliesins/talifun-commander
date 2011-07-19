using FluentValidation;
using Talifun.Commander.Command.Audio.Properties;

namespace Talifun.Commander.Command.Audio.Configuration
{
	class AudioConversionElementValidator : AbstractValidator<AudioConversionElement>
	{
		public AudioConversionElementValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageAudioConversionElementNameMandatory);
        }
	}
}
