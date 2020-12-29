using MonsterTradingCardGame.Entities.PlayerEntity;
using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Infrastructure.Authentication;
using MonsterTradingCardGame.Models;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using RestServer.WebServer.Infrastructure;
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
            try
            {
                playerEntity.OpenFirstBoosterPackage(requestContext);
                return Ok("Booster Package opened Successful.");
            }
            catch(NoBoosterPackageAssignedException noBoosterAssignedEx)
            {
                return BadRequest(noBoosterAssignedEx.Message);
            }
            catch (SessionTokenNotFoundException nfEx)
            {
                return Unauthorized(nfEx.Message);
            }
            catch (Exception ex) when (ex is InvalidSessionTokenFormatException || ex is SessionExpiredException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ShowCardLibrary")]
        public IActionResult ShowCardLibrary()
        {
            try
            {
                IEnumerable<CardLibraryItem> cardLibraryItems = playerEntity.GetCardLibrary(requestContext);
                return Json(cardLibraryItems);
            }
            catch (SessionTokenNotFoundException nfEx)
            {
                return Unauthorized(nfEx.Message);
            }
            catch (Exception ex) when (ex is InvalidSessionTokenFormatException || ex is SessionExpiredException)
            {
                return BadRequest(ex.Message);
            }
        }

        private static readonly PlayerEntity playerEntity = new PlayerEntity();
        private readonly RequestContext requestContext;
    }
}
