using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace RestServer.CommunicationObjects
{
    public sealed class HttpStatusCodeResult : HttpActionResult
    {
        public HttpStatusCodeResult(TcpClient currentClient, HttpResponseHeader responseHeader)
            : this(currentClient, responseHeader, null)
        {
        }

        public HttpStatusCodeResult(TcpClient currentClient, HttpResponseHeader responseHeader, string message)
            : base(currentClient, responseHeader)
        {
            this.message = message;
        }

        public static HttpStatusCodeResult Ok(TcpClient client, string message = null)
        {
            return StatusMessage(HttpStatusCode.OK, client, message);
        }

        public static HttpStatusCodeResult BadRequest(TcpClient client, string errorMessage = null)
        {
            return StatusMessage(HttpStatusCode.BadRequest, client, errorMessage);
        }

        public static HttpStatusCodeResult NotFound(TcpClient client, string errorMessage = null)
        {
            return StatusMessage(HttpStatusCode.NotFound, client, errorMessage);
        }

        public static HttpStatusCodeResult InternalServerError(TcpClient client, string errorMessage = null)
        {
            return StatusMessage(HttpStatusCode.InternalServerError, client, errorMessage);
        }

        public static HttpStatusCodeResult StatusMessage(HttpStatusCode status, TcpClient client, string message)
        {
            HttpResponseHeader responseHeader = new HttpResponseHeader(status, message?.Length ?? 0, "text, plain");
            AddDefaultHeaderFields(responseHeader);

            return new HttpStatusCodeResult(client, responseHeader, message);
        }

        public override void ExecuteResponsePayload(TcpClient currentClient)
        {
            if (currentClient == null)
                throw new ArgumentNullException($"{nameof(currentClient)} cannot be null.");

            if (message != null)
            {
                NetworkStream stream = currentClient.GetStream();
                byte[] responseMessageBytes = Encoding.UTF8.GetBytes(message, 0, 10);

                stream.Write(responseMessageBytes);
            }
        }

        private static void AddDefaultHeaderFields(HttpResponseHeader responseHeader)
        {
            if (responseHeader == null)
                throw new ArgumentNullException($"{nameof(responseHeader)} cannot be null.");

            responseHeader.Add("Server", "Dorian Monster Duel Cards TM ©/ 1.3.3.7");
        }

        private readonly string message;
    }
}
