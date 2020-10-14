using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace RestServer.WebServer.CommunicationObjects
{
    public class RequestContext
    {
        public RequestLine Request { get; private set; }

        public IReadOnlyDictionary<string, string> RequestHeaderFields { get; private set; }

        public RequestParameters Parameters { get; private set; }

        public RequestContent Content { get; private set; }

        private RequestContext(RequestLine request, IReadOnlyDictionary<string, string> requestHeaderFields, RequestParameters parameters, RequestContent content)
        {
            if (request == null)
                throw new ArgumentNullException($"{nameof(request)} cannot be null.");

            if (requestHeaderFields == null)
                throw new ArgumentNullException($"{nameof(requestHeaderFields)} cannot be null.");

            if (parameters == null)
                throw new ArgumentNullException($"{nameof(parameters)} cannot be null.");

            if (content == null)
                throw new ArgumentNullException($"{nameof(content)} cannot be null.");

            Request = request;
            RequestHeaderFields = requestHeaderFields;
            Parameters = parameters;
            Content = content;
        }

        public static RequestContext Build(RequestLine request, IReadOnlyDictionary<string, string> queryString, IReadOnlyDictionary<string, string> headers, NetworkStream inputStream)
        {
            if (request == null)
                throw new ArgumentNullException($"{nameof(request)} cannot be null.");

            if (queryString == null)
                throw new ArgumentNullException($"{nameof(queryString)} cannot be null.");

            if (headers == null)
                throw new ArgumentNullException($"{nameof(headers)} cannot be null.");

            if (inputStream == null)
                throw new ArgumentNullException($"{nameof(inputStream)} cannot be null.");

            RequestParameters parameters = new RequestParameters(headers, queryString);

            int contentLength;
            int.TryParse(headers.GetValueOrDefault("Content-Length"), out contentLength);

            RequestContent content = new RequestContent(headers.GetValueOrDefault("Content-Type"), contentLength, inputStream);

            return new RequestContext(request, headers, parameters, content);
        }

        public sealed class RequestLine
        {
            public string HttpVersion { get; private set; }

            public string Path { get; private set; }

            public string FullPath { get; }

            public HttpVerb Method { get; private set; }

            public RequestLine(string httpVersion, string path, string fullPath, HttpVerb method)
            {
                HttpVersion = httpVersion;
                Path = path;
                FullPath = fullPath;
                Method = method;
            }
        }

        public sealed class RequestParameters
        {
            public IReadOnlyDictionary<string, string> QueryStringValues { get; private set; }

            public IReadOnlyDictionary<string, string> Headers { get; private set; }

            public RequestParameters(IReadOnlyDictionary<string, string> headers, IReadOnlyDictionary<string, string> queryStringValues)
            {
                Headers = headers;
                QueryStringValues = queryStringValues;
            }
        }

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
