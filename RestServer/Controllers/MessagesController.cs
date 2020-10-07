using RestServer.CommunicationObjects;
using RestServer.EndpointHandling;
using RestServer.EndpointHandling.Attributes;
using RestServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestServer.Controllers
{
    public class MessagesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetMessages()
        {
            return Ok();
        }

        [HttpGet("{messageId}")]
        public IActionResult GetMessage(int messageId)
        {
            Message match = inMemoryMessages.FirstOrDefault(m => m.Id == messageId);

            if (match == null)
                return NotFound();

            return Json(match);
        }

        [HttpPost]
        public IActionResult PostMessage(Message value)
        {
            ++currentId;

            value.Id = currentId;
            value.CreationDate = DateTime.Now;
            inMemoryMessages.Add(value);

            return Created();
        }

        private int currentId = 0;

        // lokal speichern stateless undso
        private static List<Message> inMemoryMessages = new List<Message>();
    }
}
