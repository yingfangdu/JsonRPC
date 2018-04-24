using Ninja.WebSockets;
using StreamJsonRpc;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using JsonRPCServer;

namespace JsonRPC
{
    internal class WebServer : IDisposable
    {
        private TcpListener _listener;
        private bool _isDisposed = false;
        private readonly IWebSocketServerFactory _webSocketServerFactory;

        public WebServer(IWebSocketServerFactory webSocketServerFactory)
        {
            _webSocketServerFactory = webSocketServerFactory;
        }

        private void ProcessTcpClient(TcpClient tcpClient)
        {
            Task.Run(() => ProcessTcpClientAsync(tcpClient));
        }

        private async Task ProcessTcpClientAsync(TcpClient tcpClient)
        {
            CancellationTokenSource source = new CancellationTokenSource();

            try
            {
                if (_isDisposed)
                {
                    return;
                }

                // this worker thread stays alive until either of the following happens:
                // Client sends a close conection request OR
                // An unhandled exception is thrown OR
                // The server is disposed
                Console.WriteLine("Server: Connection opened. Reading Http header from stream");

                // get a secure or insecure stream
                Stream stream = tcpClient.GetStream();
                WebSocketHttpContext context = await _webSocketServerFactory.ReadHttpHeaderFromStreamAsync(stream);
                if (context.IsWebSocketRequest)
                {
                    // disable ping pong for now (it is causing multi-threaded issues)
                    var options = new WebSocketServerOptions() { KeepAliveInterval = TimeSpan.Zero };
                    Console.WriteLine("Http header has requested an upgrade to Web Socket protocol. Negotiating Web Socket handshake");
                    WebSocket webSocket = await _webSocketServerFactory.AcceptWebSocketAsync(context, options);

                    Console.WriteLine("Web Socket handshake response sent. Stream ready.");
                    await RespondToWebSocketRequestAsync(webSocket, source.Token);
                }
                else
                {
                    Console.WriteLine("Http header contains no web socket upgrade request. Ignoring");
                }

                Console.WriteLine("Server: Connection closed");
            }
            catch (ObjectDisposedException)
            {
                // do nothing. This will be thrown if the Listener has been stopped
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                try
                {
                    tcpClient.Client.Close();
                    tcpClient.Close();
                    source.Cancel();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to close TCP connection: {ex}");
                }
            }
        }

        public async Task RespondToWebSocketRequestAsync(WebSocket webSocket, CancellationToken token)
        {
            var jsonRpc = new JsonRpc(new WebSocketMessageHandler(webSocket));
            jsonRpc.AddLocalRpcTarget(new JsonService());
            jsonRpc.StartListening();
            await jsonRpc.Completion;
        }

        public async Task Listen(int port)
        {
            try
            {
                IPAddress localAddress = IPAddress.Any;
                _listener = new TcpListener(localAddress, port);
                _listener.Start();
                Console.WriteLine($"Server started listening on port {port}");
                while (true)
                {
                    TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                    ProcessTcpClient(tcpClient);
                }
            }
            catch (SocketException ex)
            {
                string message = string.Format("Error listening on port {0}. Make sure IIS or another application is not running and consuming your port.", port);
                throw new Exception(message, ex);
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                // safely attempt to shut down the listener
                try
                {
                    if (_listener != null)
                    {
                        if (_listener.Server != null)
                        {
                            _listener.Server.Close();
                        }

                        _listener.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Console.WriteLine("Web Server disposed");
            }
        }
    }
}
