using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models.Services.Application;

namespace PasswordManager.Controllers
{
    public class PasswordController : Controller
    {
        public IActionResult index () 
        {
            var ListaPassword = new PasswordService();
            ListaPassword.GetPassword();
            return View();            
        }

        public IActionResult detail ()
        {
            return View();
        }
    }
}