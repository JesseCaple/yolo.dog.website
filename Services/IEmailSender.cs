using System.Threading.Tasks;

namespace yolo.dog.website.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
