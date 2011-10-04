using System.Collections.Specialized;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    public class CommandSagaProperties : ICommandSagaProperties
    {
        public ICommanderManager CommanderManager { get; set; }
        public FileInfo InputFilePath { get; set; }
        public ProjectElement Project { get; set; }
        public FileMatchElement FileMatch { get; set; }
		public NameValueCollection AppSettings { get; set; }
    }
}
