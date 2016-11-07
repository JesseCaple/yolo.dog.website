namespace Yolo.Dog.Website.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;

    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly JsonMessageServer server;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            JsonMessageServer server)
        {
            this.userManager = userManager;
            this.server = server;
        }

        public async Task<IActionResult> Index()
        {
            await this.server.Send(new { text = "hey" });
            return this.View();
        }

        public IActionResult Error()
        {
            return this.View();
        }
    }
}
