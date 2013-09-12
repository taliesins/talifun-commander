using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.DropBoxUploader.Properties;

namespace Talifun.Commander.Command.DropBoxUploader.Configuration
{
	public class DropBoxUploaderElementValidator : AbstractValidator<DropBoxUploaderElement>
	{
		public DropBoxUploaderElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageDropBoxUploaderElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderSettings.Projects
					.Where(x => x.CommandPlugins
						.Where(y => y.Setting.ElementType == typeof(DropBoxUploaderElement))
						.Cast<DropBoxUploaderElementCollection>()
						.SelectMany(y => y)
						.Where(y=>y.Name == name)
						.Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Talifun.Commander.Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);
		}
	}
}
