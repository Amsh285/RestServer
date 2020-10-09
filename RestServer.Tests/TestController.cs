using RestServer.CommunicationObjects;
using RestServer.EndpointHandling;
using RestServer.EndpointHandling.Attributes;

namespace RestServer.Tests
{
    public sealed class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTests()
        {
            return Ok();
        }

        [HttpGet("SubResources")]
        public IActionResult GetTestsSubResources()
        {
            return Ok();
        }

        [HttpGet("{someID}")]
        public IActionResult GetTestWithID(string someID)
        {
            return Ok();
        }

        [HttpGet("{someID}/SubResource")]
        public IActionResult GetTestWithIDAndSingleSubResource(string someID)
        {
            return Ok();
        }

        [HttpGet("{someID}/SubResource/{someID}")]
        public IActionResult GetTestWithIDAndIdentifiedSubResource(string someID, string someSubResourceID)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult PostTest()
        {
            return Ok();
        }

        [HttpPost("{someID}")]
        public IActionResult PostTestWithId()
        {
            return Ok();
        }
    }
}
