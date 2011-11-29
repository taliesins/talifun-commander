using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MassTransit;
using Talifun.Commander.Command.Esb;
using Talifun.Commander.Command.Image.Command.ImageSettings;
using Talifun.Commander.Command.Image.Command.Request;
using Talifun.Commander.Command.Image.Command.Response;
using Talifun.Commander.Command.Image.Configuration;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.Image.Command
{
	public class ExecuteImageConversionWorkflowMessageHandler : Consumes<ExecuteImageConversionWorkflowMessage>.All
	{
		public void Consume(ExecuteImageConversionWorkflowMessage message)
		{
			var inputFilePath = new FileInfo(message.InputFilePath);

			var extension = "";
			switch (message.Settings.ResizeImageType)
			{
				case ResizeImageType.JPG:
					extension = ".jpg";
					break;
				case ResizeImageType.PNG:
					extension = ".png";
					break;
				case ResizeImageType.GIF:
					extension = ".gif";
					break;
				default:
					extension = Path.GetExtension(inputFilePath.Name);
					break;
			}

			var fileName = Path.GetFileNameWithoutExtension(inputFilePath.Name) + extension;
			var outPutFilePath = new FileInfo(Path.Combine(message.WorkingDirectoryPath, fileName));
			if (outPutFilePath.Exists)
			{
				outPutFilePath.Delete();
			}

			var output = string.Empty;

			var convertPath = message.AppSettings[ImageConversionConfiguration.Instance.ConvertPathSettingName];
			var convertArguments = ThumbnailArguments(message.Settings, inputFilePath.FullName, outPutFilePath.FullName);
			var convertOutput = string.Empty;

			var commandLineExecutor = new CommandLineExecutor();
			var result = commandLineExecutor.Execute(message.WorkingDirectoryPath, convertPath, convertArguments, out convertOutput);
			output += convertOutput;

			if (result && !string.IsNullOrEmpty(message.Settings.WatermarkPath))
			{
				var compositePath = message.AppSettings[ImageConversionConfiguration.Instance.CompositePathSettingName];
				var compositeArguments = WatermarkArguments(message.Settings, outPutFilePath.FullName);
				var compositeOutput = string.Empty;

				result = commandLineExecutor.Execute(message.WorkingDirectoryPath, compositePath, compositeArguments, out compositeOutput);

				output += Environment.NewLine + compositeOutput;
			}

			var executedMcAfeeWorkflowMessage = new ExecutedImageConversionWorkflowMessage()
			{
				CorrelationId = message.CorrelationId,
				EncodeSuccessful = result,
				OutPutFilePath = outPutFilePath.FullName,
				Output = output
			};

			var bus = BusDriver.Instance.GetBus(ImageConversionService.BusName);
			bus.Publish(executedMcAfeeWorkflowMessage);
		}

		private string WatermarkArguments(IImageResizeSettings settings, string outPutFilePath)
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

		private string ThumbnailArguments(IImageResizeSettings settings, string inputFilePath, string outPutFilePath)
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
