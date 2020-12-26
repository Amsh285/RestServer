using MonsterTradingCardGame.Entities.ShopEntity;
using MonsterTradingCardGame.Infrastructure;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using RestServer.WebServer.Infrastructure;
using System;

namespace MonsterTradingCardGame.Controllers
{
    public sealed class ShopController : ControllerBase
    {
        public ShopController(RequestContext requestContext)
        {
            Assert.NotNull(requestContext, nameof(requestContext));

            this.requestContext = requestContext;
        }

        [HttpPost("Booster")]
        public IActionResult BuyBoosterPackage()
        {
            if (!requestContext.Cookies.Exists(ProjectConstants.AuthenticationTokenKey))
                return Unauthorized();

            if (Guid.TryParse(requestContext.Cookies[ProjectConstants.AuthenticationTokenKey], out Guid sessionToken))
            {
                shopEntity.BuyBoosterPackage(sessionToken);
                return Ok();
            }
            else
                return BadRequest($"Invalid AuthenticationToken- Format.");
        }

        private static readonly ShopEntity shopEntity = new ShopEntity();
        private readonly RequestContext requestContext;
    }
}
