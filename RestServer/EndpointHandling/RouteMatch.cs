using RestServer.EndpointHandling.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestServer.EndpointHandling
{
    public sealed class RouteMatch
    {
        public Type Controller { get; set; }

        public MethodInfo Action { get; }

        public HttpMethodAttribute HttpMethod { get; }

        public string[] TemplatePathSegments { get; }

        public string MatchingPath { get; }

        public string[] MatchingPathSegments { get { return MatchingPath.Split("/"); } }

        public string[] MatchingActionPathSegments
        {
            get
            {
                return MatchingPathSegments
                    .Skip(1)
                    .ToArray();
            }
        }

        public RouteMatch(Type controller, MethodInfo action, HttpMethodAttribute httpMethod, string matchingPath)
        {
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));
            Action = action ?? throw new ArgumentNullException(nameof(action));
            HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            TemplatePathSegments = httpMethod.Template?.Split("/") ?? new string[0];
            MatchingPath = matchingPath ?? throw new ArgumentNullException(nameof(matchingPath));
        }

        public static bool RequestPathMatchesRouteTemplate(IReadOnlyList<string> templatePathSegments, IReadOnlyList<string> actionPathSegments)
        {
            if (templatePathSegments is null)
                throw new ArgumentNullException(nameof(templatePathSegments));
            if (actionPathSegments is null)
                throw new ArgumentNullException(nameof(actionPathSegments));

            if (actionPathSegments.Count != templatePathSegments.Count)
                return false;

            for (int i = 0; i < templatePathSegments.Count; i++)
            {
                string currentTemplateSegment = templatePathSegments[i];

                if (!(currentTemplateSegment.StartsWith("{") && currentTemplateSegment.EndsWith("}")))
                {
                    string currentRequestPathSegment = templatePathSegments[i];

                    if (!currentTemplateSegment.Equals(currentRequestPathSegment, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }

            return true;
        }
    }
}
