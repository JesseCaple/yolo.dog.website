namespace Yolo.Dog.Website.Services
{
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    public interface IMessageHandler
    {
        Task Handle(JObject message);
    }
}
