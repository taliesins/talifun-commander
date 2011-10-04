using System;
using System.Collections.Specialized;
using System.IO;
using Talifun.Commander.Command.Image.Configuration;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.Image
{
    public class ImageResizeCommand : ICommand<ImageResizeSettings>
    {
        #region ICommand<ResizeSettings> Members

        public bool Run(ImageResizeSettings settings, NameValueCollection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
        {
            var extension = "";
            var backgroundColour = settings.BackgroundColour;
            if (!string.IsNullOrEmpty(backgroundColour))
            {
				if (backgroundColour == "#00FFFFFF")
				{
					backgroundColour = "#00000000"; //This is imagemagiks transparent color
				}
                backgroundColour = "-background " + backgroundColour;
            }

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

            var quality = string.Empty;

            if (settings.Quality.HasValue)
            {
                quality = "-quality " + settings.Quality.Value + " ";
            }

            var commandArguments = string.Empty;
            switch (settings.ResizeMode)
            {
                case ResizeMode.AreaToFit:
                    {
                        var area = settings.Width*settings.Height;
                        commandArguments =
                            String.Format(
                                "-define jpeg:size=\"{0}x{1}\" \"{2}\" -thumbnail \"{3}@\" {4} -gravity {5} -extent \"{6}x{7}\" {8}\"{9}\"",
                                settings.Width*2, settings.Height*2, inputFilePath.FullName, area, backgroundColour,
                                Enum.GetName(typeof (Gravity), settings.Gravity), settings.Width, settings.Height, quality,
                                outPutFilePath);
                        break;
                    }
                case ResizeMode.CutToFit:
                    {
                        commandArguments =
                            String.Format("-define jpeg:size=\"{0}x{1}\" \"{2}\" -thumbnail \"{3}x{4}^\" -gravity {5} -extent \"{6}x{7}\" {8}\"{9}\"",
                                          settings.Width*2, settings.Height*2, inputFilePath.FullName, settings.Width, settings.Height,
                                          Enum.GetName(typeof(Gravity), settings.Gravity), settings.Width,
                                          settings.Height, quality, outPutFilePath);
                        break;
                    }
                case ResizeMode.Zoom:
                    {
                        commandArguments =
                            String.Format(
                                "-define jpeg:size=\"{0}x{1}\" \"{2}\" -thumbnail \"{3}x{4}>\" {5} -gravity {6} -extent \"{7}x{8}\" {9}\"{10}\"",
                                settings.Width * 2, settings.Height * 2, inputFilePath.FullName, settings.Width, settings.Height, backgroundColour,
                                Enum.GetName(typeof (Gravity), settings.Gravity), settings.Width, settings.Height, quality,
                                outPutFilePath);
                        break;
                    }
                case ResizeMode.Stretch:
                    {
                        commandArguments = String.Format("-define jpeg:size=\"{0}x{1}\" \"{2}\" -thumbnail \"{3}x{4}!\" {5}\"{6}\"",
                            settings.Width * 2, settings.Height * 2, inputFilePath.FullName, settings.Width, settings.Height, quality, outPutFilePath);
                        break;
                    }
                case ResizeMode.FitWidth:
                    {
                        commandArguments = String.Format("-define jpeg:size=\"{0}\" \"{1}\" -thumbnail \"{2}\" {3}\"{4}\"", 
                            settings.Width * 2, inputFilePath.FullName, settings.Width, quality, outPutFilePath);
                        break;
                    }
                case ResizeMode.FitMaximumWidth:
                    {
                        commandArguments = String.Format("-define jpeg:size=\"{0}>\" \"{1}\" -thumbnail \"{2}>\" {3}\"{4}\"",
                            settings.Width * 2, inputFilePath.FullName, settings.Width, quality, outPutFilePath);
                        break;
                    }
                case ResizeMode.FitMinimumWidth:
                    {
                        commandArguments = String.Format("-define jpeg:size=\"{0}\" \"{1}\" -thumbnail \"{2}^\" {3}\"{4}\"",
                            settings.Width * 2, inputFilePath.FullName, settings.Width, quality, outPutFilePath);
                        break;
                    }
                case ResizeMode.FitHeight:
                    {
                        commandArguments = String.Format("-define jpeg:size=\"x{0}\" \"{1}\" -thumbnail \"x{2}\" {3}\"{4}\"",
                            settings.Height * 2, inputFilePath.FullName, settings.Height, quality, outPutFilePath);
                        break;
                    }
                case ResizeMode.FitMaximumHeight:
                    {
                        commandArguments = String.Format("-define jpeg:size=\"x{0}\" \"{1}\" -thumbnail \"x{2}>\" {3}\"{4}\"",
                            settings.Height * 2, inputFilePath.FullName, settings.Height, quality, outPutFilePath);
                        break;
                    }
                case ResizeMode.FitMinimumHeight:
                    {
                        commandArguments = String.Format("-define jpeg:size=\"x{0}\" \"{1}\" -thumbnail \"x{2}^\" {3}\"{4}\"",
                            settings.Height * 2, inputFilePath.FullName, settings.Height, quality, outPutFilePath);
                        break;
                    }
                default:
                    {
                        commandArguments = String.Format("\"{0}\" \"{1}\"", inputFilePath.FullName, outPutFilePath);
                        break;
                    }
            }

            var commandPath = appSettings[ImageConversionConfiguration.Instance.ConvertPathSettingName];
            var workingDirectory = outputDirectoryPath.FullName;

            var commandLineExecutor = new CommandLineExecutor();
            return commandLineExecutor.Execute(workingDirectory, commandPath, commandArguments, out output);
        }

        #endregion
    }
}
