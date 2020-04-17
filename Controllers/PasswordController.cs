using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models.Services.Application;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Controllers
{
    public class PasswordController : Controller
    {
        private readonly IPasswordService ServizioPassword;
        public PasswordController(ICachedPasswordService ServizioPassword)
        {
            this.ServizioPassword = ServizioPassword;
        }
        public async Task<IActionResult> index(string search, int page, string orderby, bool ascending)
        {
            List<PasswordViewModel> Passwords = await ServizioPassword.GetPasswordsAsync(search, page, orderby, ascending);
            ViewData["Title"] = "Lista Password";
            return View(Passwords);
        }

        public async Task<IActionResult> detail(string id)
        {
            PasswordDetailViewModel Pass = await ServizioPassword.GetPasswordAsync(id);
            ViewData["Title"] = "Dettaglio numero " + Pass.id.ToString();
            return View(Pass);
        }
    }
}