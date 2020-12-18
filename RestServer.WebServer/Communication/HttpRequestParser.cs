using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.CommunicationObjects.CommunicationObjectBuilders;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.WebServer.Communication
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

            byte[] buffer = new byte[1];
            int bytesRead = requestStream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
                requestHeaderTextBuilder.Append(Encoding.UTF8.GetString(buffer));

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

            string requestHeaderText = RequestHeaderStringNormalizer.NormalizeForApplication(requestHeaderTextBuilder.ToString());

            Console.WriteLine(requestHeaderText);

            if (string.IsNullOrWhiteSpace(requestHeaderText))
                throw new HttpRequestParserException("Empty Requests cannot be parsed.");

            return RequestContextBuilder.BuildRequestContext(requestHeaderText, requestStream);
        }
    }
}
