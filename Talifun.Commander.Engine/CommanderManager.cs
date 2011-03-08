using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Talifun.Commander.Command;
using Talifun.Commander.Configuration;
using Talifun.Commander.FileWatcher;

namespace Talifun.Commander.MediaConversion
{
    internal class CommanderManager : ICommanderManager, IDisposable
    {
        protected TimeSpan m_LockTimeout = TimeSpan.FromSeconds(10);
        protected AsyncOperation m_AsyncOperation = AsyncOperationManager.CreateOperation(null);

        protected List<IEnhancedFileSystemWatcher> m_EnhancedFileSystemWatchers = new List<IEnhancedFileSystemWatcher>();
        protected FileCreatedPreviouslyEventHandler m_FileCreatedPreviouslyEvent;
        protected FileFinishedChangingEventHandler m_FileFinishedChangingEvent;

        protected bool isRunning = false;
        protected bool stopSignalled = false;

        public CommanderManager()
        {
            CheckConfiguration();

            var projects = CurrentConfiguration.Current.Projects;
            for (var j = 0; j < projects.Count; j++)
            {
                var folderSettings = CurrentConfiguration.Current.Projects[j].Folders;

                for (var i = 0; i < folderSettings.Count; i++)
                {
                    var folderSetting = folderSettings[i];
                    var enhancedFileSystemWatcher =
                        EnhancedFileSystemWatcherFactory.Instance.GetEnhancedFileSystemWatcher(
                            folderSetting.FolderToWatch, folderSetting.Filter, folderSetting.PollTime,
                            folderSetting.IncludeSubdirectories, folderSetting);
                    m_EnhancedFileSystemWatchers.Add(enhancedFileSystemWatcher);
                }
            }

            m_FileCreatedPreviouslyEvent = new FileCreatedPreviouslyEventHandler(OnFileCreatedPreviouslyEvent);
            m_FileFinishedChangingEvent = new FileFinishedChangingEventHandler(OnFileFinishedChangingEvent);

            foreach (var enhancedFileSystemWatcher in m_EnhancedFileSystemWatchers)
            {
                enhancedFileSystemWatcher.FileCreatedPreviouslyEvent += m_FileCreatedPreviouslyEvent;
                enhancedFileSystemWatcher.FileFinishedChangingEvent += m_FileFinishedChangingEvent;
            }
        }

        protected void OnFileFinishedChangingEvent(object sender, FileFinishedChangingEventArgs e)
        {
            if (!(e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)) return;
            var folderSetting = (FolderElement)e.UserState;
            ProcessFileMatches(e.FilePath, folderSetting);
        }

        protected void OnFileCreatedPreviouslyEvent(object sender, FileCreatedPreviouslyEventArgs e)
        {
            var folderSetting = (FolderElement)e.UserState;
            ProcessFileMatches(e.FilePath, folderSetting);
        }

        protected void ProcessFileMatches(string filePath, FolderElement folderSetting)
        {
            WaitForFileToUnlock(filePath, 10, 500);
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return;
            }

            var fileName = fileInfo.Name;

            var regxOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline;
            var fileMatchSettings = folderSetting.FileMatches;

            var uniqueDirectoryName = "master." + fileName + "." + Guid.NewGuid();

            DirectoryInfo workingDirectoryPath = null;
            if (!string.IsNullOrEmpty(folderSetting.WorkingPath))
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(folderSetting.WorkingPath, uniqueDirectoryName));
            }
            else
            {
                workingDirectoryPath = new DirectoryInfo(Path.Combine(Path.GetTempPath(), uniqueDirectoryName));
            }

            var workingFilePath = new FileInfo(Path.Combine(workingDirectoryPath.FullName, fileName));
            try
            {
                workingDirectoryPath.Create();

                WaitForFileToUnlock(fileInfo.FullName, 10, 500);
                fileInfo.Refresh();
                fileInfo.MoveTo(workingFilePath.FullName);

                for (var i = 0; i < fileMatchSettings.Count; i++)
                {
                    var fileMatch = fileMatchSettings[i];

                    var fileNameMatched = true;
                    if (!string.IsNullOrEmpty(fileMatch.Expression))
                    {
                        fileNameMatched = Regex.IsMatch(fileName, fileMatch.Expression, regxOptions);
                    }

                    if (!fileNameMatched) continue;

                    ProcessFileMatch(workingFilePath, fileMatch);

                    //If the file no longer exists, it assumed that there should be no more processing
                    //e.g. anti-virus may delete file so, we will do no more processing
                    //e.g. video process was unable to process file so it was moved to error processing folder
                    if (!workingFilePath.Exists || fileMatch.StopProcessing)
                    {
                        break;
                    }

                    //Make sure that processing on file has stopped
                    WaitForFileToUnlock(workingFilePath.FullName, 10, 500);
                    workingFilePath.Refresh();
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(folderSetting.CompletedPath) && workingFilePath.Exists)
                {
                    var completedFilePath = new FileInfo(Path.Combine(folderSetting.CompletedPath, fileName));
                    if (completedFilePath.Exists)
                    {
                        completedFilePath.Delete();
                    }

                    //Make sure that processing on file has stopped
                    WaitForFileToUnlock(workingFilePath.FullName, 10, 500);
                    workingFilePath.Refresh();
                    workingFilePath.MoveTo(completedFilePath.FullName);
                }

                if (workingDirectoryPath.Exists)
                {
                    workingDirectoryPath.Delete(true);
                }
            }
        }

        protected static bool WaitForFileToUnlock(string filePath, int retry, int delay)
        {
            for (var i = 0; i < retry; i++)
            {
                if (!IsFileLocked(filePath))
                {
                    return true;
                }
                Thread.Sleep(delay);
            }

            return false;
        }

        protected static bool IsFileLocked(string filePath)
        {
            var result = false;
            FileStream file = null;
            try
            {
                file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                result = true;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            return result;
        }

        protected static ProjectElement GetCurrentProject(FileMatchElement fileMatch)
        {
            var projects = CurrentConfiguration.Current.Projects;

            for (var i = 0; i < projects.Count; i++)
            {
                var folders = projects[i].Folders;

                for (var j = 0; j < folders.Count; j++)
                {
                    var fileMatches = folders[j].FileMatches;

                    for (var k = 0; k < fileMatches.Count; k++)
                    {
                        if (fileMatches[k] == fileMatch) return projects[i];
                    }
                }
            }

            throw new Exception("Cannot find <project> for <fileMatch> element");
        }

        protected void ProcessFileMatch(FileInfo fileInfo, FileMatchElement fileMatch)
        {
            var project = GetCurrentProject(fileMatch);

            switch (fileMatch.ConversionType)
            {
                case ConversionType.Image:
                    {
                        var imageConversionSettings = project.ImageConversionSettings;
                        var imageConversionSettingsKey = fileMatch.ConversionSettingsKey;
                        var imageConversionSetting = imageConversionSettings[imageConversionSettingsKey];
                        if (imageConversionSetting == null)
                            throw new ConfigurationErrorsException("fileMatch attribute conversionSettingsKey='" +
                                                                   imageConversionSettingsKey +
                                                                   "' does not match any key found in imageConversionSettings name attributes");

                        var imageConverter = new ImageConverterRunner();
                        imageConverter.Run(this, fileInfo, imageConversionSetting);

                        break;
                    }
                case ConversionType.Video:
                    {
                        var videoConversionSettings = project.VideoConversionSettings;
                        var videoConversionSettingsKey = fileMatch.ConversionSettingsKey;
                        var videoConversionSetting = videoConversionSettings[videoConversionSettingsKey];
                        if (videoConversionSetting == null)
                            throw new ConfigurationErrorsException("fileMatch attribute conversionSettingsKey='" +
                                                                   videoConversionSettingsKey +
                                                                   "' does not match any key found in videoConversionSettings name attributes");

                        var videoConverter = new VideoConverterRunner();
                        videoConverter.Run(this, fileInfo, videoConversionSetting);
                        break;
                    }
                case ConversionType.Audio:
                    {
                        var audioConversionSettings = project.AudioConversionSettings;
                        var audioConversionSettingsKey = fileMatch.ConversionSettingsKey;
                        var audioConversionSetting = audioConversionSettings[audioConversionSettingsKey];
                        if (audioConversionSetting == null)
                            throw new ConfigurationErrorsException("fileMatch attribute conversionSettingsKey='" +
                                                                   audioConversionSettingsKey +
                                                                   "' does not match any key found in audioConversionSettings name attributes");

                        var audioConverter = new AudioConverterRunner();
                        audioConverter.Run(this, fileInfo, audioConversionSetting);
                        break;
                    }
                case ConversionType.AntiVirus:
                    {
                        var antiVirusSettings = project.AntiVirusSettings;
                        var antiVirusSettingsKey = fileMatch.ConversionSettingsKey;
                        var antiVirusSetting = antiVirusSettings[antiVirusSettingsKey];
                        if (antiVirusSetting == null)
                            throw new ConfigurationErrorsException("fileMatch attribute conversionSettingsKey='" +
                                                                   antiVirusSetting +
                                                                   "' does not match any key found in antiVirusSettings name attributes");

                        var antiVirus = new AntiVirusRunner();
                        antiVirus.Run(this, fileInfo, antiVirusSetting);

                        break;
                    }
                case ConversionType.VideoThumbnailer:
                    {
                        var videoThumbnailerSettings = project.VideoThumbnailerSettings;
                        var videoThumbnailerSettingsKey = fileMatch.ConversionSettingsKey;
                        var videoThumbnailerSetting = videoThumbnailerSettings[videoThumbnailerSettingsKey];

                        if (videoThumbnailerSetting == null)
                            throw new ConfigurationErrorsException("fileMatch attribute conversionSettingsKey='" +
                                                                   videoThumbnailerSetting +
                                                                   "' does not match any key found in videoThumbnailerSettings name attributes");

                        var videoThumbnailer = new VideoThumbnailerRunner();
                        videoThumbnailer.Run(this, fileInfo, videoThumbnailerSetting);

                        break;
                    }
                case ConversionType.CommandLine:
                    {
                        var commandLineSettings = project.CommandLineSettings;
                        var commandLineSettingsKey = fileMatch.ConversionSettingsKey;
                        var commandLineSetting = commandLineSettings[commandLineSettingsKey];

                        if (commandLineSetting == null)
                            throw new ConfigurationErrorsException("fileMatch attribute conversionSettingsKey='" +
                                                                   commandLineSetting +
                                                                   "' does not match any key found in commandLineSettings name attributes");

                        var commandLine = new CommandLineRunner();
                        commandLine.Run(this, fileInfo, commandLineSetting);
                        break;
                    }
            }
        }

        public void LogException(FileInfo errorFileInfo, Exception exception)
        {
            var exceptionMessage = GetExceptionMessage(exception);

            var commandErrorEventArgs = new CommandErrorEventArgs(exceptionMessage);

            RaiseAsynchronousOnCommandErrorEvent(commandErrorEventArgs);

            if (errorFileInfo == null) return;

            using (var streamWriter = errorFileInfo.CreateText())
            {
                streamWriter.Write(exceptionMessage);
            }
        }

        protected static string GetExceptionMessage(Exception exception)
        {
            var error = new StringBuilder();
            error.Append("Date:              " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);
            error.Append("Computer name:     " + Environment.MachineName + Environment.NewLine);
            error.Append("User name:         " + Environment.UserName + Environment.NewLine);
            error.Append("OS:                " + Environment.OSVersion.ToString() + Environment.NewLine);
            error.Append("Culture:           " + CultureInfo.CurrentCulture.Name + Environment.NewLine);

            error.Append("Exception class:   " +
            exception.GetType().ToString() + Environment.NewLine);
            error.Append("Exception message: " + GetExceptionStack(exception) + Environment.NewLine);
            error.Append(Environment.NewLine);
            error.Append("Stack Trace:");
            error.Append(Environment.NewLine);
            error.Append(exception.StackTrace);
            error.Append(Environment.NewLine);
            error.Append(Environment.NewLine);
            error.Append("Loaded Modules:");
            error.Append(Environment.NewLine);
            var thisProcess = Process.GetCurrentProcess();
            foreach (ProcessModule module in thisProcess.Modules)
            {
                error.Append(module.FileName + " " + module.FileVersionInfo.FileVersion);
                error.Append(Environment.NewLine);
            }
            error.Append(Environment.NewLine);
            error.Append(Environment.NewLine);
            error.Append(Environment.NewLine);

            return error.ToString();
        }

        protected static string GetExceptionStack(Exception e)
        {
            var message = new StringBuilder();
            message.Append(e.Message);
            while (e.InnerException != null)
            {
                e = e.InnerException;
                message.Append(Environment.NewLine);
                message.Append(e.Message);
            }

            return message.ToString();
        }

        #region Test Configuration
        public void CheckConfiguration()
        {
            var projects = CurrentConfiguration.Current.Projects;
            for (var j = 0; j < projects.Count; j++)
            {
                CheckProjectConfiguration(projects[j]);
            }
        }

        protected void CheckProjectConfiguration(ProjectElement project)
        {
            //We only want to check the sections if they are used, otherwise it will complain about
            //sections missing even if we aren't using them.
            var imageConversionSettingsKeys = new Dictionary<string, FileMatchElement>();
            var audioConversionSettingsKeys = new Dictionary<string, FileMatchElement>();
            var videoConversionSettingsKeys = new Dictionary<string, FileMatchElement>();
            var antiVirusSettingsKeys = new Dictionary<string, FileMatchElement>();
            var videoThumbnailerSettingsKeys = new Dictionary<string, FileMatchElement>();
            var commandLineSettingsKeys = new Dictionary<string, FileMatchElement>();

            var foldersToWatch = new List<string>();
            //This will check that all the required folders exists
            //It will also check that the service has the correct permissions to create, edit and delete files
            var folderSettings = project.Folders;
            for (var i = 0; i < folderSettings.Count; i++)
            {
                var folderSetting = folderSettings[i];

                //Check that folder to watch exists
                if (!Directory.Exists(folderSetting.FolderToWatch)) throw new Exception(string.Format("<project name=\"{0}\"><folders><folder name=\"{1}\"> folderToWatch does not exist - {2}", project.Name, folderSetting.Name, folderSetting.FolderToWatch));

                //Check that there are no duplicate folderToWatch
                if (foldersToWatch.Contains(folderSetting.FolderToWatch)) throw new Exception(string.Format("<project name=\"{0}\"><folders><folders name=\"{1}\"> folderToWatch is a duplicate, only one watcher per folder branch - {2}", project.Name, folderSetting.Name, folderSetting.FolderToWatch));
                foldersToWatch.Add(folderSetting.FolderToWatch);

                //Check that working path is valid
                if (!string.IsNullOrEmpty(folderSetting.WorkingPath))
                {
                    if (!Directory.Exists(folderSetting.WorkingPath)) throw new Exception(string.Format("<project name=\"{0}\"><folders><folders name=\"{1}\"> workingPath does not exist - {2}", project.Name, folderSetting.Name, folderSetting.WorkingPath));
                    else TryCreateTestFile(new DirectoryInfo(folderSetting.WorkingPath));
                }
                else TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));

                //Check completed path is valid
                if (!string.IsNullOrEmpty(folderSetting.CompletedPath))
                {
                    if (!Directory.Exists(folderSetting.CompletedPath)) throw new Exception(string.Format("<project name=\"{0}\"><folders><folders name=\"{1}\"> completedPath does not exist - {2}", project.Name, folderSetting.Name, folderSetting.CompletedPath));
                    else TryCreateTestFile(new DirectoryInfo(folderSetting.CompletedPath));
                }

                var fileMatches = folderSetting.FileMatches;

                for (var j = 0; j < fileMatches.Count; j++)
                {
                    var fileMatch = fileMatches[j];

                    switch (fileMatch.ConversionType)
                    {
                        case ConversionType.Image:
                            if (!imageConversionSettingsKeys.ContainsKey(fileMatch.ConversionSettingsKey))
                            {
                                imageConversionSettingsKeys.Add(fileMatch.ConversionSettingsKey, fileMatch);
                            }
                            break;
                        case ConversionType.Audio:
                            if (!audioConversionSettingsKeys.ContainsKey(fileMatch.ConversionSettingsKey))
                            {
                                audioConversionSettingsKeys.Add(fileMatch.ConversionSettingsKey, fileMatch);
                            }
                            break;
                        case ConversionType.Video:
                            if (!videoConversionSettingsKeys.ContainsKey(fileMatch.ConversionSettingsKey))
                            {
                                videoConversionSettingsKeys.Add(fileMatch.ConversionSettingsKey, fileMatch);
                            }
                            break;
                        case ConversionType.AntiVirus:
                            if (!antiVirusSettingsKeys.ContainsKey(fileMatch.ConversionSettingsKey))
                            {
                                antiVirusSettingsKeys.Add(fileMatch.ConversionSettingsKey, fileMatch);
                            }
                            break;
                        case ConversionType.VideoThumbnailer:
                            if (!videoThumbnailerSettingsKeys.ContainsKey(fileMatch.ConversionSettingsKey))
                            {
                                videoThumbnailerSettingsKeys.Add(fileMatch.ConversionSettingsKey, fileMatch);
                            }
                            break;
                        case ConversionType.CommandLine:
                            if (!videoThumbnailerSettingsKeys.ContainsKey(fileMatch.ConversionSettingsKey))
                            {
                                commandLineSettingsKeys.Add(fileMatch.ConversionSettingsKey, fileMatch);
                            }
                            break;
                    }
                }
            }

            if (imageConversionSettingsKeys.Count > 0)
            {
                var ConvertPath = SettingsHelper.ConvertPath;

                if (string.IsNullOrEmpty(ConvertPath))
                {
                    throw new Exception("ConvertPath appSetting Required");
                }

                var imageConversionSettings = project.ImageConversionSettings;
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
                            project.Name, fileMatch.Name, fileMatch.ConversionSettingsKey));
                }
            }

            if (audioConversionSettingsKeys.Count > 0)
            {
                var FFMpegPath = SettingsHelper.FFMpegPath;

                if (string.IsNullOrEmpty(FFMpegPath))
                {
                    throw new Exception("FFMpegPath appSetting Required");
                }

                var audioConversionSettings = project.AudioConversionSettings;
                for (var i = 0; i < audioConversionSettings.Count; i++)
                {
                    var audioSetting = audioConversionSettings[i];

                    if (!Directory.Exists(audioSetting.OutPutPath))
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><audioConversionSettings><audioConversionSetting name=\"{1}\"> outPutPath does not exist - {2}",
                                project.Name, audioSetting.Name, audioSetting.OutPutPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(audioSetting.OutPutPath));
                    }

                    if (!string.IsNullOrEmpty(audioSetting.WorkingPath))
                    {
                        if (!Directory.Exists(audioSetting.WorkingPath))
                        {
                            throw new Exception(
                                string.Format(
                                    "<project name=\"{0}\"><audioConversionSettings><audioConversionSetting name=\"{1}\"> workingPath does not exist - {2}",
                                    project.Name, audioSetting.Name, audioSetting.WorkingPath));
                        }
                        else
                        {
                            TryCreateTestFile(new DirectoryInfo(audioSetting.WorkingPath));
                        }
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));
                    }

                    if (!string.IsNullOrEmpty(audioSetting.ErrorProcessingPath))
                    {
                        if (!Directory.Exists(audioSetting.ErrorProcessingPath))
                        {
                            throw new Exception(
                                string.Format(
                                    "<project name=\"{0}\"><audioConversionSettings><audioConversionSetting name=\"{1}\"> errorProcessingPath does not exist - {2}",
                                    project.Name, audioSetting.Name, audioSetting.ErrorProcessingPath));
                        }
                        else
                        {
                            TryCreateTestFile(new DirectoryInfo(audioSetting.ErrorProcessingPath));
                        }
                    }

                    audioConversionSettingsKeys.Remove(audioSetting.Name);
                }

                if (audioConversionSettingsKeys.Count > 0)
                {
                    FileMatchElement fileMatch = null;
                    foreach (var value in audioConversionSettingsKeys.Values)
                    {
                        fileMatch = value;
                        break;
                    }

                    throw new Exception(
                        string.Format(
                            "<project name=\"{0}\"><folders><folder name=\"?\"><fileMatches><fileMatch name=\"{1}\"> conversionSettingsKey specified points to non-existant <audioConversionSetting> - {2}",
                            project.Name, fileMatch.Name, fileMatch.ConversionSettingsKey));
                }
            }

            if (videoConversionSettingsKeys.Count > 0)
            {
                var FFMpegPath = SettingsHelper.FFMpegPath;

                if (string.IsNullOrEmpty(FFMpegPath))
                {
                    throw new Exception("FFMpegPath appSetting Required");
                }

                var FlvTool2Path = SettingsHelper.FlvTool2Path;

                if (string.IsNullOrEmpty(FlvTool2Path))
                {
                    throw new Exception("FlvTool2Path appSetting Required");
                }

                var videoConversionSettings = project.VideoConversionSettings;
                for (var i = 0; i < videoConversionSettings.Count; i++)
                {
                    var videoSetting = videoConversionSettings[i];

                    if (!Directory.Exists(videoSetting.OutPutPath))
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><videoConversionSettings><videoConversionSetting name=\"{1}\"> outPutPath does not exist - {2}",
                                project.Name, videoSetting.Name, videoSetting.OutPutPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(videoSetting.OutPutPath));
                    }

                    if (!string.IsNullOrEmpty(videoSetting.WorkingPath))
                    {
                        if (!Directory.Exists(videoSetting.WorkingPath))
                        {
                            throw new Exception(
                                string.Format(
                                    "<project name=\"{0}\"><videoConversionSettings><videoConversionSetting name=\"{1}\"> workingPath does not exist - {2}",
                                    project.Name, videoSetting.Name, videoSetting.WorkingPath));
                        }
                        else
                        {
                            TryCreateTestFile(new DirectoryInfo(videoSetting.WorkingPath));
                        }
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));
                    }

                    if (!string.IsNullOrEmpty(videoSetting.ErrorProcessingPath))
                    {
                        if (!Directory.Exists(videoSetting.ErrorProcessingPath))
                        {
                            throw new Exception(
                                string.Format(
                                    "<project name=\"{0}\"><videoConversionSettings><videoConversionSetting name=\"{1}\"> errorProcessingPath does not exist - {2}",
                                    project.Name, videoSetting.Name, videoSetting.ErrorProcessingPath));
                        }
                        else
                        {
                            TryCreateTestFile(new DirectoryInfo(videoSetting.ErrorProcessingPath));
                        }
                    }

                    videoConversionSettingsKeys.Remove(videoSetting.Name);
                }

                if (videoConversionSettingsKeys.Count > 0)
                {
                    FileMatchElement fileMatch = null;
                    foreach (var value in videoConversionSettingsKeys.Values)
                    {
                        fileMatch = value;
                        break;
                    }

                    throw new Exception(
                        string.Format(
                            "<project name=\"{0}\"><folders><folder name=\"?\"><fileMatches><fileMatch name=\"{1}\"> conversionSettingsKey specified points to non-existant <videoConversionSetting> - {2}",
                            project.Name, fileMatch.Name, fileMatch.ConversionSettingsKey));
                }
            }

            if (antiVirusSettingsKeys.Count > 0)
            {
                var antiVirusSettings = project.AntiVirusSettings;
                for (var i = 0; i < antiVirusSettings.Count; i++)
                {
                    var antiVirusSetting = antiVirusSettings[i];

                    if (!string.IsNullOrEmpty(antiVirusSetting.ErrorProcessingPath))
                    {
                        if (!Directory.Exists(antiVirusSetting.ErrorProcessingPath))
                        {
                            throw new Exception(
                                string.Format(
                                    "<project name=\"{0}\"><antiVirusSettings><antiVirusSetting name=\"{1}\"> errorProcessingPath does not exist - {2}",
                                    project.Name, antiVirusSetting.Name, antiVirusSetting.ErrorProcessingPath));
                        }
                        else
                        {
                            TryCreateTestFile(new DirectoryInfo(antiVirusSetting.ErrorProcessingPath));
                        }
                    }

                    if (antiVirusSetting.VirusScannerType == Commander.Command.AntiVirus.VirusScannerType.NotSpecified
                        || antiVirusSetting.VirusScannerType == Commander.Command.AntiVirus.VirusScannerType.McAfee)
                    {
                        var virusScannerPath = SettingsHelper.McAfeePath;

                        if (string.IsNullOrEmpty(virusScannerPath))
                        {
                            throw new Exception("McAfeePath appSetting Required");
                        }
                    }

                    antiVirusSettingsKeys.Remove(antiVirusSetting.Name);
                }

                if (antiVirusSettingsKeys.Count > 0)
                {
                    FileMatchElement fileMatch = null;
                    foreach (var value in antiVirusSettingsKeys.Values)
                    {
                        fileMatch = value;
                        break;
                    }

                    throw new Exception(
                        string.Format(
                            "<project name=\"{0}\"><folders><folder name=\"?\"><fileMatches><fileMatch name=\"{1}\"> conversionSettingsKey specified points to non-existant <antiVirusSetting> - {2}",
                            project.Name, fileMatch.Name, fileMatch.ConversionSettingsKey));
                }
            }

            if (videoThumbnailerSettingsKeys.Count > 0)
            {
                var FFMpegPath = SettingsHelper.FFMpegPath;

                if (string.IsNullOrEmpty(FFMpegPath))
                {
                    throw new Exception("FFMpegPath appSetting Required");
                }

                var videoThumbnailerSettings = project.VideoThumbnailerSettings;
                for (var i = 0; i < videoThumbnailerSettings.Count; i++)
                {
                    var videoThumbnailerSetting = videoThumbnailerSettings[i];

                    if (!Directory.Exists(videoThumbnailerSetting.OutPutPath))
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><videoThumbnailerSettings><videoThumbnailerSetting name=\"{1}\"> outPutPath does not exist - {2}",
                                project.Name, videoThumbnailerSetting.Name, videoThumbnailerSetting.OutPutPath));
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(videoThumbnailerSetting.OutPutPath));
                    }

                    if (!string.IsNullOrEmpty(videoThumbnailerSetting.WorkingPath))
                    {
                        if (!Directory.Exists(videoThumbnailerSetting.WorkingPath))
                        {
                            throw new Exception(
                                string.Format(
                                    "<project name=\"{0}\"><videoThumbnailerSettings><videoThumbnailerSetting name=\"{1}\"> workingPath does not exist - {2}",
                                    project.Name, videoThumbnailerSetting.Name, videoThumbnailerSetting.WorkingPath));
                        }
                        else
                        {
                            TryCreateTestFile(new DirectoryInfo(videoThumbnailerSetting.WorkingPath));
                        }
                    }
                    else
                    {
                        TryCreateTestFile(new DirectoryInfo(Path.GetTempPath()));
                    }

                    if (!string.IsNullOrEmpty(videoThumbnailerSetting.ErrorProcessingPath))
                    {
                        if (!Directory.Exists(videoThumbnailerSetting.ErrorProcessingPath))
                        {
                            throw new Exception(
                                string.Format(
                                    "<project name=\"{0}\"><videoThumbnailerSettings><videoThumbnailerSetting name=\"{1}\"> errorProcessingPath does not exist - {2}",
                                    project.Name, videoThumbnailerSetting.Name,
                                    videoThumbnailerSetting.ErrorProcessingPath));
                        }
                        else
                        {
                            TryCreateTestFile(new DirectoryInfo(videoThumbnailerSetting.ErrorProcessingPath));
                        }
                    }

                    videoThumbnailerSettingsKeys.Remove(videoThumbnailerSetting.Name);
                }

                if (videoThumbnailerSettingsKeys.Count > 0)
                {
                    FileMatchElement fileMatch = null;
                    foreach (var value in videoThumbnailerSettingsKeys.Values)
                    {
                        fileMatch = value;
                        break;
                    }

                    throw new Exception(
                        string.Format(
                            "<project name=\"{0}\"><folders><folder name=\"?\"><fileMatches><fileMatch name=\"{1}\"> conversionSettingsKey specified points to non-existant <videoThumbnailerSetting> - {2}",
                            project.Name, fileMatch.Name, fileMatch.ConversionSettingsKey));
                }
            }

            if (commandLineSettingsKeys.Count > 0)
            {
                var commandLineSettings = project.CommandLineSettings;
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
                            project.Name, fileMatch.Name, fileMatch.ConversionSettingsKey));
                }
            }
        }

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


        protected void TestImageResizeModeSetting(ProjectElement project, ImageConversionSettingElement imageSetting)
        {
            switch (imageSetting.ResizeMode)
            {
                case Commander.Command.Image.ResizeMode.AreaToFit:
                case Commander.Command.Image.ResizeMode.CutToFit:
                case Commander.Command.Image.ResizeMode.Zoom:
                case Commander.Command.Image.ResizeMode.Stretch:
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
                case Commander.Command.Image.ResizeMode.FitWidth:
                case Commander.Command.Image.ResizeMode.FitMinimumWidth:
                case Commander.Command.Image.ResizeMode.FitMaximumWidth:
                    if (!imageSetting.Width.HasValue)
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><imageConversionSettings><imageConversionSetting name=\"{1}\"> width is required when resize mode is - {2}",
                                project.Name, imageSetting.Name,
                                Enum.GetName(typeof(Commander.Command.Image.ResizeMode), imageSetting.ResizeMode)));
                    }
                    break;
                case Commander.Command.Image.ResizeMode.FitHeight:
                case Commander.Command.Image.ResizeMode.FitMinimumHeight:
                case Commander.Command.Image.ResizeMode.FitMaximumHeight:
                    if (!imageSetting.Height.HasValue)
                    {
                        throw new Exception(
                            string.Format(
                                "<project name=\"{0}\"><imageConversionSettings><imageConversionSetting name=\"{1}\"> height is required when resize mode is - {2}",
                                project.Name, imageSetting.Name,
                                Enum.GetName(typeof(Commander.Command.Image.ResizeMode), imageSetting.ResizeMode)));
                    }
                    break;
            }
        }
        #endregion

        #region ICommanderManager Members

        public void Start()
        {
            if (!isRunning && !stopSignalled)
            {
                isRunning = true;
                StartEnhancedFileSystemWatchers();
            }
        }

        public void Stop()
        {
            if (isRunning && !stopSignalled)
            {
                stopSignalled = true;
                StopEnhancedFileSystemWatchers();
                isRunning = false;
                stopSignalled = false;
            }
        }

        public bool IsRunning
        {
            get { return isRunning; }
        }

        private void StartEnhancedFileSystemWatchers()
        {
            foreach (var enhancedFileSystemWatcher in m_EnhancedFileSystemWatchers)
            {
                enhancedFileSystemWatcher.Start();
            }
        }

        private void StopEnhancedFileSystemWatchers()
        {
            foreach (var enhancedFileSystemWatcher in m_EnhancedFileSystemWatchers)
            {
                enhancedFileSystemWatcher.Stop();
            }
        }

        #endregion

        #region CommandErrorEvent
        /// <summary>
        /// Where the actual event is stored.
        /// </summary>
        private CommandErrorEventHandler m_CommandErrorEvent;

        /// <summary>
        /// Lock for event delegate access.
        /// </summary>
        private readonly object m_CommandErrorEventLock = new object();

        /// <summary>
        /// The event that is fired.
        /// </summary>
        public event CommandErrorEventHandler CommandErrorEvent
        {
            add
            {
                if (!Monitor.TryEnter(m_CommandErrorEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - CommandErrorEvent.add");
                }
                try
                {
                    m_CommandErrorEvent += value;
                }
                finally
                {
                    Monitor.Exit(m_CommandErrorEventLock);
                }
            }
            remove
            {
                if (!Monitor.TryEnter(m_CommandErrorEventLock, m_LockTimeout))
                {
                    throw new ApplicationException("Timeout waiting for lock - CommandErrorEvent.remove");
                }
                try
                {
                    m_CommandErrorEvent -= value;
                }
                finally
                {
                    Monitor.Exit(m_CommandErrorEventLock);
                }
            }
        }

        /// <summary>
        /// Template method to add default behaviour for the event
        /// </summary>
        private void OnCommandErrorEvent(CommandErrorEventArgs e)
        {
            // TODO: Implement default behaviour of OnCommandErrorEvent
        }

        private void AsynchronousOnCommandErrorEventRaised(object state)
        {
            CommandErrorEventArgs e = state as CommandErrorEventArgs;
            RaiseOnCommandErrorEvent(e);
        }

        /// <summary>
        /// Will raise the event on the calling thread synchronously. 
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseCrossThreadOnCommandErrorEvent(CommandErrorEventArgs e)
        {
            m_AsyncOperation.SynchronizationContext.Send(new SendOrPostCallback(AsynchronousOnCommandErrorEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the calling thread asynchronously. 
        /// i.e. it will immediatly continue processing even though event 
        /// handlers have not processed the event yet.
        /// </summary>
        /// <param name="state">The state to be passed to the event.</param>
        private void RaiseAsynchronousOnCommandErrorEvent(CommandErrorEventArgs e)
        {
            m_AsyncOperation.Post(new SendOrPostCallback(AsynchronousOnCommandErrorEventRaised), e);
        }

        /// <summary>
        /// Will raise the event on the current thread synchronously.
        /// i.e. it will wait until all event handlers have processed the event.
        /// </summary>
        /// <param name="e">The state to be passed to the event.</param>
        private void RaiseOnCommandErrorEvent(CommandErrorEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.

            CommandErrorEventHandler eventHandler;

            if (!Monitor.TryEnter(m_CommandErrorEventLock, m_LockTimeout))
            {
                throw new ApplicationException("Timeout waiting for lock - RaiseOnCommandErrorEvent");
            }
            try
            {
                eventHandler = m_CommandErrorEvent;
            }
            finally
            {
                Monitor.Exit(m_CommandErrorEventLock);
            }

            OnCommandErrorEvent(e);

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
        #endregion

        #region IDisposable Members
        private int m_AlreadyDisposed = 0;

        ~CommanderManager()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_AlreadyDisposed == 0)
            {
                // dispose of the managed and unmanaged resources
                Dispose(true);

                // tell the GC that the Finalize process no longer needs
                // to be run for this object.		
                GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                var disposedAlready = Interlocked.Exchange(ref m_AlreadyDisposed, 1);
                if (disposedAlready != 0)
                {
                    return;
                }

                // Dispose managed resources.

                foreach (IEnhancedFileSystemWatcher enhancedFileSystemWatcher in m_EnhancedFileSystemWatchers)
                {
                    enhancedFileSystemWatcher.FileCreatedPreviouslyEvent -= m_FileCreatedPreviouslyEvent;
                    enhancedFileSystemWatcher.FileFinishedChangingEvent -= m_FileFinishedChangingEvent;
                }

                foreach (IEnhancedFileSystemWatcher enhancedFileSystemWatcher in m_EnhancedFileSystemWatchers)
                {
                    enhancedFileSystemWatcher.Dispose();
                }

                m_FileCreatedPreviouslyEvent = null;
                m_FileFinishedChangingEvent = null;

                m_CommandErrorEvent = null;
            }
            // Dispose unmanaged resources.

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
        }
        #endregion
    }
}
