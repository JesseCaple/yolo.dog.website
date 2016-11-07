namespace Yolo.Dog.Website.Services
{
    using System.Threading.Tasks;

    public interface IEmailValidator
    {
        bool IsValidEmail(string email);

        bool IsBannedEmailDomain(string email);
    }
}
