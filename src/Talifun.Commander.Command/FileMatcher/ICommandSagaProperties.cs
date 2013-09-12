using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.FileMatcher
{
    public interface ICommandSagaProperties
    {
        string InputFilePath { get; }
        ProjectElement Project { get; }
        FileMatchElement FileMatch { get; }
    	IDictionary<string, string> AppSettings { get; }
    }
}
