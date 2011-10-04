using System.Collections.Specialized;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    public interface ICommandSagaProperties
    {
        ICommanderManager CommanderManager { get; }
        FileInfo InputFilePath { get; }
        ProjectElement Project { get; }
        FileMatchElement FileMatch { get; }
    	NameValueCollection AppSettings { get; }
    }
}
