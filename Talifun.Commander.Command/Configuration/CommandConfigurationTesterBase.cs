using System;
using System.IO;

namespace Talifun.Commander.Command.Configuration
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

                if (s != "Test") throw new Exception("Data read from test file (" + fileInfo.FullName + ") was incorrect");
            }

            fileInfo.Delete();
        }

        public abstract void CheckProjectConfiguration(Configuration.ProjectElement project);

        public abstract ISettingConfiguration Settings { get; }
    }
}