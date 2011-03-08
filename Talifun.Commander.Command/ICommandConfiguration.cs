namespace Talifun.Commander.Command
{
    public interface ICommandConfiguration
    {
        string Name { get; set; }
        string OutPutPath { get; set; }
        string WorkingPath { get; set; }
        string ErrorProcessingPath { get; set; }
        string FileNameFormat { get; set; }
    }
}
