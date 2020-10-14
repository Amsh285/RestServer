using System;

namespace RestServer.WebServer.Communication
{
    public static class RequestHeaderStringNormalizer
    {
        public static string NormalizeForApplication(string webRequestString)
        {
            const string commonNewLineToken = "<<@newline>>";

            return webRequestString
                .Replace("\r\n", commonNewLineToken)
                .Replace("\n", commonNewLineToken)
                .Replace(commonNewLineToken, Environment.NewLine);
        }
    }
}
