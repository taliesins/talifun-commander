using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Audio.Properties;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Audio.Configuration
{
	public class AudioConversionElementValidator : AbstractValidator<AudioConversionElement>
	{
		public AudioConversionElementValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageAudioConversionElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderSettings.Projects
					.Where(x => x.CommandPlugins
						.Where(y => y.Setting.ElementType == typeof(AudioConversionElement))
						.Cast<AudioConversionElementCollection>()
						.SelectMany(y => y)
						.Where(y=>y.Name == name)
						.Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Talifun.Commander.Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);
        }
	}
}
