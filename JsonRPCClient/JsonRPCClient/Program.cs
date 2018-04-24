using JsonRPCShared;
using Ninja.WebSockets;
using StreamJsonRpc;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace JsonRPCClient
{
    class Program
    {
        static async Task RunClient()
        {
            var factory = new WebSocketClientFactory();
            var uri = new Uri("ws://localhost:27416/chat");
            using (WebSocket webSocket = await factory.ConnectAsync(uri))
            {
                var jsonRpc = new JsonRpc(new WebSocketMessageHandler(webSocket));
                jsonRpc.StartListening();

                EntityServiceResult result = await CampaginServiceGetCountRequest.MakeCall(jsonRpc);

                Console.WriteLine(result.Result.ToString());

                result = await CampaginServiceGetEntityRequest.MakeCall(jsonRpc, 0, 200, 1, "CampaignName");

                Console.WriteLine(result.Result.ToString());

                Console.ReadLine();

                // initiate the close handshake
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
        }

        static void Main(string[] args)
        {
            RunClient().Wait();
        }
    }
}
