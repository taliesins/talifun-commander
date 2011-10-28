using System.IO;
using System.Threading;

namespace Talifun.Commander.Command
{
	public static class FileExtensions
	{
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
