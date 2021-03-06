﻿using System.Linq;
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
				.Must((name) => !CurrentConfiguration.CommanderSettings.Projects
					.Where(x => x.CommandPlugins
						.Where(y => y.Setting.ElementType == typeof(AntiVirusElement))
						.Cast<AntiVirusElementCollection>()
						.SelectMany(y => y)
						.Where(y=>y.Name == name)
						.Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Commander.Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);
        }
	}
}
