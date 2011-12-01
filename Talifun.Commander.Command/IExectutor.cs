using System.Threading;

namespace Talifun.Commander.Command
{
    public interface IExectutor
    {
        bool Execute(CancellationToken cancellationToken, string workingDirectory, string commandPath, string commandArguments, out string output);
    }
}
