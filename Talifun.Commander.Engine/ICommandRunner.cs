using System.IO;

namespace Talifun.Commander.MediaConversion
{
    public interface ICommandRunner<TSettings>
    {
        void Run(ICommanderManager commanderManager, FileInfo inputFilePath, TSettings settings);
    }
}