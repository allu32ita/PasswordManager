using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models.Services.Application;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Controllers
{
    public class PasswordController : Controller
    {
        private readonly IPasswordService ServizioPassword;
        public PasswordController(IPasswordService ServizioPassword)
        {
            this.ServizioPassword = ServizioPassword;

        }
        public IActionResult index()
        {
            List<PasswordViewModel> Passwords = ServizioPassword.GetPasswords();
            ViewData["Title"] = "Lista Password";
            return View(Passwords);
        }

        public IActionResult detail(string id)
        {
            PasswordDetailViewModel Pass = ServizioPassword.GetPassword(id);
            ViewData["Title"] = "Dettaglio numero " + Pass.id.ToString();
            return View(Pass);
        }
    }
}