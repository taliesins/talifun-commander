using System.IO;

namespace Talifun.Commander.Command
{
    public interface ICommand<TSettings>
    {
        bool Run(TSettings settings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output);
    }
}