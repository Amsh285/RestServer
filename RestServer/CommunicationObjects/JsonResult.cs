using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace RestServer.CommunicationObjects
{
    public sealed class JsonResult : IActionResult
    {
        public JsonResult(object dataTransferObject, TcpClient currentClient)
        {
            this.dataTransferObject = dataTransferObject ?? throw new ArgumentNullException(nameof(dataTransferObject));
            this.currentClient = currentClient ?? throw new ArgumentNullException(nameof(currentClient));
        }

        public void Execute()
        {
            NetworkStream responseStream = currentClient.GetStream();
            EnsureResponseStream(responseStream);

            string payload = JsonSerializer.Serialize(dataTransferObject, dataTransferObject.GetType());

            HttpResponseHeader responseHeader = new HttpResponseHeader(HttpStatusCode.OK, payload?.Length ?? 0, "Json");
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
    }
}
