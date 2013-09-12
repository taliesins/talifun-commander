using Growl.Connector;

namespace Talifun.Commander.TestHarness
{
    public class GrowlHelper
    {
        public static void simpleGrowl(string title, string message = "")
        {
            var simpleGrowl = new GrowlConnector();
            var thisApp = new Growl.Connector.Application(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            var simpleGrowlType = new NotificationType("SIMPLEGROWL");
            simpleGrowl.Register(thisApp, new NotificationType[] { simpleGrowlType });
            var myGrowl = new Notification(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "SIMPLEGROWL", title, title, message);
            simpleGrowl.Notify(myGrowl);
        }
    }
}
