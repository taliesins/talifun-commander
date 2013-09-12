using System.Linq;
using FluentValidation;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.FlickrUploader.Properties;

namespace Talifun.Commander.Command.FlickrUploader.Configuration
{
	public class FlickrUploaderElementValidator : AbstractValidator<FlickrUploaderElement>
	{
		public FlickrUploaderElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageFlickrUploaderElementNameMandatory)
				.Must((name) => !CurrentConfiguration.CommanderSettings.Projects
					.Where(x => x.CommandPlugins
						.Where(y => y.Setting.ElementType == typeof(FlickrUploaderElement))
						.Cast<FlickrUploaderElementCollection>()
						.SelectMany(y => y)
						.Where(y=>y.Name == name)
						.Count() > 1)
					.Any())
				.WithLocalizedMessage(() => Talifun.Commander.Command.Properties.Resource.ValidatorMessageProjectElementNameHasAlreadyBeenUsed);
		}
	}
}
