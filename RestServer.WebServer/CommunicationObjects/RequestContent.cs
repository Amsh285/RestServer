using System;
using System.Net.Sockets;

namespace RestServer.WebServer.CommunicationObjects
{
    public partial class RequestContext
    {
        public sealed class RequestContent
        {
            public string ContentType { get; private set; }

            public int ContentLength { get; private set; }

            public NetworkStream InputStream { get; private set; }

            public RequestContent(string contentType, int contentLength, NetworkStream inputStream)
            {
                if (inputStream == null)
                    throw new ArgumentNullException($"{nameof(inputStream)} cannot be null.");

                ContentType = contentType;
                ContentLength = contentLength;
                InputStream = inputStream;
            }
        }
    }
}
