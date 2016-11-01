using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using yolo.dog.website.Services;
using yolo.dog.website.Models;
using Microsoft.AspNetCore.Identity;
using mRPC;
using NUglify.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace yolo.dog.website.Controllers
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

        public struct TestStuct
        {
            public string value1;
            public string value2;
        }

        [RPC]
        [NonAction]
        public string Test(TestStuct test)
        {
            return $"You gave me: {test.value1} and {test.value2}";
        }


        [RPC]
        [NonAction]
        public string Test2(int number)
        {
            return $"You gave me: {number}";
        }
    }
}
