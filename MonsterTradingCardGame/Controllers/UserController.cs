using MonsterTradingCardGame.Entities.PlayerEntity;
using MonsterTradingCardGame.Infrastructure;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Controllers
{
    public sealed class UserController : ControllerBase
    {
        public UserController(RequestContext requestContext)
        {
            this.requestContext = requestContext;
        }

        [HttpPost("OpenFirstBoosterPackage")]
        public IActionResult OpenFirstBoosterPackage()
        {
            //Todo: Remove duplicate.
            if (!requestContext.Cookies.Exists(ProjectConstants.AuthenticationTokenKey))
                return Unauthorized();

            if (Guid.TryParse(requestContext.Cookies[ProjectConstants.AuthenticationTokenKey], out Guid sessionToken))
            {
                playerEntity.OpenFirstBoosterPackage(sessionToken);
                return Ok();
            }
            else
                return BadRequest($"Invalid AuthenticationToken- Format.");
        }

        private static readonly PlayerEntity playerEntity = new PlayerEntity();
        private readonly RequestContext requestContext;
    }
}
