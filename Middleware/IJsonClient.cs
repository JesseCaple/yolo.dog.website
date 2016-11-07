namespace Yolo.Dog.Website.Middleware
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A persistent connection to the server that can send and receive Json messages.
    /// </summary>
    public interface IJsonClient : IClient
    {
        /// <summary>
        /// Retrieves the value indicating wither the underlying connection is closed.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// Sends a Json message to the underlying connection.
        /// </summary>
        Task Send(JObject obj);

        /// <summary>
        /// Asynchronously retrieves a Json message from the underlying connection.
        /// </summary>
        /// <returns></returns>
        Task<JObject> Receive();
    }
}
