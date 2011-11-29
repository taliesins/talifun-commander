namespace Talifun.Commander.Command.CommandLine.Command.Arguments
{
	public interface ICommandLineParameters
	{
		string CommandPath { get; set; }
		string CommandArguments { get; set; }
	}
}