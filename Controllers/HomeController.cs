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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RPCManager<HomeController> _rpc;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            RPCManager<HomeController> rpc)
        {
            _userManager = userManager;
            _rpc = rpc;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[NonAction]
        public async Task Error()
        {
            await _rpc.DynamicRPC.Test(27, "Jesse");
        }

        public struct TestStuct
        {
            public int value1;
            public string value2;
        }

        [RPC]
        public void Test(TestStuct test)
        {
            _rpc.DynamicRPC.Test(test.value1, test.value2);
            foreach (var connection in _rpc.Connections)
            {
                connection.DynamicRPC.Test2();
            }
        }


        [RPC]
        public IActionResult Test2()
        {
            return NotFound();
        }
    }
}
