using System;

namespace RestServer.Models
{
    public sealed class Message
    {
        public int Id { get; set; }

        public string Sender { get; set; }

        public string Recipient { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
