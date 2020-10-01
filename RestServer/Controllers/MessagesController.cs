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
            return null;
        }

        [HttpGet("{messageId}")]
        public IActionResult GetMessage(int messageId)
        {
            Message match = inMemoryMessages.FirstOrDefault(m => m.Id == messageId);

            if (match == null)
                return NotFound();

            return null;
        }

        [HttpPost]
        public IActionResult PostMessage(Message value)
        {
            value.CreationDate = DateTime.Now;
            inMemoryMessages.Add(value);

            return Ok();
        }

        // lokal speichern stateless undso
        private static List<Message> inMemoryMessages = new List<Message>(); 
    }
}
