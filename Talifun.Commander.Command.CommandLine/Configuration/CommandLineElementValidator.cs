using FluentValidation;
using Talifun.Commander.Command.CommandLine.Properties;

namespace Talifun.Commander.Command.CommandLine.Configuration
{
	public class CommandLineElementValidator : AbstractValidator<CommandLineElement>
	{
		public CommandLineElementValidator()
		{
			RuleFor(x => x.Name).NotEmpty().WithLocalizedMessage(() => Resource.ValidatorMessageCommandLineElementNameMandatory);
		}
	}
}
