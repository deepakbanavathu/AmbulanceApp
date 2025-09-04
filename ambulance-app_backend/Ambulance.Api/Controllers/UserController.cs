using Microsoft.AspNetCore.Mvc;

namespace Ambulance.Api.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
