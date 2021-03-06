﻿using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace RestServer.WebServer.EndpointHandling
{
    public sealed class EndpointHandler
    {
        public IServiceProvider ServiceProvider { get; }

        public EndpointHandler(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IActionResult Invoke(RouteMatch match, TcpClient client, RequestContext context)
        {
            if (match is null)
                throw new ArgumentNullException($"{nameof(match)} cannot be null.", nameof(match));

            if (match.ActionMatch.Action is null)
                throw new ArgumentNullException($"match.HttpMethod.Template");

            if (match.ActionMatch.Action.ReturnType != typeof(IActionResult))
                throw new NotSupportedException($"Invalid Match: Invalid Action ReturnType:{match.ActionMatch.Action.ReturnType}. " +
                    $"ReturnType must be {typeof(IActionResult)}");

            if (match.ActionMatch.HttpMethod == null)
                throw new ArgumentNullException($"{nameof(match)}.{nameof(match.ActionMatch.HttpMethod)} cannot be null.");

            if (match.ActionMatch.HttpMethod.Template == null)
                throw new ArgumentNullException($"{nameof(match)}.{nameof(match.ActionMatch.HttpMethod)}.{nameof(match.ActionMatch.HttpMethod.Template)} cannot be null.");

            if (client is null)
                throw new ArgumentNullException($"{nameof(client)} cannot be null.", nameof(client));

            Assert.NotNull(context, nameof(context));
            Assert.NotNull(context.Content, $"{nameof(context)}.{nameof(context.Content)}.");

            Assert.NotNull(match.MatchingActionPathSegments, $"{nameof(match)}.{nameof(match.MatchingActionPathSegments)}");
            Assert.NotNull(match.TemplatePathSegments, $"{nameof(match)}.{nameof(match.TemplatePathSegments)}");

            Assert.That(match.MatchingActionPathSegments.Length == match.TemplatePathSegments.Length,
                "Invalid match: actionPathLength must be equal to templatePathLength");

            RequestBodyExtractor bodyExtractor = new RequestBodyExtractor(context);

            object controller = BuildController(match, client, context);
            List<object> invokeArguments = new List<object>();

            foreach (ParameterInfo actionParameter in match.ActionMatch.Action.GetParameters())
            {
                string correspondingParameter = string.Empty;

                int actionParameterIndex = Array.IndexOf(match.TemplatePathSegments, $"{{{actionParameter.Name}}}");

                if (actionParameterIndex == -1)
                {
                    if (context.Parameters.HeaderEntries.ContainsKey(actionParameter.Name))
                        correspondingParameter = ExtractFromRequestHeader(context, actionParameter);
                    else
                    {
                        byte[] bodyContent = ExtractFromRequestBody(match, context, bodyExtractor, actionParameter);
                        correspondingParameter = Encoding.UTF8.GetString(bodyContent);
                    }
                }
                else
                {
                    string routeParameter = match.MatchingActionPathSegments[actionParameterIndex];
                    correspondingParameter = ConvertToDeserializeableString(routeParameter, actionParameter);
                }

                try
                {
                    if (actionParameter.ParameterType == typeof(Guid))
                    {
                        if (Guid.TryParse(correspondingParameter, out Guid result))
                            invokeArguments.Add(result);
                        else
                            throw new EndPointHandlerException(
                                $"Error parsing expected Guid- Parameter: {actionParameter.Name}. "
                                + $"Value: {correspondingParameter} could not be parsed."
                            );
                    }
                    else
                    {
                        object result = JsonSerializer.Deserialize(correspondingParameter, actionParameter.ParameterType);
                        invokeArguments.Add(result);
                    }
                }
                catch (JsonException jsonEx)
                {
                    throw BuildEndpointHandlerException(
                         $"Could not Resolve Action Parameter:{actionParameter.Name} of Type: {actionParameter.ParameterType}.{Environment.NewLine}",
                         match,
                         jsonEx
                    );
                }
            }

            return (IActionResult)match.ActionMatch.Action.Invoke(controller, invokeArguments.ToArray());
        }

        private static string ExtractFromRequestHeader(RequestContext context, ParameterInfo actionParameter)
        {
            string headerValue = context.Parameters.HeaderEntries[actionParameter.Name].First();
            return ConvertToDeserializeableString(headerValue, actionParameter);
        }

        private static string ConvertToDeserializeableString(string value, ParameterInfo actionParameter)
        {
            if (actionParameter.ParameterType == typeof(string))
                return $"\"{value}\"";
            else
                return value;
        }

        private static byte[] ExtractFromRequestBody(RouteMatch match, RequestContext context, RequestBodyExtractor bodyExtractor, ParameterInfo actionParameter)
        {
            if (supportedContentTypes.Any(sct => sct.Equals(context.Content.ContentType, StringComparison.OrdinalIgnoreCase)))
            {
                if (bodyExtractor.BodyExtracted)
                    return bodyExtractor.Cache.ToArray();
                else
                {
                    try
                    {
                        return bodyExtractor.Extract();
                    }
                    catch (RequestBodyExtractorException bodyExtractorEx)
                    {
                        EndPointHandlerException error = BuildEndpointHandlerException(
                             $"Could not Resolve Action Parameter:{actionParameter.Name} of Type: {actionParameter.ParameterType}.",
                             match,
                             bodyExtractorEx
                        );

                        throw error;
                    }
                }
            }
            else
            {
                NotSupportedException innerException = new NotSupportedException(
                    $"Could not Resolve Action Parameter:{actionParameter.Name} of Type: {actionParameter.ParameterType}. " +
                    $"Invalid Content-Type:{context.Content.ContentType}. " +
                    $"Supported Contenttypes are: {string.Join(',', supportedContentTypes)}");

                throw BuildEndpointHandlerException(match, innerException);
            }
        }

        private object BuildController(RouteMatch match, TcpClient client, RequestContext context)
        {
            ConstructorInfo constructor = match.ActionMatch.Controller.GetConstructors()
                .First();

            List<object> parameters = ResolveConstructorParameters(match, client, context, constructor);
            object controller = constructor.Invoke(parameters.ToArray());

            if (!match.ActionMatch.Controller.IsSubclassOf(typeof(ControllerBase)))
                return controller;

            FieldInfo clientField = typeof(ControllerBase).GetField("client", BindingFlags.NonPublic | BindingFlags.Instance);

            if (clientField != null && clientField.FieldType == typeof(TcpClient))
                clientField.SetValue(controller, client);
            else
                Console.WriteLine($"Warning: Field: client could not be found on Baseclass: {nameof(ControllerBase)}.");

            return controller;
        }

        private List<object> ResolveConstructorParameters(RouteMatch match, TcpClient client, RequestContext context, ConstructorInfo constructor)
        {
            List<object> parameters = new List<object>();
            ParameterInfo[] constructorParameters = constructor.GetParameters();

            foreach (ParameterInfo currentParameter in constructorParameters)
            {
                object resolvedParameter = ServiceProvider.GetService(currentParameter.ParameterType);

                if (resolvedParameter != null)
                    parameters.Add(resolvedParameter);
                else if (currentParameter.ParameterType == typeof(RequestContext))
                    parameters.Add(context);
                else if (currentParameter.ParameterType == typeof(TcpClient))
                    parameters.Add(client);
                else if (currentParameter.ParameterType == typeof(NetworkStream))
                    parameters.Add(client.GetStream());
                else
                {
                    NotSupportedException innerException = new NotSupportedException(
                        $"Could not resolve ConstructorParameter:{currentParameter.Name} of Type:{currentParameter.ParameterType}.");
                    EndPointHandlerException error = BuildEndpointHandlerException(match, innerException);

                    throw error;
                }

            }

            return parameters;
        }

        private static EndPointHandlerException BuildEndpointHandlerException(RouteMatch match, Exception innerException)
        {
            return BuildEndpointHandlerException(null, match, innerException);
        }

        private static EndPointHandlerException BuildEndpointHandlerException(string info, RouteMatch match, Exception innerException)
        {
            Assert.NotNull(match, nameof(match));

            if (info == null)
                return new EndPointHandlerException($"Error Invoking Controller:{match.ActionMatch.Controller}.{match.ActionMatch.Action}.", innerException);
            else
                return new EndPointHandlerException(
                    $"Error Invoking Controller:{match.ActionMatch.Controller}.{match.ActionMatch.Action}.{Environment.NewLine}" +
                    info,
                    innerException);
        }

        private static readonly string[] supportedContentTypes = { "Json", "Text", "Text/Plain" };
    }
}
