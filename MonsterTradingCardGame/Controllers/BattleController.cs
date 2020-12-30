using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Infrastructure.Authentication;
using MonsterTradingCardGame.Modules;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using RestServer.WebServer.Infrastructure;
using System;
using System.Threading.Tasks;

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
                MatchmakingEntry match = matchmaker.FindMatch(requestContext, deckName);

                if(match != null && match.IsMatched && match.ShouldInitiate)
                    Task.Run(() => duelModule.ExecuteDuel(match, ProjectConstants.MaxNumberOfRounds));

                return Json(match);
            }
            catch (SessionTokenNotFoundException nfEx)
            {
                return Unauthorized(nfEx.Message);
            }
            catch (Exception ex) when (ex is InvalidSessionTokenFormatException || ex is SessionExpiredException
                || ex is ValidationException)
            {
                return BadRequest(ex.Message);
            }
        }

        private readonly AutomaticDuelMatchmaker matchmaker = new AutomaticDuelMatchmaker();
        private readonly AutomaticDuelModule duelModule = new AutomaticDuelModule();
        private readonly RequestContext requestContext;
    }
}
