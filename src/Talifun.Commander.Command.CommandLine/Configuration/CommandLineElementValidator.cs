﻿using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.CommandLine.Properties;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
	public class CommandLineElementValidator : AbstractValidator<CommandLineElement>
	{
		public CommandLineElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageCommandLineElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderSettings.Projects
					.Where(x => x.CommandPlugins
						.Where(y => y.Setting.ElementType == typeof(CommandLineElement))
						.Cast<CommandLineElementCollection>()
						.SelectMany(y => y)
						.Where(y=>y.Name == name)
						.Count() > 1)
					.Any())
				.WithLocalizedMessage(() =>Talifun.Commander.Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);

		}
	}
}
