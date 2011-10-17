using System.Configuration;
using System.IO;

namespace Talifun.Commander.Command
{
    public interface ICommand<TSettings>
    {
        bool Run(TSettings settings, AppSettingsSection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output);
    }
}