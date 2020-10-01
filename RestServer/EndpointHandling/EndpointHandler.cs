using RestServer.CommunicationObjects;
using RestServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;

namespace RestServer.EndpointHandling
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

            if (match.Action is null)
                throw new ArgumentNullException($"match.HttpMethod.Template");

            if (match.Action.ReturnType != typeof(IActionResult))
                throw new NotSupportedException($"Invalid Match: Invalid Action ReturnType:{match.Action.ReturnType}. " +
                    $"ReturnType must be {typeof(IActionResult)}");

            if (match.HttpMethod == null)
                throw new ArgumentNullException($"{nameof(match)}.{nameof(match.HttpMethod)} cannot be null.");

            if (match.HttpMethod.Template == null)
                throw new ArgumentNullException($"{nameof(match)}.{nameof(match.HttpMethod)}.{nameof(match.HttpMethod.Template)} cannot be null.");

            if (client is null)
                throw new ArgumentNullException($"{nameof(client)} cannot be null.", nameof(client));

            Assert.NotNull(context, nameof(context));
            Assert.NotNull(context.Content, $"{nameof(context)}.{nameof(context.Content)}.");

            Assert.NotNull(match.MatchingActionPathSegments, $"{nameof(match)}.{nameof(match.MatchingActionPathSegments)}");
            Assert.NotNull(match.TemplatePathSegments, $"{nameof(match)}.{nameof(match.TemplatePathSegments)}");

            Assert.That(match.MatchingActionPathSegments.Length == match.TemplatePathSegments.Length,
                "Invalid match: actionPathLength must be equal to templatePathLength");

            RequestBodyExtractor bodyExtractor = new RequestBodyExtractor(context);

            ConstructorInfo constructor = match.Controller.GetConstructors()
                .First();

            List<object> parameters = ResolveConstructorParameters(match, client, context, constructor);
            object controller = constructor.Invoke(parameters.ToArray());

            FieldInfo clientField = match.Controller.GetField("client");

            if (clientField != null && clientField.FieldType == typeof(TcpClient))
                clientField.SetValue(controller, client);
            else
                Console.WriteLine($"Warning: Field: client could not be found on Baseclass: {nameof(ControllerBase)}.");

            List<object> arguments = new List<object>();

            ParameterInfo[] actionParanmeters = match.Action.GetParameters();

            foreach (ParameterInfo actionParameter in actionParanmeters)
            {
                int position = Array.IndexOf(match.TemplatePathSegments, $"{{{actionParameter.Name}}}");
                string[] supportedContentTypes = { "Json", "Text", "Text/Pain" };

                if (position == -1)
                {
                    if (supportedContentTypes.Any(sct => sct.Equals(context.Content.ContentType, StringComparison.OrdinalIgnoreCase)))
                    {
                        byte[] requestBody = null;

                        if (bodyExtractor.BodyExtracted)
                            requestBody = bodyExtractor.Cache.ToArray();
                        else
                        {
                            try
                            {
                                requestBody = bodyExtractor.Extract();
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

                        Assert.NotNull(requestBody, nameof(requestBody));

                        try
                        {
                            object result = JsonSerializer.Deserialize(requestBody, actionParameter.ParameterType);
                            parameters.Add(result);
                        }
                        catch (JsonException jsonEx)
                        {
                            EndPointHandlerException error = BuildEndpointHandlerException(
                                     $"Could not Resolve Action Parameter:{actionParameter.Name} of Type: {actionParameter.ParameterType}.{Environment.NewLine}" +
                                     "Failed to Deserialize the RequestBody.",
                                     match,
                                     jsonEx
                                );

                            throw error;
                        }
                    }
                    else
                    {
                        NotSupportedException innerException = new NotSupportedException(
                            $"Could not Resolve Action Parameter:{actionParameter.Name} of Type: {actionParameter.ParameterType}. " +
                            $"Tried to Deserialize Content of Type:{context.Content.ContentType} which is not supported. " +
                            $"Supported Contenttypes are: {string.Join(',', supportedContentTypes)}");
                        EndPointHandlerException error = BuildEndpointHandlerException(match, innerException);

                        throw error;
                    }
                }
                else
                {

                }
            }

            return (IActionResult)match.Action.Invoke(controller, arguments.ToArray());
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

            if(info == null)
                return new EndPointHandlerException($"Error Invoking Controller:{match.Controller}.{match.Action}.", innerException);
            else
                return new EndPointHandlerException(
                    $"Error Invoking Controller:{match.Controller}.{match.Action}.{Environment.NewLine}" +
                    info,
                    innerException)
                ;
        }
    }
}
