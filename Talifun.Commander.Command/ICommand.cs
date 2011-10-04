using System.Collections.Specialized;
using System.IO;

namespace Talifun.Commander.Command
{
    public interface ICommand<TSettings>
    {
        bool Run(TSettings settings, NameValueCollection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output);
    }
}