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
        public EndpointHandlerRegister(IEnumerable<Type> endPointHandlerTypes)
        {
            Assert.NotNull(endPointHandlerTypes, nameof(endPointHandlerTypes));

            this.registeredEndpointHandlerTypes = endPointHandlerTypes
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

                string[] actionPathSegments = requestPathSegments
                    .Skip(1)
                    .Where(segment => !string.IsNullOrWhiteSpace(segment))
                    .ToArray();

                try
                {
                    RouteActionMatch actionMatch = EndpointControllerReflector.SearchRouteActionMatch(actionPathSegments, context.Request.Method, controllerType);
                    return new RouteMatch(actionMatch, requestPath);
                }
                catch (NotFoundException nfEx)
                {
                    throw new EndpointHandlerRegisterException($"Die angeforderte Resource:{controllerType} konnte unter dem Pfad:{requestPath} nicht gefunden werden.", nfEx);
                }
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