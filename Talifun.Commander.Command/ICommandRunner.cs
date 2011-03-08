using System.ComponentModel.Composition;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command
{
    [InheritedExport]
    public interface ICommandRunner
    {
        string ConversionType { get; }
        void Run(ICommanderManager commanderManager, FileInfo inputFilePath, ProjectElement project, FileMatchElement fileMatch);
    }
}