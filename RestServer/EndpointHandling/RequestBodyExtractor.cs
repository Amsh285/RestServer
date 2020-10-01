using RestServer.CommunicationObjects;
using RestServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace RestServer.EndpointHandling
{
    public class RequestBodyExtractor
    {
        public bool BodyExtracted { get; private set; }

        public IReadOnlyCollection<byte> Cache { get; private set; }

        public RequestContext Context { get; }

        public RequestBodyExtractor(RequestContext context)
        {
            Assert.NotNull(context, nameof(context));
            Assert.NotNull(context.Content, $"{nameof(context)}{nameof(context.Content)}");

            Context = context;
            Cache = Enumerable.Empty<byte>()
                .ToArray();
        }

        public byte[] Extract()
        {
            if (BodyExtracted)
                throw new InvalidOperationException($"Cannot extract RequestBody more than once.");

            NetworkStream stream = Context.Content.InputStream;

            if (Context.Content.ContentLength == 0)
            {
                FormatException innerException = new FormatException("RequestBody is Empty.");
                RequestBodyExtractorException error = GenerateBodyExtractorException(innerException);

                throw error;
            }
            
            if (Context.Content.ContentLength > 10 * 1024 * 1024)
            {
                FormatException innerException = new FormatException("ContentLength is to long, only 10MB are allowed.");
                RequestBodyExtractorException error = GenerateBodyExtractorException(innerException);

                throw error;
            }

            if (stream.CanRead)
            {
                Cache = ExtractRequestBody(Context.Content.ContentLength, stream);
                BodyExtracted = true;

                return Cache.ToArray();
            }
            else
            {
                InvalidOperationException innerException = new InvalidOperationException("Cannot Read from NetworkStream.");
                RequestBodyExtractorException error = GenerateBodyExtractorException(innerException);

                throw error;
            }
        }

        private static byte[] ExtractRequestBody(int contentLength, NetworkStream stream)
        {
            byte[] bodyContent = new byte[contentLength];
            byte[] readBuffer = new byte[1024];

            int totalBytesRead = 0, totalBytesMoved = 0;
            
            while (stream.DataAvailable)
            { 
                int bytesRead = stream.Read(readBuffer);
                totalBytesRead += bytesRead;

                if (totalBytesRead > bodyContent.Length)
                    break;

                Array.Copy(readBuffer, 0, bodyContent, totalBytesMoved, bytesRead);
                totalBytesMoved = totalBytesRead;
            }
            
            return bodyContent;
        }

        private static RequestBodyExtractorException GenerateBodyExtractorException(Exception innerException)
        {
            return new RequestBodyExtractorException("Error extractingRequestBody", innerException);
        }
    }
}
