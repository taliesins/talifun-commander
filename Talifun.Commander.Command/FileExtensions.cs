using System;
using System.IO;
using System.Threading;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command
{
	public static class FileExtensions
	{
		public static DirectoryInfo GetWorkingDirectoryPath(this FileInfo inputFilePath, string conversionType, string workingPath, string uniqueProcessingNumber)
		{
			var uniqueDirectoryName = conversionType + "." + inputFilePath.Name + "." + uniqueProcessingNumber;

			DirectoryInfo workingDirectoryPath = null;
			if (!string.IsNullOrEmpty(workingPath))
			{
				workingDirectoryPath = new DirectoryInfo(Path.Combine(workingPath, uniqueDirectoryName));
			}
			else
			{
				workingDirectoryPath = new DirectoryInfo(Path.Combine(Path.GetTempPath(), uniqueDirectoryName));
			}

			return workingDirectoryPath;
		}

		public static void MoveCompletedFileToOutputFolder(this FileInfo workingFilePath, string fileNameFormat, string outPutPath)
		{
			var filename = workingFilePath.Name;

			if (!string.IsNullOrEmpty(fileNameFormat))
			{
				filename = string.Format(fileNameFormat, filename);
			}

			var outputFilePath = new FileInfo(Path.Combine(outPutPath, filename));
			if (outputFilePath.Exists)
			{
				outputFilePath.Delete();
			}

			workingFilePath.MoveTo(outputFilePath.FullName);
		}

		public static void Cleanup(this DirectoryInfo workingDirectoryPath)
		{
			if (workingDirectoryPath.Exists)
			{
				workingDirectoryPath.RetryDelete(5, true);
			}
		}

		public static void RetryDelete(this DirectoryInfo directory, int retry, bool recursively)
		{
			var delay = 0;

			for (var i = 0; i < retry; i++)
			{
				try
				{
					directory.Delete(recursively);
					return;
				}
				catch (DirectoryNotFoundException)
				{
					throw;
				}
				catch (IOException)
				{
					delay += 100;
					if (i == retry) throw;
				}

				Thread.Sleep(delay);
			}

			//We will never get here
			throw new IOException(string.Format(Resource.ErrorMessageUnableToDeleteDirectory, directory));
		}

		public static void TryCreateTestFile(this DirectoryInfo directory)
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

		public static bool WaitForFileToUnlock(this FileInfo fileInfo, int retry, int delay)
		{
			for (var i = 0; i < retry; i++)
			{
				if (!fileInfo.IsFileLocked())
				{
					return true;
				}
				Thread.Sleep(delay);
			}

			return false;
		}

		public static bool IsFileLocked(this FileInfo fileInfo)
		{
			var result = false;
			FileStream file = null;
			try
			{
				file = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
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
	}
}
