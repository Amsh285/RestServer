using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using System;

namespace MonsterTradingCardGame.Controllers
{
    public sealed class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Status()
        {
            return Json(Environment.Version);
        }
    }
}
