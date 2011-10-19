﻿using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.Image.Properties;

namespace Talifun.Commander.Command.Image.Configuration
{
	public class ImageConversionElementValidator : AbstractValidator<ImageConversionElement>
	{
		public ImageConversionElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageImageConversionElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderSettings.Projects.Cast<ProjectElement>()
					.Where(x => x.CommandPlugins
						.Where(y => y.Setting.ElementType == typeof(ImageConversionElement))
						.Cast<ImageConversionElementCollection>()
						.SelectMany(y => y.Cast<ImageConversionElement>())
						.Where(y=>y.Name == name)
						.Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);
		}
	}
}
