using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.AntiVirus.Properties;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.AntiVirus.Configuration
{
	public class AntiVirusElementValidator : AbstractValidator<AntiVirusElement>
	{
		public AntiVirusElementValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageAntiVirusElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderConfiguration.Projects.Cast<ProjectElement>()
					.Where(x => x.GetCommandConfiguration<AntiVirusElementCollection>(x.GetConfigurationProperty(AntiVirusConfiguration.Instance.ElementCollectionSettingName)).Cast<AntiVirusElement>()
						.Where(y => y.Name == name).Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUses);
        }
	}
}
