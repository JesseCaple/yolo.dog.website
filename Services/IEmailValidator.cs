using System.Threading.Tasks;

namespace TubesWebsite.Services
{
    public interface IEmailValidator
    {
        bool IsValidEmail(string email);
        bool IsBannedEmailDomain(string email);
    }
}
