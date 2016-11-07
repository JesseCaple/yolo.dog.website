namespace Yolo.Dog.Website.Services
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class InviteManager : IInviteManager
    {
        private readonly ApplicationDbContext database;
        private readonly IEmailSender emailSender;
        private readonly IEmailValidator emailValidator;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHostingEnvironment env;

        public InviteManager(
            ApplicationDbContext database,
            IEmailSender emailSender,
            IEmailValidator emailValidator,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment env)
        {
            this.database = database;
            this.emailSender = emailSender;
            this.emailValidator = emailValidator;
            this.userManager = userManager;
            this.env = env;
        }

        public async Task<InviteResult> SendAsync(ApplicationUser user, string email, ActionContext context)
        {
            // validation
            if (user == null || email == null || context == null)
            {
                throw new ArgumentNullException();
            }

            if (user.InvitesClaimed >= user.InvitesAwarded)
            {
                return InviteResult.NoInvites;
            }

            var invites = this.database.Invites.Where(e => e.User.Id == user.Id);
            foreach (var i in invites)
            {
                if (i.Expires > DateTime.UtcNow)
                {
                    this.database.Remove(i);
                }

                await this.database.SaveChangesAsync();
            }

            if (invites.Count() >= 25)
            {
                return InviteResult.LimitReached;
            }

            if (!this.emailValidator.IsValidEmail(email))
            {
                return InviteResult.InvalidEmail;
            }

            if (this.emailValidator.IsBannedEmailDomain(email))
            {
                return InviteResult.BannedEmailDomain;
            }

            if (await this.userManager.FindByEmailAsync(email) != null)
            {
                return InviteResult.EmailInUse;
            }

            // generate random salt & token
            byte[] saltBytes = new byte[16];
            byte[] tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
                rng.GetBytes(tokenBytes);
            }

            // encode and combine them to create the invite code
            var salt = WebEncoders.Base64UrlEncode(saltBytes);
            var token = WebEncoders.Base64UrlEncode(tokenBytes);
            var inviteCode = salt + "." + token;

            // hash the token
            var hash = await Task.Run(() =>
            {
                return KeyDerivation.Pbkdf2(
                    password: token,
                    salt: WebEncoders.Base64UrlDecode(salt),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8);
            });

            // create the invite
            var invite = new Invite
            {
                TokenHash = hash,
                Email = email,
                Expires = DateTime.UtcNow.AddDays(3),
                User = user
            };
            this.database.Invites.Add(invite);
            await this.database.SaveChangesAsync();

            // send invite to specified email address
            var callbackUrl = new UrlHelper(context).Action(
                "Register",
                "Account",
                new { inviteCode = inviteCode },
                protocol: context.HttpContext.Request.Scheme);
            string subject = $"{user.UserName} has invited you to TUBES.";
            string message = $@"
                <h1>{subject}</h1>
                <p>Click the following link to create an account.</p>
                <p><a href='{callbackUrl}'>{callbackUrl}</a></p>"
                .TrimMultiline();
            await this.emailSender.SendEmailAsync(email, subject, message);
            return InviteResult.Success;
        }

        public async Task<Invite> FindByCodeAsync(string inviteCode)
        {
            if (inviteCode == null)
            {
                return null;
            }

            // generate token hash from inviteCode
            var parts = inviteCode.Split('.');
            if (parts.Length != 2)
            {
                return null;
            }

            var salt = parts[0];
            var token = parts[1];
            var hash = await Task.Run(() =>
            {
                return KeyDerivation.Pbkdf2(
                    password: token,
                    salt: WebEncoders.Base64UrlDecode(salt),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8);
            });

            // get invite by hash if it exists
            var invite = this.database.Invites
                .Where(e => e.TokenHash.SequenceEqual(hash))
                .Include(e => e.User)
                .SingleOrDefault();

            // check that invite is still valid
            if (invite == null)
            {
                return null;
            }

            if (DateTime.UtcNow >= invite.Expires ||
                invite.User.InvitesAwarded <= invite.User.InvitesClaimed)
            {
                this.database.Remove(invite);
                await this.database.SaveChangesAsync();
                return null;
            }

            return invite;
        }

        public async Task ClaimAsync(ApplicationUser newUser, Invite invite)
        {
            invite.User.InvitesClaimed++;
            newUser.InvitedBy = invite.User;
            this.database.Invites.Remove(invite);
            await this.database.SaveChangesAsync();
        }
    }
}
