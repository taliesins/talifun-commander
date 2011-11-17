using System;
using System.IO;
using System.Threading;
using Talifun.Commander.Command.Properties;

namespace Talifun.Commander.Command
{
	public static class FileExtensions
	{
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
