using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Configuration;
using Talifun.Commander.Command.ConfigurationChecker;
using Talifun.Commander.Command.Image.Properties;

namespace Talifun.Commander.Command.Image.Configuration
{
    public class ImageConversionTester : CommandConfigurationTesterBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return ImageConversionConfiguration.Instance;
            }
        }

        protected void TestImageResizeModeSetting(ProjectElement project, ImageConversionElement image)
        {
            switch (image.ResizeMode)
            {
                case ResizeMode.AreaToFit:
                case ResizeMode.CutToFit:
                case ResizeMode.Zoom:
                case ResizeMode.Stretch:
                    if (image.Width == 0)
                    {
                        throw new Exception(
                            string.Format(
								Resource.ErrorMessageWidthIsRequiredForResizeMode,
                                project.Name, 
								Settings.ElementCollectionSettingName,
								Settings.ElementSettingName,
								image.Name,
                                Enum.GetName(typeof(ResizeMode), image.ResizeMode)));
                    }
                    if (image.Height == 0)
                    {
                        throw new Exception(
                            string.Format(
								Resource.ErrorMessageHeightIsRequiredForResizeMode,
                                project.Name,
								Settings.ElementCollectionSettingName,
								Settings.ElementSettingName,
								image.Name,
                                Enum.GetName(typeof(ResizeMode), image.ResizeMode)));
                    }
                    break;
                case ResizeMode.FitWidth:
                case ResizeMode.FitMinimumWidth:
                case ResizeMode.FitMaximumWidth:
                    if (image.Width == 0)
                    {
                        throw new Exception(
                            string.Format(
								Resource.ErrorMessageWidthIsRequiredForResizeMode,
                                project.Name,
								Settings.ElementCollectionSettingName,
								Settings.ElementSettingName,
								image.Name,
                                Enum.GetName(typeof(ResizeMode), image.ResizeMode)));
                    }
                    break;
                case ResizeMode.FitHeight:
                case ResizeMode.FitMinimumHeight:
                case ResizeMode.FitMaximumHeight:
                    if (image.Height == 0)
                    {
                        throw new Exception(
                            string.Format(
								Resource.ErrorMessageHeightIsRequiredForResizeMode,
                                project.Name,
								Settings.ElementCollectionSettingName,
								Settings.ElementSettingName,
								image.Name,
                                Enum.GetName(typeof(ResizeMode), image.ResizeMode)));
                    }
                    break;
            }
        }

		public override void CheckProjectConfiguration(AppSettingsSection appSettings, ProjectElement project)
        {
            var commandSettings = new ProjectElementCommand<ImageConversionElementCollection>(Settings.ElementCollectionSettingName, project);
            var imageConversionSettings = commandSettings.Settings;

            var imageConversionSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (imageConversionSettingsKeys.Count <= 0)
            {
                return;
            }
			
            var convertPath = appSettings.Settings[ImageConversionConfiguration.Instance.ConvertPathSettingName].Value;

            if (string.IsNullOrEmpty(convertPath))
            {
				throw new Exception(string.Format(Command.Properties.Resource.ErrorMessageAppSettingRequired, ImageConversionConfiguration.Instance.ConvertPathSettingName));
            }

            for (var i = 0; i < imageConversionSettings.Count; i++)
            {
                var imageConversionSetting = imageConversionSettings[i];

            	var outPutPath = imageConversionSetting.GetOutPutPathOrDefault();
				if (!Directory.Exists(outPutPath))
                {
                	throw new Exception(
                		string.Format(Command.Properties.Resource.ErrorMessageCommandOutPutPathDoesNotExist,
                		              project.Name,
                		              Settings.ElementCollectionSettingName,
                		              Settings.ElementSettingName,
                		              imageConversionSetting.Name,
									  outPutPath));
                }
                else
                {
					(new DirectoryInfo(outPutPath)).TryCreateTestFile();
                }

            	var workingPath = imageConversionSetting.GetWorkingPathOrDefault();
				if (!string.IsNullOrEmpty(workingPath))
                {
					if (!Directory.Exists(workingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandWorkingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              imageConversionSetting.Name,
										 workingPath));
                    }
                    else
                    {
						(new DirectoryInfo(workingPath)).TryCreateTestFile();
                    }
                }
                else
                {
					(new DirectoryInfo(Path.GetTempPath())).TryCreateTestFile();
                }

            	var errorProcessingPath = imageConversionSetting.GetErrorProcessingPathOrDefault();
				if (!string.IsNullOrEmpty(errorProcessingPath))
                {
					if (!Directory.Exists(errorProcessingPath))
                    {
                    	throw new Exception(
                    		string.Format(Command.Properties.Resource.ErrorMessageCommandErrorProcessingPathDoesNotExist,
                    		              project.Name,
                    		              Settings.ElementCollectionSettingName,
                    		              Settings.ElementSettingName,
                    		              imageConversionSetting.Name,
										  errorProcessingPath));
                    }
                    else
                    {
						(new DirectoryInfo(errorProcessingPath)).TryCreateTestFile();
                    }
                }

            	var watermarkPath = imageConversionSetting.WatermarkPath;
				if (!string.IsNullOrEmpty(watermarkPath))
				{
					if (!File.Exists(watermarkPath))
					{
						throw new Exception(
							string.Format(Resource.ErrorMessageCommandWatermarkPathDoesNotExist,
										  project.Name,
										  Settings.ElementCollectionSettingName,
										  Settings.ElementSettingName,
										  imageConversionSetting.Name,
										  watermarkPath));
					}
				}

                TestImageResizeModeSetting(project, imageConversionSetting);

                imageConversionSettingsKeys.Remove(imageConversionSetting.Name);
            }

            if (imageConversionSettingsKeys.Count > 0)
            {
                FileMatchElement fileMatch = null;
                foreach (var value in imageConversionSettingsKeys.Values)
                {
                    fileMatch = value;
                    break;
                }

            	throw new Exception(
            		string.Format(
            			Command.Properties.Resource.ErrorMessageCommandConversionSettingKeyPointsToNonExistantCommand,
            			project.Name, fileMatch.Name, Settings.ElementSettingName, fileMatch.CommandSettingsKey));
            }
        }
    }
}
