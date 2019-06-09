using Microsoft.AspNetCore.Mvc;

namespace PasswordManager.Controllers
{
    public class PasswordController : Controller
    {
        public IActionResult index () 
        {
            return View();            
        }

        public IActionResult detail ()
        {
            return View();
        }
    }
}