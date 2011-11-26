using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MassTransit;
using Talifun.Commander.Command.AntiVirus.Command.Request;
using Talifun.Commander.Command.AntiVirus.Command.Response;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.AntiVirus.Command
{
	public class ExecuteMcAfeeWorkflowMessageHandler : Consumes<ExecuteMcAfeeWorkflowMessage>.All
	{
		private const string AllFixedOptions = @"/Quiet /AllOle /Archive /Packers /Mime /Primary Clean /Secondary Delete /LowPriority";

		public void Consume(ExecuteMcAfeeWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);
			var outPutFilePath = new FileInfo(Path.Combine(message.WorkingDirectoryPath, inputFilePath.Name));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			inputFilePath.CopyTo(outPutFilePath.FullName);

			var commandPath = message.AppSettings[AntiVirusConfiguration.Instance.McAfeePathSettingName];
			var workingDirectory = message.WorkingDirectoryPath;
			var commandArguments = @"/target """ + outPutFilePath.FullName + @""" " + AllFixedOptions;

			var commandLineExecutor = new CommandLineExecutor();
			string output;
			var filePassed = commandLineExecutor.Execute(workingDirectory, commandPath, commandArguments, out output);

			if (filePassed)
			{
				outPutFilePath.Refresh();
				filePassed = outPutFilePath.Exists;
			}

			var executedMcAfeeWorkflowMessage = new ExecutedMcAfeeWorkflowMessage()
			{
				CorrelationId = message.CorrelationId,
				FilePassed = filePassed,
				OutPutFilePath = outPutFilePath.FullName,
				Output = output
			};

			var bus = BusDriver.Instance.GetBus(AntiVirusService.BusName);
			bus.Publish(executedMcAfeeWorkflowMessage);
		}

		private static readonly Regex GetPropertiesExpression = new Regex(@"([^\r\n]*)\s+:\s+([^\r\n]*)", RegexOptions.Compiled);

		/// <summary>
		/// Get a list of properties from the results of the McAfee scan.
		/// </summary>
		/// <param name="output">Output generated from McAfee scan.</param>
		/// <returns>A list of properties of the result from the McAfee scan.</returns>
		internal static Dictionary<string, string> GetProperties(string output)
		{
			var match = GetPropertiesExpression.Match(output);
			var properties = new Dictionary<string, string>();

			if (match.Success)
			{
				while (match.Success)
				{
					var groups = match.Groups;

					var propertyKey = groups[1].Value.Trim();
					var propertyValue = groups[2].Value.Trim();

					properties.Add(propertyKey, propertyValue);
					match = match.NextMatch();
				}
			}

			return properties;
		}
	}
}
