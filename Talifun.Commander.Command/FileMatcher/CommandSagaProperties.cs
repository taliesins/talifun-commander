using System.Collections.Generic;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.FileMatcher
{
    public class CommandSagaProperties : ICommandSagaProperties
    {
    	public string InputFilePath { get; set; }
        public ProjectElement Project { get; set; }
        public FileMatchElement FileMatch { get; set; }
		public IDictionary<string, string> AppSettings { get; set; }
    }
}
