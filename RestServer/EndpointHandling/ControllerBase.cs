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

        // Could be better solved with an IoC- Container (instantiating Controllers with a transient clientfunc)
        //but this would be too much for that little Project, so Privateinvoke it is.
        private TcpClient client;
    }
}
