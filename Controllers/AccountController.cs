namespace Yolo.Dog.Website.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models;
    using Models.AccountViewModels;
    using Services;

    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IInviteManager inviteManager;
        private readonly IEmailSender emailSender;
        private readonly ILogger logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IInviteManager inviteManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.inviteManager = inviteManager;
            this.emailSender = emailSender;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;
            if (this.ModelState.IsValid)
            {
                var result = await this.signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    this.logger.LogInformation(1, "User logged in.");
                    if (this.Url.IsLocalUrl(returnUrl))
                    {
                        return this.Redirect(returnUrl);
                    }
                    else
                    {
                        return this.RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }

                if (result.IsLockedOut)
                {
                    this.logger.LogWarning(2, "User account locked out.");
                    return this.View("Lockout");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.View(model);
                }
            }

            return this.View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string inviteCode = null)
        {
            if (inviteCode == null)
            {
                return new ChallengeResult();
            }

            Invite invite = await this.inviteManager.FindByCodeAsync(inviteCode);
            if (invite == null)
            {
                return this.View("InviteExpired");
            }

            RegisterViewModel model = new RegisterViewModel
            {
                InviteCode = inviteCode
            };

            return this.View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                Invite invite = await this.inviteManager.FindByCodeAsync(model.InviteCode);
                if (invite == null)
                {
                    return this.View("InviteExpired");
                }

                var user = new ApplicationUser { UserName = model.UserName, Email = invite.Email };
                var result = await this.userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await this.inviteManager.ClaimAsync(user, invite);
                    var emailToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    await this.userManager.ConfirmEmailAsync(user, emailToken);
                    this.logger.LogInformation(3, "User account created.");
                    await this.signInManager.SignInAsync(user, isPersistent: false);
                    this.logger.LogInformation(1, "User logged in.");
                    return this.RedirectToAction(nameof(HomeController.Index), "Home");
                }

                this.AddErrors(result);
            }

            return this.View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult InviteExpired()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await this.signInManager.SignOutAsync();
            this.logger.LogInformation(4, "User logged out.");
            return this.RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await this.userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return this.View("ForgotPasswordConfirmation");
                }

                var code = await this.userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = this.Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: this.HttpContext.Request.Scheme);
                await this.emailSender.SendEmailAsync(
                    model.Email,
                    "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return this.View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? this.View("Error") : this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return this.RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }

            var result = await this.userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return this.RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }

            this.AddErrors(result);
            return this.View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return this.View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return this.userManager.GetUserAsync(this.HttpContext.User);
        }
    }
}
