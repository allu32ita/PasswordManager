using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models.InputModels;
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
        public async Task<IActionResult> index(PasswordListInputModel input)
        {
            List<PasswordViewModel> passwords = await ServizioPassword.GetPasswordsAsync(input);

            PasswordListViewModel viewModel = new PasswordListViewModel
            {
                Passwords = passwords,
                Input = input
            };
            
            ViewData["Title"] = "Lista Password";
            return View(viewModel);
        }

        public async Task<IActionResult> detail(string id)
        {
            PasswordDetailViewModel Pass = await ServizioPassword.GetPasswordAsync(id);
            ViewData["Title"] = "Dettaglio numero " + Pass.id.ToString();
            return View(Pass);
        }
    }
}