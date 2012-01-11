using Talifun.Commander.Command.Esb;

namespace Talifun.Commander.Command.Plugins.Request
{
	public interface ICreateTempDirectoryMessage : ICommandIdentifier
	{
		string Prefix { get; set; }
		string InputFilePath { get; set; }
		string WorkingDirectoryPath { get; set; }
	}
}
