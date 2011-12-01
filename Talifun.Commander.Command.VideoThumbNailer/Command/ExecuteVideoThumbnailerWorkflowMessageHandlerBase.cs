using System.Threading;
using System.Threading.Tasks;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.VideoThumbNailer.Command.Request;
using Talifun.Commander.Executor.FFMpeg;

namespace Talifun.Commander.Command.VideoThumbnailer.Command
{
	public abstract class ExecuteVideoThumbnailerWorkflowMessageHandlerBase
	{
		protected bool ExecuteFfMpegCommandLineExecutor(IExecuteVideoThumbnailerWorkflowMessage message, FfMpegCommandLineExecutor ffMpegCommandLineExecutor, string workingDirectory, string commandPath, string commandArguments, out string output)
		{
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var commandLineExecutorOutput = string.Empty;

			var task = Task.Factory.StartNew(
				() => ffMpegCommandLineExecutor.Execute(cancellationToken, workingDirectory, commandPath, commandArguments, out commandLineExecutorOutput)
				, cancellationToken);

			VideoThumbnailerService.CommandLineExecutors.Add(message, new CancellableTask
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
				VideoThumbnailerService.CommandLineExecutors.Remove(message);
			}
		}
	}
}
