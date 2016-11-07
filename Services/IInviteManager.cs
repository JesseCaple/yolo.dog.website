namespace Yolo.Dog.Website.Services
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public enum InviteResult
    {
        Success,
        NoInvites,
        LimitReached,
        InvalidEmail,
        BannedEmailDomain,
        EmailInUse
    }

    public interface IInviteManager
    {
        Task<InviteResult> SendAsync(ApplicationUser user, string email, ActionContext context);

        Task<Invite> FindByCodeAsync(string inviteCode);

        Task ClaimAsync(ApplicationUser newUser, Invite invite);
    }
}
