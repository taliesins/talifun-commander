namespace Talifun.Commander.Command.CommandLine
{
	public interface ICommandLineParameters
	{
		string CommandPath { get; set; }
		string CommandArguments { get; set; }
	}
}