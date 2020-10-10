using RestServer.CommunicationObjects;
using System.Net.Sockets;
using System.Text.Json;

namespace RestServer.EndpointHandling
{
    public abstract class ControllerBase
    {
        protected IActionResult Ok(string message = null)
        {
            return HttpStatusCodeResult.Ok(client, message);
        }

        protected IActionResult Created(string message = null)
        {
            return HttpStatusCodeResult.Created(client, message);
        }

        protected IActionResult BadRequest(string errorMessage = null)
        {
            return HttpStatusCodeResult.BadRequest(client, errorMessage);
        }

        protected IActionResult NotFound(string errorMessage = null)
        {
            return HttpStatusCodeResult.NotFound(client, errorMessage);
        }

        protected IActionResult Json(object value)
        {
            return new JsonResult(value, client);
        }

        protected IActionResult Json(object value, JsonSerializerOptions serializerOptions)
        {
            return new JsonResult(value, client, serializerOptions);
        }

        // Could be better solved with an IoC- Container (instantiating Controllers with a transient clientfunc)
        //but this would be too much for that little Project, so Privateinvoke it is.
        private TcpClient client;
    }
}
