using RestServer.EndpointHandling.Attributes;
using RestServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestServer.EndpointHandling
{
    public sealed class RouteActionMatch
    {
        public Type Controller { get; }

        public MethodInfo Action { get; }

        public HttpMethodAttribute HttpMethod { get; }

        public RouteActionMatch(Type controller, MethodInfo action, HttpMethodAttribute httpMethod)
        {
            Assert.NotNull(controller, nameof(controller));
            Assert.NotNull(action, nameof(action));
            Assert.NotNull(httpMethod, nameof(httpMethod));

            Controller = controller;
            Action = action;
            HttpMethod = httpMethod;
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
                string currentRequestPathSegment = actionPathSegments[i];

                if (!currentTemplateSegment.Equals(currentRequestPathSegment, StringComparison.OrdinalIgnoreCase) &&
                    !(currentTemplateSegment.StartsWith("{") && currentTemplateSegment.EndsWith("}")))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"Controller: {Controller}, Action: {Action}, HttpMethod: {HttpMethod}";
        }
    }
}
