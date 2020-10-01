using RestServer.CommunicationObjects;
using RestServer.CommunicationObjects.CommunicationObjectBuilders;
using System;
using System.Net.Sockets;
using System.Text;

namespace RestServer.Communication
{
    public sealed class HttpRequestParser
    {
        public RequestContext ParseRequestStream(NetworkStream requestStream)
        {
            if (requestStream == null)
                throw new ArgumentNullException($"{nameof(requestStream)} cannot be null.");

            StringBuilder requestHeaderTextBuilder = new StringBuilder();

            int currentByte;
            bool lastCharNewLine = false;

            while (requestStream.DataAvailable)
            {
                currentByte = requestStream.ReadByte();

                if (currentByte > -1)
                {
                    char currentChar = Convert.ToChar(currentByte);

                    if (lastCharNewLine && currentChar == '\r')
                    {
                        requestStream.ReadByte();
                        break;
                    }
                    else if (lastCharNewLine && currentChar == '\n')
                        break;

                    lastCharNewLine = currentChar == '\n';
                    requestHeaderTextBuilder.Append(currentChar);
                }
            }

            string requestHeaderText = requestHeaderTextBuilder.ToString();

            Console.WriteLine(requestHeaderText);

            if (string.IsNullOrWhiteSpace(requestHeaderText))
                throw new HttpRequestParserException("Empty Requests cannot be parsed.");

            return RequestContextBuilder.BuildRequestContext(requestHeaderText, requestStream);
        }
    }
}
