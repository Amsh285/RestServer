using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;

namespace RestServer.Controllers
{
    public sealed class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok("Hi, wie gehts?");
        }
    }
}
