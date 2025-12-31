using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
