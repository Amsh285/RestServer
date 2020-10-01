using RestServer.Communication;
using RestServer.CommunicationObjects;
using RestServer.EndpointHandling;
using RestServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace RestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //127.0.0.1:13000
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(localAddr, 13000);

            Console.WriteLine("Starte WebServer auf 127.0.0.1:13000.");

            IServiceProvider serviceProvider = new HardcodedServiceProvider();
            EndpointHandler endpointHandler = new EndpointHandler(serviceProvider);

            EndpointHandlerRegister handlerRegister = new EndpointHandlerRegister();
            handlerRegister.RegisterTypes();

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

            byte[] readBuffer = new byte[1024];

            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();

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
                        if(endPointHandlerEx.InnerException is FormatException fex)
                        {
                            HttpStatusCodeResult.BadRequest(client, fex.Message)
                                .Execute();
                        }
                        else if(endPointHandlerEx.InnerException is NotFoundException nfex)
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
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        private static void LogRequestBody(NetworkStream requestStream)
        {
            // Test only
            // Antworten werden in den gleichen Networkstream geschrieben daher muss dieser vorher komplett durchgelesen werden
            // bevor man eine Antwort an den Client zurückschicken kann.
            var builder = new StringBuilder();

            while (requestStream.DataAvailable)
            {
                builder.Append(Convert.ToChar(requestStream.ReadByte()));
            }

            Console.WriteLine("Request-Body:");
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine(builder);
            Console.WriteLine("-----------------------------------------------------------------------------");
        }
    }
}
