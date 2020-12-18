using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Entities.CardEntity;
using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Models;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;

namespace MonsterTradingCardGame.Controllers
{
    public class CardController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateCard(Card value)
        {
            try
            {
                cardEntity.CreateCard(value);
                return Created("Card created.");
            }
            catch(ValidationException vex)
            {
                return BadRequest($"Ungültige Karte: {vex.Message}");
            }
            catch (UniqueConstraintViolationException ucvEx)
            {
                return Ok(ucvEx.Message);
            }
        }

        private static readonly CardEntity cardEntity = new CardEntity();
    }
}
