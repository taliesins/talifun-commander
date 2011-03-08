using System.ServiceProcess;

namespace Talifun.Commander.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new CommanderService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
