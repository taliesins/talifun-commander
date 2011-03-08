namespace Talifun.Commander.Executor
{
    public interface IExectutor
    {
        bool Execute(string workingDirectory, string commandPath, string commandArguments, out string output);
    }
}
