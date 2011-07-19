using FluentValidation;
using Talifun.Commander.Command.AntiVirus.Properties;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
	public class AntiVirusElementValidator : AbstractValidator<AntiVirusElement>
	{
		public AntiVirusElementValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageAntiVirusElementNameMandatory);
        }
	}
}
