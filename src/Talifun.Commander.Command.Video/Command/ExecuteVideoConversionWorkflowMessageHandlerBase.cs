using System.Threading;
using System.Threading.Tasks;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Video.Command.Request;
using Talifun.Commander.Executor.CommandLine;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.Video.Command
{
	public abstract class ExecuteVideoConversionWorkflowMessageHandlerBase
	{
		protected bool ExecuteFfMpegCommandLineExecutor(IExecuteVideoConversionWorkflowMessage message, string workingDirectory, string commandPath, string commandArguments, out string output)
		{
			var ffMpegCommandLineExecutor = new FfMpegCommandLineExecutor();

			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var commandLineExecutorOutput = string.Empty;

			var task = Task.Factory.StartNew(
				() => ffMpegCommandLineExecutor.Execute(cancellationToken, workingDirectory, commandPath, commandArguments, out commandLineExecutorOutput)
				, cancellationToken);

			VideoConversionService.CommandLineExecutors.Add(message, new CancellableTask
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
				VideoConversionService.CommandLineExecutors.Remove(message);
			}
		}

		protected bool ExecuteCommandLineExecutor(IExecuteVideoConversionWorkflowMessage message, string workingDirectory, string commandPath, string commandArguments, out string output)
		{
			var commandLineExecutor = new CommandLineExecutor();

			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var commandLineExecutorOutput = string.Empty;

			var task = Task.Factory.StartNew(
				() => commandLineExecutor.Execute(cancellationToken, workingDirectory, commandPath, commandArguments, out commandLineExecutorOutput)
				, cancellationToken);

			VideoConversionService.CommandLineExecutors.Add(message, new CancellableTask
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
				VideoConversionService.CommandLineExecutors.Remove(message);
			}
		}
	}
}
