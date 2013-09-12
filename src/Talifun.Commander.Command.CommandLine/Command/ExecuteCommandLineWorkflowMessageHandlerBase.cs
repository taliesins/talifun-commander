using System.Threading;
using System.Threading.Tasks;
using Talifun.Commander.Command.CommandLine.Command.Request;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.CommandLine.Command
{
	public abstract class ExecuteCommandLineWorkflowMessageHandlerBase
	{
		protected bool ExecuteCommandLineExecutor(IExecuteCommandLineWorkflowMessage message, string workingDirectory, string commandPath, string commandArguments, out string output)
		{
			var commandLineExecutor = new CommandLineExecutor();
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var commandLineExecutorOutput = string.Empty;

			var task = Task.Factory.StartNew(
				() => commandLineExecutor.Execute(cancellationToken, workingDirectory, commandPath, commandArguments, out commandLineExecutorOutput)
				, cancellationToken);

			CommandLineService.CommandLineExecutors.Add(message, new CancellableTask
			{
				Task = task,
				CancellationTokenSource = cancellationTokenSource
			});

			try
			{
				var result = task.Result;
				output = commandLineExecutorOutput;
				return result;
			}
			finally
			{
				CommandLineService.CommandLineExecutors.Remove(message);
			}
		}
	}
}
