using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.ImageSettings;
using Talifun.Commander.Command.Image.Command.Request;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.Image.Command
{
	public abstract class ExecuteImageConversionWorkflowMessageHandlerBase
	{
		protected bool ExecuteCommandLineExecutor(IExecuteImageConversionWorkflowMessage message, string workingDirectory, string commandPath, string commandArguments, out string output)
		{
			var commandLineExecutor = new CommandLineExecutor();
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;

			var commandLineExecutorOutput = string.Empty;

			var task = Task.Factory.StartNew(
				() => commandLineExecutor.Execute(cancellationToken, workingDirectory, commandPath, commandArguments, out commandLineExecutorOutput)
				, cancellationToken);

			ImageConversionService.CommandLineExecutors.Add(message, new CancellableTask
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
				ImageConversionService.CommandLineExecutors.Remove(message);
			}
		}

		protected string WatermarkArguments(IImageResizeSettings settings, string outPutFilePath)
		{
			var watermarkArguments = string.Empty;

			if (!string.IsNullOrEmpty(settings.WatermarkPath))
			{
				var watermarkOptions = new Dictionary<string, string>()
			                   	{
			                   		{"-dissolve", settings.WatermarkDissolveLevels.ToString()},
									{"-gravity", Enum.GetName(typeof(Gravity), settings.WatermarkGravity)}
			                   	};

				var commandArguments = watermarkOptions.Select(x => x.Key + " " + x.Value).Aggregate(new StringBuilder(), (x, y) => x.Append(" " + y)).ToString();

				watermarkArguments = string.Format("{0} {1} {2} {2}", commandArguments.ToString(), settings.WatermarkPath, outPutFilePath);
			}

			return watermarkArguments;
		}

		protected string ThumbnailArguments(IImageResizeSettings settings, string inputFilePath, string outPutFilePath)
		{
			var backgroundColour = settings.BackgroundColour;
			if (!string.IsNullOrEmpty(backgroundColour))
			{
				if (backgroundColour == "#00FFFFFF")
				{
					backgroundColour = "#00000000"; //This is imagemagicks transparent color
				}
				backgroundColour = " -background " + backgroundColour;
			}

			var quality = string.Empty;
			if (settings.Quality.HasValue)
			{
				quality = " -quality " + settings.Quality.Value;
			}

			var commandArguments = string.Empty;
			switch (settings.ResizeMode)
			{
				case ResizeMode.AreaToFit:
					{
						var area = settings.Width * settings.Height;
						commandArguments =
							String.Format(
								"-define jpeg:size=\"{0}x{1}\" \"{2}\" -thumbnail \"{3}@\"{4} -gravity {5} -extent \"{6}x{7}\"{8}",
								settings.Width * 2, settings.Height * 2, inputFilePath, area, backgroundColour,
								Enum.GetName(typeof(Gravity), settings.Gravity), settings.Width, settings.Height, quality);
						break;
					}
				case ResizeMode.CutToFit:
					{
						commandArguments =
							String.Format("-define jpeg:size=\"{0}x{1}\" \"{2}\" -thumbnail \"{3}x{4}^\" -gravity {5} -extent \"{6}x{7}\"{8}",
										  settings.Width * 2, settings.Height * 2, inputFilePath, settings.Width, settings.Height,
										  Enum.GetName(typeof(Gravity), settings.Gravity), settings.Width,
										  settings.Height, quality);
						break;
					}
				case ResizeMode.Zoom:
					{
						commandArguments =
							String.Format(
								"-define jpeg:size=\"{0}x{1}\" \"{2}\" -thumbnail \"{3}x{4}>\" {5} -gravity {6} -extent \"{7}x{8}\"{9}",
								settings.Width * 2, settings.Height * 2, inputFilePath, settings.Width, settings.Height, backgroundColour,
								Enum.GetName(typeof(Gravity), settings.Gravity), settings.Width, settings.Height, quality);
						break;
					}
				case ResizeMode.Stretch:
					{
						commandArguments = String.Format("-define jpeg:size=\"{0}x{1}\" \"{2}\" -thumbnail \"{3}x{4}!\"{5}",
							settings.Width * 2, settings.Height * 2, inputFilePath, settings.Width, settings.Height, quality);
						break;
					}
				case ResizeMode.FitWidth:
					{
						commandArguments = String.Format("-define jpeg:size=\"{0}\" \"{1}\" -thumbnail \"{2}\"{3}",
							settings.Width * 2, inputFilePath, settings.Width, quality);
						break;
					}
				case ResizeMode.FitMaximumWidth:
					{
						commandArguments = String.Format("-define jpeg:size=\"{0}>\" \"{1}\" -thumbnail \"{2}>\"{3}",
							settings.Width * 2, inputFilePath, settings.Width, quality);
						break;
					}
				case ResizeMode.FitMinimumWidth:
					{
						commandArguments = String.Format("-define jpeg:size=\"{0}\" \"{1}\" -thumbnail \"{2}^\"{3}",
							settings.Width * 2, inputFilePath, settings.Width, quality);
						break;
					}
				case ResizeMode.FitHeight:
					{
						commandArguments = String.Format("-define jpeg:size=\"x{0}\" \"{1}\" -thumbnail \"x{2}\"{3}",
							settings.Height * 2, inputFilePath, settings.Height, quality);
						break;
					}
				case ResizeMode.FitMaximumHeight:
					{
						commandArguments = String.Format("-define jpeg:size=\"x{0}\" \"{1}\" -thumbnail \"x{2}>\"{3}",
							settings.Height * 2, inputFilePath, settings.Height, quality);
						break;
					}
				case ResizeMode.FitMinimumHeight:
					{
						commandArguments = String.Format("-define jpeg:size=\"x{0}\" \"{1}\" -thumbnail \"x{2}^\"{3}",
							settings.Height * 2, inputFilePath, settings.Height, quality);
						break;
					}
				default:
					{
						commandArguments = String.Format("\"{0}\"", inputFilePath);
						break;
					}
			}

			commandArguments += string.Format(" \"{0}\"", outPutFilePath);

			return commandArguments;
		}
	}
}
