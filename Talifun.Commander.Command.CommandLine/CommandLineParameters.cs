namespace Talifun.Commander.Command.CommandLine
{
    public class CommandLineParameters : ICommandLineParameters
    {
        public string CommandPath { get; set; }
        public string CommandArguments { get; set;}
    }
}