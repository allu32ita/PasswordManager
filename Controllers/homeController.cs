using Microsoft.AspNetCore.Mvc;

namespace PasswordManager.Controllers
{
    //attributo ha effetto su tutte le action del controller
    //[ResponseCache(CacheProfileName = "Home")]
    public class homeController : Controller
    {
        [ResponseCache(CacheProfileName = "Home")]
        public IActionResult index()
        {
            ViewData["Title"] = "Home";
            return View();
        }
    }
}