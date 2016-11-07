namespace Yolo.Dog.Website.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.Extensions.Logging;
    using Middleware;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Processes Json messages sent from <see cref="IJsonClient"/>s.
    /// </summary>
    public class JsonMessageServer
    {
        private readonly ILogger<JsonMessageServer> logger;
        private readonly IServiceProvider services;
        private readonly IControllerFactory factory;
        private readonly IAuthorizationService authorization;
        private readonly ConcurrentDictionary<IJsonClient, byte> connections;

        public JsonMessageServer(
            ILogger<JsonMessageServer> logger,
            IServiceProvider services,
            IControllerFactory factory,
            IAuthorizationService authorization)
        {
            this.logger = logger;
            this.services = services;
            this.factory = factory;
            this.authorization = authorization;
            this.connections = new ConcurrentDictionary<IJsonClient, byte>();
        }

        /// <summary>
        /// All currently connected <see cref="IJsonClient"/>s.
        /// </summary>
        public IEnumerable<IClient> Clients => this.connections.Keys;

        public async Task Send(object obj)
        {
            var message = JToken.FromObject(obj);
            var tasks = new List<Task>();
            foreach (var client in this.Clients)
            {
                tasks.Add(client.Send(message));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// Runs an asynchronous JSON message loop for a <see cref="IJsonClient"/>
        /// </summary>
        public async Task HandleClient(IJsonClient client)
        {
            try
            {
                this.connections.TryAdd(client, 0);
                while (!client.IsClosed)
                {
                    await this.HandleJSONMessageAsync(client);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Exception in RPC message loop: {ex.Message}");
            }
            finally
            {
                byte b;
                this.connections.TryRemove(client, out b);
            }
        }

        /// <summary>
        /// Asynchronously processes a JSON message sent from a <see cref="IJsonClient"/>
        /// </summary>
        private async Task HandleJSONMessageAsync(IJsonClient client)
        {
            var message = await client.Receive();
            var destination = message.Value<string>("Destination");
            if (destination != null)
            {
            }
        }
    }
}