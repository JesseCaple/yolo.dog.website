namespace Yolo.Dog.Website.Middleware
{
    using System.Net.WebSockets;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using Services;

    /// <summary>
    /// Creates <see cref="IJsonClient"/>s from <see cref="WebSocket"/>
    /// connections and routes them to <see cref="JsonMessageServer"/>
    /// </summary>
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<WebSocketMiddleware> logger;
        private readonly JsonMessageServer server;

        public WebSocketMiddleware(
            RequestDelegate next,
            ILogger<WebSocketMiddleware> logger,
            JsonMessageServer server)
        {
            this.next = next;
            this.logger = logger;
            this.server = server;
        }

        // called by ASP.NET for every request
        public async Task Invoke(HttpContext context)
        {
            // see if we want to handle this request
            if (!context.WebSockets.IsWebSocketRequest
                || context.Request.Path != "/YoloWebSocket/")
            {
                await this.next.Invoke(context);
                return;
            }

            // connect socket and pass off to RPC server
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var client = new WebsocketClient(context, socket);
            await this.server.HandleClient(client);
        }

        // result of reading from a WebSocket
        private class WebSocketReadResult
        {
            public WebSocketMessageType Type { get; set; }

            public JObject JSON { get; set; }

            public byte[] Data { get; set; }
        }
    }
}
