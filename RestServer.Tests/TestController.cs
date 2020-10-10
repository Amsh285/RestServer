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
        public IActionResult PostTests()
        {
            return Ok();
        }

        [HttpPost("SubResources")]
        public IActionResult PostTestsSubResources()
        {
            return Ok();
        }

        [HttpPost("{someID}")]
        public IActionResult PostTestWithID(string someID)
        {
            return Ok();
        }

        [HttpPost("{someID}/SubResource")]
        public IActionResult PostTestWithIDAndSingleSubResource(string someID)
        {
            return Ok();
        }

        [HttpPost("{someID}/SubResource/{someID}")]
        public IActionResult PostTestWithIDAndIdentifiedSubResource(string someID, string someSubResourceID)
        {
            return Ok();
        }

        [HttpPut]
        public IActionResult PutTests()
        {
            return Ok();
        }

        [HttpPut("SubResources")]
        public IActionResult PutTestsSubResources()
        {
            return Ok();
        }

        [HttpPut("{someID}")]
        public IActionResult PutTestWithID(string someID)
        {
            return Ok();
        }

        [HttpPut("{someID}/SubResource")]
        public IActionResult PutTestWithIDAndSingleSubResource(string someID)
        {
            return Ok();
        }

        [HttpPut("{someID}/SubResource/{someID}")]
        public IActionResult PutTestWithIDAndIdentifiedSubResource(string someID, string someSubResourceID)
        {
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteTests()
        {
            return Ok();
        }

        [HttpDelete("SubResources")]
        public IActionResult DeleteTestsSubResources()
        {
            return Ok();
        }

        [HttpDelete("{someID}")]
        public IActionResult DeleteTestWithID(string someID)
        {
            return Ok();
        }

        [HttpDelete("{someID}/SubResource")]
        public IActionResult DeleteTestWithIDAndSingleSubResource(string someID)
        {
            return Ok();
        }

        [HttpDelete("{someID}/SubResource/{someID}")]
        public IActionResult DeleteTestWithIDAndIdentifiedSubResource(string someID, string someSubResourceID)
        {
            return Ok();
        }
    }
}
