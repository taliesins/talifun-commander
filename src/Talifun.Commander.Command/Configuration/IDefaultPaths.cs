namespace Talifun.Commander.Command.Configuration
{
	public interface IDefaultPaths
	{
		string FolderToWatch(NamedConfigurationElement namedConfigurationElement);
		string WorkingPath(NamedConfigurationElement namedConfigurationElement);
		string ErrorProcessingPath(NamedConfigurationElement namedConfigurationElement);
		string OutPutPath(NamedConfigurationElement namedConfigurationElement);
		string CompletedPath(NamedConfigurationElement namedConfigurationElement);
	}
}