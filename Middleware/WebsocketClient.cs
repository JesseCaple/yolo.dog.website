namespace Yolo.Dog.Website.Middleware
{
    using System;
    using System.IO;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A <see cref="WebSocket"/> connection implementing <see cref="IJsonClient"/>.
    /// </summary>
    public class WebsocketClient : IJsonClient
    {
        public WebsocketClient(HttpContext context, WebSocket webSocket)
        {
            this.Context = context;
            this.Socket = webSocket;
        }

        public WebSocket Socket { get; }

        public HttpContext Context { get; }

        /// <summary>
        /// Checks if the <see cref="WebSocket"/> is still open
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return this.Socket.State != WebSocketState.Open;
            }
        }

        /// <summary>
        /// Sends a Json message to the <see cref="WebSocket"/>.
        /// </summary>
        public async Task Send(JObject message)
        {
            var bytes = Encoding.UTF8.GetBytes(message.ToString());
            var buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await this.Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Converts the object to a Json message and sends it to the <see cref="WebSocket"/>.
        /// </summary>
        public async Task Send(object obj)
        {
            var message = JToken.FromObject(obj);
            var bytes = Encoding.UTF8.GetBytes(message.ToString());
            var buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await this.Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Retrieves a JSON message from the <see cref="WebSocket"/>.
        /// </summary>
        public async Task<JObject> Receive()
        {
            var message = new JObject();
            var buffer = new ArraySegment<byte>(new byte[4096]);
            using (var stream = new MemoryStream())
            {
                WebSocketReceiveResult recResult;
                do
                {
                    recResult = await this.Socket.ReceiveAsync(buffer, CancellationToken.None);
                    stream.Write(buffer.Array, buffer.Offset, recResult.Count);
                }
                while (!recResult.EndOfMessage);
                stream.Seek(0, SeekOrigin.Begin);

                if (recResult.MessageType == WebSocketMessageType.Text)
                {
                    var array = stream.ToArray();
                    var asString = Encoding.UTF8.GetString(array, 0, array.Length);
                    message = JObject.Parse(asString);
                }
            }

            return message;
        }
    }
}
