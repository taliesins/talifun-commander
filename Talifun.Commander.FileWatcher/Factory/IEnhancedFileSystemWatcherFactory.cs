namespace Talifun.Commander.FileWatcher
{
    public interface IEnhancedFileSystemWatcherFactory
    {
        IEnhancedFileSystemWatcher CreateEnhancedFileSystemWatcher(string folderToWatch, string filter, int pollTime, bool includeSubdirectories);
        IEnhancedFileSystemWatcher CreateEnhancedFileSystemWatcher(string folderToWatch, string filter, int pollTime, bool includeSubdirectories, object userState);
    }
}
