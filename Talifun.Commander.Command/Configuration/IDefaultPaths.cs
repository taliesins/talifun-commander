namespace Talifun.Commander.Command.Configuration
{
	public interface IDefaultPaths
	{
		string FolderToWatch(CommandConfigurationBase commandConfiguration);
		string WorkingPath(CommandConfigurationBase commandConfiguration);
		string ErrorProcessingPath(CommandConfigurationBase commandConfiguration);
		string OutPutPath(CommandConfigurationBase commandConfiguration);
		string CompletedPath(CommandConfigurationBase commandConfiguration);
	}
}