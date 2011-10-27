using System;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Image.Configuration;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.Image
{
    public class ImageResizeCommand : ICommand<ImageResizeSettings>
    {
        #region ICommand<ResizeSettings> Members

		private string ThumbnailArguments(ImageResizeSettings settings, string inputFilePath, string outPutFilePath)
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

		public bool Run(ImageResizeSettings settings, AppSettingsSection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
        {
            var extension = "";
            switch (settings.ResizeImageType)
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
            outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, fileName));

            if (outPutFilePath.Exists)
            {
                outPutFilePath.Delete();
            }

			var commandArguments = ThumbnailArguments(settings, inputFilePath.FullName, outPutFilePath.FullName);
            var commandPath = appSettings.Settings[ImageConversionConfiguration.Instance.ConvertPathSettingName].Value;
            var workingDirectory = outputDirectoryPath.FullName;

            var commandLineExecutor = new CommandLineExecutor();
            return commandLineExecutor.Execute(workingDirectory, commandPath, commandArguments, out output);
        }

        #endregion
    }
}
