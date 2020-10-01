using RestServer.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace RestServer.CommunicationObjects.CommunicationObjectBuilders
{
    internal static class RequestContextBuilder
    {
        public static RequestContext BuildRequestContext(string requestHeaderText, NetworkStream requestStream)
        {
            if (string.IsNullOrWhiteSpace(requestHeaderText))
                throw new ArgumentException($"{nameof(requestHeaderText)} cannot be null, empty or whitespace.");
            if (requestStream == null)
                throw new ArgumentNullException($"{nameof(requestStream)} cannot be null.");

            string[] requestHeaderFields = requestHeaderText.ToString().Split(Environment.NewLine);
            string[] requestLineSegments = requestHeaderFields
                .First()
                .Split(' ');

            if (requestLineSegments.Length != 3)
                throw new HttpRequestParserException($"Invalid RequestLine- Format. Number of Segments:{requestLineSegments.Length}, expected 3.");

            string method = requestLineSegments[0];
            string requestedPath = requestLineSegments[1];
            string version = requestLineSegments[2];

            if (!supportedVersion.Contains(version, StringComparer.OrdinalIgnoreCase))
                throw new HttpRequestParserException($"Invalid Request Version: {version}.");

            string[] requestedPathSegments = requestedPath
                .Split('?');

            string path = string.Empty;
            string queryString = string.Empty;

            if (requestedPathSegments.Length > 0)
                path = requestedPathSegments[0];

            if (requestedPathSegments.Length > 1)
                queryString = requestedPathSegments[1];

            RequestContext.RequestLine requestLine = new RequestContext.RequestLine(version, path, requestedPath, ConvertMethod(method));
            IReadOnlyDictionary<string, string> parsedQueryString = ParseQueryString(queryString);
            IReadOnlyDictionary<string, string> parsedRequestHeaderFields = ParseRequestHeaderFields(requestHeaderFields);

            return RequestContext.Build(requestLine, parsedQueryString, parsedRequestHeaderFields, requestStream);
        }

        private static IReadOnlyDictionary<string, string> ParseQueryString(string queryString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(queryString))
                return result;

            string[] segments = queryString.Split('&');

            foreach (string segment in segments)
            {
                string[] queryStringParameterSegments = segment.Split('=');

                if (queryStringParameterSegments.Length < 2)
                    throw new HttpRequestParserException($"Invalid Querystring- Parameter: {segment}.");

                result.Add(key: queryStringParameterSegments[0], value: queryStringParameterSegments[1]);
            }

            return result;
        }

        private static IReadOnlyDictionary<string, string> ParseRequestHeaderFields(string[] requestHeaderFields)
        {
            KeyValuePair<string, string> exctractHeaderField(string value)
            {
                string[] fieldValues = value.Split(':');
                return new KeyValuePair<string, string>(fieldValues[0], fieldValues[1]);
            };

            IEnumerable<KeyValuePair<string, string>> fields = requestHeaderFields
                .Skip(1)
                .Where(s => !string.IsNullOrWhiteSpace(s) && s.Contains(':'))
                .Select(exctractHeaderField);

            return new Dictionary<string, string>(fields);
        }

        private static HttpVerb ConvertMethod(string method)
        {
            if (method.ToLower() == "post")
                return HttpVerb.POST;
            else if (method.ToLower() == "get")
                return HttpVerb.GET;
            else if (method.ToLower() == "put")
                return HttpVerb.PUT;
            else if (method.ToLower() == "head")
                return HttpVerb.HEAD;
            else if (method.ToLower() == "delete")
                return HttpVerb.DELETE;
            else if (method.ToLower() == "connect")
                return HttpVerb.CONNECT;
            else if (method.ToLower() == "options")
                return HttpVerb.OPTIONS;
            else if (method.ToLower() == "trace")
                return HttpVerb.TRACE;
            else if (method.ToLower() == "patch")
                return HttpVerb.PATCH;

            throw new ArgumentException($"invalid Argument {nameof(method)} value: {method} is not supported.");
        }

        private static readonly string[] supportedVersion = { "	HTTP/1.0", "HTTP/1.1", "HTTP/2", "HTTP/3" };
    }
}
