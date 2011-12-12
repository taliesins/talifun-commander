using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.BoxNetUploader.Properties;

namespace Talifun.Commander.Command.BoxNetUploader.Configuration
{
	public class BoxNetUploaderElementValidator : AbstractValidator<BoxNetUploaderElement>
	{
		public BoxNetUploaderElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageBoxNetUploaderElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderSettings.Projects
					.Where(x => x.CommandPlugins
						.Where(y => y.Setting.ElementType == typeof(BoxNetUploaderElement))
						.Cast<BoxNetUploaderElementCollection>()
						.SelectMany(y => y)
						.Where(y=>y.Name == name)
						.Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Talifun.Commander.Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);
		}
	}
}
