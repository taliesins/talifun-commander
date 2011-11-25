using System.Collections.Generic;
using System.IO;

namespace Talifun.Commander.Command
{
    public interface ICommand<in TSettings>
    {
		bool Run(TSettings settings, IDictionary<string, string> appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output);
    }
}