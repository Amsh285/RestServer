using System;
using System.Net.Sockets;
using System.Text;

namespace RestServer.CommunicationObjects
{
    public abstract class HttpActionResult : IActionResult
    {
        public HttpActionResult(TcpClient currentClient, HttpResponseHeader responseHeader)
        {
            this.currentClient = currentClient ?? throw new ArgumentNullException($"{nameof(currentClient)} cannot be null.");
            this.responseHeader = responseHeader ?? throw new ArgumentNullException($"{nameof(responseHeader)} cannot be null.");
        }

        public virtual void Execute()
        {
            NetworkStream responseStream = currentClient.GetStream();
            EnsureResponseStream(responseStream);

            byte[] responseHeaderBytes = Encoding.UTF8.GetBytes(responseHeader.ToString());
            responseStream.Write(responseHeaderBytes, 0, responseHeaderBytes.Length);

            ExecuteResponsePayload(currentClient);
        }

        public abstract void ExecuteResponsePayload(TcpClient currentClient);

        private void EnsureResponseStream(NetworkStream responseStream)
        {
            if (responseStream == null)
                throw new ArgumentNullException($"{nameof(responseStream)} cannot be null.");

            while (responseStream.DataAvailable)
                responseStream.ReadByte();
        }

        private readonly TcpClient currentClient;
        private readonly HttpResponseHeader responseHeader;
    }
}
