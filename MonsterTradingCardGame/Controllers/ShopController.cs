using MonsterTradingCardGame.Entities.ShopEntity;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;

namespace MonsterTradingCardGame.Controllers
{
    public sealed class ShopController : ControllerBase
    {
        [HttpPost("Booster")]
        public void BuyBoosterPackage()
        {

        }

        private static readonly ShopEntity shopEntity = new ShopEntity();
    }
}
