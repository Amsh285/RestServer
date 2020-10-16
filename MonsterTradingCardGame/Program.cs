using RestServer;
using RestServer.WebServer;
using RestServer.WebServer.EndpointHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace MasterTradingCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Type> handlerTypes = ScanHandlerTypes();
            HttpWebServer server = new HttpWebServer(IPAddress.Parse("127.0.0.1"), 13001, handlerTypes, new HardcodedEmptyServiceProvider());
            Task t = server.Start();
            t.Wait();
        }

        public static IEnumerable<Type> ScanHandlerTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(ControllerBase)));
        }
    }
}
