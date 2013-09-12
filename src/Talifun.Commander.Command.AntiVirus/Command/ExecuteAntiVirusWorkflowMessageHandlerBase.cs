using System.Threading;
using System.Threading.Tasks;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.AntiVirus.Command
{
	public abstract class ExecuteAntiVirusWorkflowMessageHandlerBase
	{
		protected bool ExecuteCommandLineExecutor(IExecuteAntiVirusWorkflowMessage message, string workingDirectory, string commandPath, string commandArguments, out string output)
		{
			var commandLineExecutor = new CommandLineExecutor();
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var commandLineExecutorOutput = string.Empty;

			var task = Task.Factory.StartNew(
				() => commandLineExecutor.Execute(cancellationToken, workingDirectory, commandPath, commandArguments, out commandLineExecutorOutput)
				, cancellationToken);

			AntiVirusService.CommandLineExecutors.Add(message, new CancellableTask
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
				AntiVirusService.CommandLineExecutors.Remove(message);
			}
		}
	}
}
