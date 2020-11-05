using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;

namespace MonsterTradingCardGame.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            return Ok();
        }
    }
}
