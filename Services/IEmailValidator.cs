using System.Threading.Tasks;

namespace yolo.dog.website.Services
{
    public interface IEmailValidator
    {
        bool IsValidEmail(string email);
        bool IsBannedEmailDomain(string email);
    }
}
