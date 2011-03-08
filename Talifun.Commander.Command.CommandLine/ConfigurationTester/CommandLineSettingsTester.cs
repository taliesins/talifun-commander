using System;
using System.Collections.Generic;
using System.IO;
using Talifun.Commander.Command.CommandLine.Configuration;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.CommandLine.ConfigurationTester
{
    public class CommandLineSettingsTester : CommandConfigurationTesterBase
    {
        public override string ConversionType
        {
            get
            {
                return CommandLineSettingConfiguration.ConversionType;
            }
        }

        public override void CheckProjectConfiguration(ProjectElement project)
        {
            var commandSettings = new ProjectElementCommand<CommandLineSettingElementCollection>(CommandLineSettingConfiguration.CollectionSettingName, project);
            var commandLineSettings = commandSettings.Settings;

            var commandLineSettingsKeys = new Dictionary<string, FileMatchElement>();

            if (commandLineSettingsKeys.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < commandLineSettings.Count; i++)
            {
                var commandLineSetting = commandLineSettings[i];

                if (commandLineSetting.CheckCommandPathExists && !File.Exists(commandLineSetting.CommandPath))
                {
                    throw new Exception(
                        string.Format(
                            "<project name=\"{0}\"><commandLineSettings><commandLineSetting name=\"{1}\"> commandPath does not exist - {2}",
                            project.Name, commandLineSetting.Name,
                            commandLineSetting.CommandPath));
                }

                if (!Directory.Exists(commandLineSetting.OutPutPath))
                {
                    throw new Exception(
                        string.Format(
                            "<project name=\"{0}\"><commandLineSettings><commandLineSetting name=\"{1}\"> outPutPath does not exist - {2}",
                            project.Name, commandLineSetting.Name,
                            commandLineSetting.OutPutPath));
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(commandLineSetting.OutPutPath));
                }

                if (!string.IsNullOrEmpty(commandLineSetting.WorkingPath))
                {
                    if (!Directory.Exists(commandLineSetting.WorkingPath))
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><commandLineSettings><commandLineSetting name=\"{1}\"> workingPath does not exist - {2}",
                                project.Name, commandLineSetting.Name,
                                commandLineSetting.WorkingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(commandLineSetting.WorkingPath));
                    }
                }
                else
                {
                    TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));
                }

                if (!string.IsNullOrEmpty(commandLineSetting.ErrorProcessingPath))
                {
                    if (!Directory.Exists(commandLineSetting.ErrorProcessingPath))
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><commandLineSettings><commandLineSetting name=\"{1}\"> errorProcessingPath does not exist - {2}",
                                project.Name, commandLineSetting.Name,
                                commandLineSetting.ErrorProcessingPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(commandLineSetting.ErrorProcessingPath));
                    }
                }

                commandLineSettingsKeys.Remove(commandLineSetting.Name);
            }

            if (commandLineSettingsKeys.Count > 0)
            {
                FileMatchElement fileMatch = null;
                foreach (var value in commandLineSettingsKeys.Values)
                {
                    fileMatch = value;
                    break;
                }

                throw new Exception(
                    string.Format(
                        "<project name=\"{0}\"><folders><folder name=\"?\"><fileMatches><fileMatch name=\"{1}\"> conversionSettingsKey specified points to non-existant <commandLineSetting> - {2}",
                        project.Name, fileMatch.Name, fileMatch.CommandSettingsKey));
            }
        }
    }
}
