using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MasterTradingCardGame.Repositories;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using RestServer.WebServer.Infrastructure;

namespace MasterTradingCardGame.Controllers
{
    public sealed class RegisterController : ControllerBase
    {
        [HttpPost]
        public IActionResult Register(RegisterUser user)
        {
            Assert.NotNull(user, nameof(user));

            try
            {
                userRepository.Register(user);
                return Created();
            }
            catch (UniqueConstraintViolationException ucvEx)
            {
                return Ok(ucvEx.Message);
            }
        }

        private static readonly UserRepository userRepository = new UserRepository();
    }
}
