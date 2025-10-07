using Microsoft.AspNetCore.Mvc;

namespace ChatBotAplus.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
