using Microsoft.AspNetCore.Mvc;

namespace PasswordManager.Controllers
{
    public class homeController : Controller
    {
        public IActionResult index()
        {
            ViewData["Title"] = "Home";
            return View();
        }
    }
}