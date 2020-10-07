using RestServer.CommunicationObjects;
using RestServer.EndpointHandling.Attributes;
using RestServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestServer.EndpointHandling
{
    public sealed class EndpointHandlerRegister
    {
        public void RegisterTypes()
        {
            registeredEndpointHandlerTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(ControllerBase)))
                .ToList();
        }

        public RouteMatch GetEndPointHandler(RequestContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Request == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Request)}");

            if (string.IsNullOrWhiteSpace(context.Request.Path))
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Request)}.{nameof(context.Request.Path)}");

            string requestPath = context.Request.Path;
            string[] requestPathSegments = requestPath
                .Split("/")
                .Skip(1)
                .ToArray();

            if (requestPathSegments.Length > 0)
            {
                string controller = requestPathSegments[0];

                if (controller == string.Empty)
                {
                    //Todo: default e.g.: HomeController
                }
                else
                {
                    Type controllerType = registeredEndpointHandlerTypes
                        .FirstOrDefault(endPoint => IsRequestedControllerType(controller, endPoint));

                    if (controllerType == null)
                    {
                        NotFoundException innerException = new NotFoundException(
                            $"Die angeforderte Resource:{controller} konnte nicht gefunden werden.");

                        throw new EndpointHandlerRegisterException(
                            "Error resolving EndpointHandler. See InnerException.",
                            innerException);
                    }

                    if (context.Request.Method == HttpVerb.GET)
                    {
                        var methodMatches = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .Where(m => m.IsDefined(typeof(HttpGetAttribute), false) && m.ReturnType == typeof(IActionResult))
                            .Select(m => new {Method = m, Attr = (HttpGetAttribute)m.GetCustomAttribute(typeof(HttpGetAttribute))} )
                            .ToArray();

                        string[] actionPathSegments = requestPathSegments
                            .Skip(1)
                            .Where(segment => !string.IsNullOrWhiteSpace(segment))
                            .ToArray();

                        var match = methodMatches
                            .FirstOrDefault(match => RouteMatch.RequestPathMatchesRouteTemplate(match.Attr.GetTemplatePathSegments(), actionPathSegments));

                        if (match != null)
                            return new RouteMatch(controllerType, match.Method, match.Attr, requestPath);
                        else
                            throw new NotFoundException(
                                $"Die angeforderte Resource:{controllerType} konnte unter dem Pfad:{requestPath} nicht gefunden werden.");
                    }
                }

                throw new NotImplementedException($"{nameof(GetEndPointHandler)} not implemented yet");
            }
            else
            {
                FormatException innerException = new FormatException($"Invalid RequestFormat: {context.Request}");

                throw new EndpointHandlerRegisterException(
                    "Error resolving EndpointHandler. See InnerException.",
                    innerException);
            }
        }

        private static bool IsRequestedControllerType(string controller, Type currentEndpointHandler)
        {
            string endpointHandlerName = currentEndpointHandler.Name;

            if (endpointHandlerName.Contains("Controller", StringComparison.OrdinalIgnoreCase))
                endpointHandlerName = endpointHandlerName.Replace("Controller", string.Empty, StringComparison.OrdinalIgnoreCase);

            return endpointHandlerName.Equals(controller, StringComparison.OrdinalIgnoreCase);
        }

        private List<Type> registeredEndpointHandlerTypes = new List<Type>();
    }
}