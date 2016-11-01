using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TubesWebsite.Models;

namespace TubesWebsite.Services
{

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
