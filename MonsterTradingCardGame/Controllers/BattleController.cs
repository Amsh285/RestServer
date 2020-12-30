using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Infrastructure.Authentication;
using MonsterTradingCardGame.Modules;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using RestServer.WebServer.Infrastructure;
using System;

namespace MonsterTradingCardGame.Controllers
{
    public sealed class BattleController : ControllerBase
    {
        public BattleController(RequestContext requestContext)
        {
            Assert.NotNull(requestContext, nameof(requestContext));

            this.requestContext = requestContext;
        }

        [HttpPost]
        public IActionResult InitiateAutomaticDuel(string deckName)
        {
            try
            {
                MatchmakingEntry match = duelModule.FindAdversary(requestContext);

                //Todo: GetDecks and start workerThread with autoduel

                return Json(match);
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

        private readonly AutomaticDuelMatchmaker duelModule = new AutomaticDuelMatchmaker();
        private readonly RequestContext requestContext;
    }
}
