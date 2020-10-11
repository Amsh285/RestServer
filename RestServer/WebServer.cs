using RestServer.Communication;
using RestServer.CommunicationObjects;
using RestServer.EndpointHandling;
using RestServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RestServer
{
    public sealed class WebServer
    {
        public IPAddress Address { get; }
        public int Port { get; }
        public IEnumerable<Type> HandlerTypes { get; }

        public WebServer(IPAddress address, int port, IEnumerable<Type> handlerTypes)
        {
            Assert.NotNull(address, nameof(address));
            Assert.That(port >= 0 && port <= 65535, $"{nameof(port)} must be between 0 and 65535.");
            Assert.NotNull(handlerTypes, nameof(handlerTypes));

            Address = address;
            Port = port;
            HandlerTypes = handlerTypes;
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
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(localAddr, 13000);

            Console.WriteLine("Starte WebServer auf 127.0.0.1:13000.");

            IServiceProvider serviceProvider = new HardcodedEmptyServiceProvider();
            EndpointHandler endpointHandler = new EndpointHandler(serviceProvider);

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
            catch (EndpointHandlerRegisterException endPointHandlerEx)
            {
                if (endPointHandlerEx.InnerException is FormatException fex)
                {
                    HttpStatusCodeResult.BadRequest(client, fex.Message)
                        .Execute();
                }
                else if (endPointHandlerEx.InnerException is NotFoundException nfex)
                {
                    HttpStatusCodeResult.NotFound(client, nfex.Message)
                        .Execute();
                }
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
