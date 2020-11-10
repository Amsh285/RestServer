using MonsterTradingCardGame.Entities;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using System.Text;

namespace MonsterTradingCardGame.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (userEntity.Authenticate(username, Encoding.UTF8.GetBytes(password)))
                return Ok("Authentication Successful.");

            return Unauthorized("Authentication Failed.");
        }

        private static readonly UserEntity userEntity = new UserEntity();
    }
}
