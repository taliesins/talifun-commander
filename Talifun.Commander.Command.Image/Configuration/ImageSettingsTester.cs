using System;
using System.Collections.Generic;
using System.IO;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.Image.Configuration
{
    public class ImageSettingsTester : CommandConfigurationTesterBase
    {
        public override ISettingConfiguration Settings
        {
            get
            {
                return ImageConversionSettingConfiguration.Instance;
            }
        }

        protected void TestImageResizeModeSetting(ProjectElement project, ImageConversionSettingElement imageSetting)
        {
            switch (imageSetting.ResizeMode)
            {
                case ResizeMode.AreaToFit:
                case ResizeMode.CutToFit:
                case ResizeMode.Zoom:
                case ResizeMode.Stretch:
                    if (!imageSetting.Width.HasValue)
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><imageConversionSettings><imageConversionSetting name=\"{1}\"> width is required when resize mode is - {2}",
                                project.Name, imageSetting.Name,
                                Enum.GetName(typeof(Commander.Command.Image.ResizeMode), imageSetting.ResizeMode)));
                    }
                    if (!imageSetting.Height.HasValue)
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><imageConversionSettings><imageConversionSetting name=\"{1}\"> height is required when resize mode is - {2}",
                                project.Name, imageSetting.Name,
                                Enum.GetName(typeof(Commander.Command.Image.ResizeMode), imageSetting.ResizeMode)));
                    }
                    break;
                case ResizeMode.FitWidth:
                case ResizeMode.FitMinimumWidth:
                case ResizeMode.FitMaximumWidth:
                    if (!imageSetting.Width.HasValue)
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><imageConversionSettings><imageConversionSetting name=\"{1}\"> width is required when resize mode is - {2}",
                                project.Name, imageSetting.Name,
                                Enum.GetName(typeof(Commander.Command.Image.ResizeMode), imageSetting.ResizeMode)));
                    }
                    break;
                case ResizeMode.FitHeight:
                case ResizeMode.FitMinimumHeight:
                case ResizeMode.FitMaximumHeight:
                    if (!imageSetting.Height.HasValue)
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><imageConversionSettings><imageConversionSetting name=\"{1}\"> height is required when resize mode is - {2}",
                                project.Name, imageSetting.Name,
                                Enum.GetName(typeof(ResizeMode), imageSetting.ResizeMode)));
                    }
                    break;
            }
        }

        public override void CheckProjectConfiguration(ProjectElement project)
        {
            var commandSettings = new ProjectElementCommand<ImageConversionSettingElementCollection>(Settings.CollectionSettingName, project);
            var imageConversionSettings = commandSettings.Settings;

            var imageConversionSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (imageConversionSettingsKeys.Count <= 0)
            {
                return;
            }

            var convertPath = ImageConversionSettingConfiguration.Instance.ConvertPath;

            if (string.IsNullOrEmpty(convertPath))
            {
                throw new Exception("ConvertPath appSetting Required");
            }

            for (var i = 0; i < imageConversionSettings.Count; i++)
            {
                var imageSetting = imageConversionSettings[i];

                if (!Directory.Exists(imageSetting.OutPutPath))
                {
                    throw new Exception(
                        string.Format(
                            "<project name=\"{0}\"><imageConversionSettings><imageConversionSetting name=\"{1}\"> outPutPath does not exist - {2}",
                            project.Name, imageSetting.Name, imageSetting.OutPutPath));
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(imageSetting.OutPutPath));
                }

                if (!string.IsNullOrEmpty(imageSetting.WorkingPath))
                {
                    if (!Directory.Exists(imageSetting.WorkingPath))
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><imageConversionSettings><imageConversionSetting name=\"{1}\"> workingPath does not exist - {2}",
                                project.Name, imageSetting.Name, imageSetting.WorkingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(imageSetting.WorkingPath));
                    }
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));
                }

                if (!string.IsNullOrEmpty(imageSetting.ErrorProcessingPath))
                {
                    if (!Directory.Exists(imageSetting.ErrorProcessingPath))
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><imageConversionSettings><imageConversionSetting name=\"{1}\"> errorProcessingPath does not exist - {2}",
                                project.Name, imageSetting.Name, imageSetting.ErrorProcessingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(imageSetting.ErrorProcessingPath));
                    }
                }

                TestImageResizeModeSetting(project, imageSetting);

                imageConversionSettingsKeys.Remove(imageSetting.Name);
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
                        "<project name=\"{0}\"><folders><folder name=\"?\"><fileMatches><fileMatch name=\"{1}\"> conversionSettingsKey specified points to non-existant <imageConversionSetting>- {2}",
                        project.Name, fileMatch.Name, fileMatch.CommandSettingsKey));
            }
        }
    }
}
