using RestServer.CommunicationObjects;
using RestServer.EndpointHandling;

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
