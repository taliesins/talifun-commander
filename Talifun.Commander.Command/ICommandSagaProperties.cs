using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    public interface ICommandSagaProperties
    {
        FileInfo InputFilePath { get; }
        ProjectElement Project { get; }
        FileMatchElement FileMatch { get; }
    	AppSettingsSection AppSettings { get; }
    }
}
