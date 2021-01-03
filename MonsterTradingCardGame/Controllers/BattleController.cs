using MonsterTradingCardGame.Entities.PlayerEntity;
using MonsterTradingCardGame.Entities.UserEntity;
using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Infrastructure.Authentication;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Modules;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using RestServer.WebServer.Infrastructure;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
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

        [HttpGet("Highscore")]
        public IActionResult GetHighScore()
        {
            HighScoreEntry[] highScore = playerEntity.GetHighScore()
                .ToArray();

            return Json(highScore, new JsonSerializerOptions() { WriteIndented = true });
        }

        [HttpGet("Battlelog")]
        public IActionResult GetBattleResult(MatchmakingEntry match)
        {
            Assert.NotNull(match, nameof(match));

            BattleLog result = duelModule.GetBattleLog(match.MatchID);
            return Json(result, new JsonSerializerOptions() { WriteIndented = true });
        }

        [HttpPost]
        public IActionResult InitiateAutomaticDuel(string deckName)
        {
            try
            {
                MatchmakingEntry match = matchmaker.FindMatch(requestContext, deckName);

                if (match != null && match.IsMatched && match.ShouldInitiate)
                    Task.Run(() => ExecuteDuel(match));

                return Json(match, new JsonSerializerOptions() { WriteIndented = true });
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

        private void ExecuteDuel(MatchmakingEntry match)
        {
            try
            {
                duelModule.ExecuteDuel(match, ProjectConstants.MaxNumberOfRounds);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private readonly PlayerEntity playerEntity = new PlayerEntity();
        private readonly AutomaticDuelMatchmaker matchmaker = new AutomaticDuelMatchmaker();
        private readonly AutomaticDuelModule duelModule = new AutomaticDuelModule();
        private readonly RequestContext requestContext;
    }
}
