using Microsoft.IO;
using Ninja.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonRPC
{
    class Program
    {
        static IWebSocketServerFactory _webSocketServerFactory;
        static RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        static void Main(string[] args)
        {
            const int DefaultBlockSize = 16 * 1024;
            const int MaxBufferSize = 128 * 1024;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager(DefaultBlockSize, 4, MaxBufferSize);
            _webSocketServerFactory = new WebSocketServerFactory(_recyclableMemoryStreamManager.GetStream);
            Task task = StartWebServer();
            task.Wait();
        }

        static async Task StartWebServer()
        {
            try
            {
                int port = 27416;

                using (WebServer server = new WebServer(_webSocketServerFactory/*, _loggerFactory*/))
                {
                    await server.Listen(port);
                    Console.WriteLine($"Listening on port {port}");
                    Console.WriteLine("Press any key to quit");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
