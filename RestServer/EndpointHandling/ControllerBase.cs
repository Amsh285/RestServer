using RestServer.CommunicationObjects;
using System.Net.Sockets;

namespace RestServer.EndpointHandling
{
    public abstract class ControllerBase
    {
        protected IActionResult Ok(string message = null)
        {
            return HttpStatusCodeResult.Ok(client, message);
        }

        protected IActionResult BadRequest(string errorMessage = null)
        {
            return HttpStatusCodeResult.BadRequest(client, errorMessage);
        }

        protected IActionResult NotFound(string errorMessage = null)
        {
            return HttpStatusCodeResult.NotFound(client, errorMessage);
        }

        private TcpClient client;
    }
}
