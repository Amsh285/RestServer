using RestServer.WebServer;
using RestServer.WebServer.EndpointHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace RestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //127.0.0.1:13000

            List<Type> handlerTypes = ScanHandlerTypes()
                .ToList();

            HttpWebServer server = new HttpWebServer(IPAddress.Parse("127.0.0.1"), 13000, handlerTypes, new HardcodedEmptyServiceProvider());
            Task result = server.Start();
            result.Wait();
        }

        public static IEnumerable<Type> ScanHandlerTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(ControllerBase)));
        }
    }
}
