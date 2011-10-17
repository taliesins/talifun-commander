﻿using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Talifun.Commander.Command.AntiVirus.Configuration;
using Talifun.Commander.Executor.CommandLine;

namespace Talifun.Commander.Command.AntiVirus
{
    public class McAfeeCommand : ICommand<McAfeeSettings>
    {
        const string AllFixedOptions = @"/Quiet /AllOle /Archive /Packers /Mime /Primary Clean /Secondary Delete /LowPriority";

        #region ICommand<McAfeeCommand,McAfeeSettings> Members

		public bool Run(McAfeeSettings settings, AppSettingsSection appSettings, FileInfo inputFilePath, DirectoryInfo outputDirectoryPath, out FileInfo outPutFilePath, out string output)
        {
            outPutFilePath = new FileInfo(Path.Combine(outputDirectoryPath.FullName, inputFilePath.Name));
            if (outPutFilePath.Exists)
            {
                outPutFilePath.Delete();
            }

            inputFilePath.CopyTo(outPutFilePath.FullName);

            var commandPath = appSettings.Settings[AntiVirusConfiguration.Instance.McAfeePathSettingName].Value;
            var workingDirectory = outputDirectoryPath.FullName;
            var commandArguments = @"/target """ + outPutFilePath.FullName + @""" " + AllFixedOptions;

            var commandLineExecutor = new CommandLineExecutor();
            var filePassed = commandLineExecutor.Execute(workingDirectory, commandPath, commandArguments, out output);

            if (filePassed)
            {
                filePassed = outPutFilePath.Exists;
            }
            return filePassed;
        }

        #endregion

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
