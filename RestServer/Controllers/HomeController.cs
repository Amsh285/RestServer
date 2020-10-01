using RestServer.CommunicationObjects;
using RestServer.EndpointHandling;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestServer.Controllers
{
    public sealed class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok("Hi, wie gehts?");
        }
    }
}
