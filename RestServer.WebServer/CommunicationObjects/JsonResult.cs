using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace RestServer.WebServer.CommunicationObjects
{
    public sealed class JsonResult : IActionResult
    {
        public JsonResult(object dataTransferObject, TcpClient currentClient)
            : this(dataTransferObject, currentClient, null)
        {
        }

        public JsonResult(object dataTransferObject, TcpClient currentClient, JsonSerializerOptions serializerOptions)
        {
            this.dataTransferObject = dataTransferObject ?? throw new ArgumentNullException(nameof(dataTransferObject));
            this.currentClient = currentClient ?? throw new ArgumentNullException(nameof(currentClient));
            this.serializerOptions = serializerOptions;
        }

        public void AddHeaderEntry(string key, string value)
        {
            tempHeaderEntries.Add(KeyValuePair.Create(key, value));
        }

        public void Execute()
        {
            NetworkStream responseStream = currentClient.GetStream();
            EnsureResponseStream(responseStream);

            string payload = string.Empty;

            if (serializerOptions != null)
                payload = JsonSerializer.Serialize(dataTransferObject, dataTransferObject.GetType(), serializerOptions);
            else
                payload = JsonSerializer.Serialize(dataTransferObject, dataTransferObject.GetType());

            HttpResponseHeader responseHeader = new HttpResponseHeader(HttpStatusCode.OK, payload?.Length ?? 0, "Json");
            responseHeader.AddRange(tempHeaderEntries);

            byte[] responseHeaderBytes = Encoding.UTF8.GetBytes(responseHeader.ToString());
            responseStream.Write(responseHeaderBytes);

            byte[] responseMessageBytes = Encoding.UTF8.GetBytes(payload);
            responseStream.Write(responseMessageBytes);
        }

        private void EnsureResponseStream(NetworkStream responseStream)
        {
            if (responseStream == null)
                throw new ArgumentNullException($"{nameof(responseStream)} cannot be null.");

            while (responseStream.DataAvailable)
                responseStream.ReadByte();
        }

        private readonly object dataTransferObject;
        private readonly TcpClient currentClient;
        private readonly JsonSerializerOptions serializerOptions;

        private readonly List<KeyValuePair<string, string>> tempHeaderEntries = new List<KeyValuePair<string, string>>();
    }
}
