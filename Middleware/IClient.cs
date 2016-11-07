namespace Yolo.Dog.Website.Middleware
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// A persistent connection to the server that can receive generic messages.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Retrieves the <see cref="HttpContext"/> of the underlying connection.
        /// </summary>
        HttpContext Context { get; }

        /// <summary>
        /// Sends the given object to the underlying connection.
        /// </summary>
        Task Send(object obj);
    }
}
