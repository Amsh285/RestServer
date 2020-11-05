using RestServer.WebServer.Communication;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.Infrastructure;
using RestServer.WebServer.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RestServer.WebServer
{
    public sealed class HttpWebServer
    {
        public IPAddress Address { get; }
        public int Port { get; }
        public IEnumerable<Type> HandlerTypes { get; }
        public IServiceProvider ServiceProvider { get; }

        public HttpWebServer(IPAddress address, int port, IEnumerable<Type> handlerTypes, IServiceProvider serviceProvider)
        {
            Assert.NotNull(address, nameof(address));
            Assert.That(port >= 0 && port <= 65535, $"{nameof(port)} must be between 0 and 65535.");
            Assert.NotNull(handlerTypes, nameof(handlerTypes));
            Assert.NotNull(serviceProvider, nameof(serviceProvider));

            Address = address;
            Port = port;
            HandlerTypes = handlerTypes;
            ServiceProvider = serviceProvider;
        }

        public Task Start()
        {
            return Task.Run(ExecuteWebServer, cancellationTokenSource.Token);
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        private void ExecuteWebServer()
        {
            TcpListener listener = new TcpListener(Address, Port);
            Console.WriteLine($"Starte WebServer auf {Address}:{Port}.");

            EndpointHandler endpointHandler = new EndpointHandler(ServiceProvider);
            EndpointHandlerRegister handlerRegister = new EndpointHandlerRegister(HandlerTypes);

            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting TcpListener: {ex.Message}");
                return;
            }

            Console.WriteLine("Webserver wurde erfolgreich gestartet.");

            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Task.Run(() => ProcessRequest(endpointHandler, handlerRegister, client));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error AcceptClient: {ex.Message}");
            }
            finally
            {
                listener.Stop();
            }
        }

        private static void ProcessRequest(EndpointHandler endpointHandler, EndpointHandlerRegister handlerRegister, TcpClient client)
        {
            try
            {
                NetworkStream requestStream = client.GetStream();

                Console.WriteLine($"Nachricht erhalten um: {DateTime.Now} Client: {client.Client.RemoteEndPoint}");
                Console.WriteLine("-----------------------------------------------------------------------------");

                HttpRequestParser requestParser = new HttpRequestParser();
                RequestContext context;

                context = requestParser.ParseRequestStream(requestStream);

                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.WriteLine();

                RouteMatch match = handlerRegister.GetEndPointHandler(context);

                if (match != null)
                {
                    IActionResult result = endpointHandler.Invoke(match, client, context);
                    result.Execute();
                }
                else
                    HttpStatusCodeResult.NotFound(client)
                        .Execute();

                client.Close();
            }
            catch (HttpRequestParserException parserEx)
            {
                string parseErrorMessage = $"Ungültiger Request, parsen nicht möglich: {parserEx.Message}";

                Console.WriteLine(parseErrorMessage);
                IActionResult parseError = HttpStatusCodeResult.BadRequest(client, parseErrorMessage);
            }
            catch (Exception ex) when (ex is EndPointHandlerException || ex is EndpointHandlerRegisterException)
            {
                HttpStatusCodeResult.BadRequest(client, ex.GetFullMessage(verbose: true))
                    .Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unerwarteter Fehler: {ex.Message}");

                HttpStatusCodeResult.InternalServerError(client)
                    .Execute();
            }
            finally
            {
                client.Close();
            }
        }

        private static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    }
}
