using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models.Services.Application;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Controllers
{
    //attributo ha effetto su tutte le action del controller
    //[ResponseCache(CacheProfileName = "Home")]
    public class homeController : Controller
    {
        //[ResponseCache(CacheProfileName = "Home")]
        public async Task<IActionResult> Index([FromServices] ICachedPasswordService passwordService)
        {
            ViewData["Title"] = "Home";
            List<PasswordViewModel> List_UltimePassword = await passwordService.GetListUltimePasswordAsync();

            HomeViewModel View_Home = new HomeViewModel();
            View_Home.List_UltimePassword = List_UltimePassword;

            return View(View_Home);
        }
    }
}