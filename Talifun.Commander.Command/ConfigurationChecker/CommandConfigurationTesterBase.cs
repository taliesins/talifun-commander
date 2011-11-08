using System;
using System.Configuration;
using System.IO;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command.ConfigurationChecker
{
    public abstract class CommandConfigurationTesterBase : ICommandConfigurationTester
    {
        protected void TryCreateTestFile(DirectoryInfo directory)
        {
            var fileInfo = new FileInfo(Path.Combine(directory.FullName, "~test~.file"));
            if (fileInfo.Exists) fileInfo.Delete();

            using (var streamWriter = fileInfo.CreateText())
            {
                streamWriter.Write("Test");
            }

            using (var streamReader = fileInfo.OpenText())
            {
                var s = streamReader.ReadToEnd();

                if (s != "Test") throw new Exception(string.Format(Resource.ErrorMessageDataReadFromTestFileWasIncorrect, fileInfo.FullName));
            }

            fileInfo.Delete();
        }

        public abstract void CheckProjectConfiguration(AppSettingsSection appSettings, Configuration.ProjectElement project);

        public abstract ISettingConfiguration Settings { get; }
    }
}