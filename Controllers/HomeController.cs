using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TubesWebsite.Services;
using TubesWebsite.Models;
using Microsoft.AspNetCore.Identity;
using mRPC;
using NUglify.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace TubesWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IInviteManager _inviteManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RPCManager<HomeController> _tube;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            IInviteManager inviteManager,
            RPCManager<HomeController> tube)
        {
            _userManager = userManager;
            _inviteManager = inviteManager;
            _tube = tube;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task Error()
        {
            await _tube.Test("Jesse");
        }

        [RPC]
        [NonAction]
        public string Test(int i = 0)
        {
            return $"You gave me: {i}";
        }
    }
}
