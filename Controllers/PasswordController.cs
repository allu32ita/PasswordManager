using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models.Services.Application;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Controllers
{
    public class PasswordController : Controller
    {
        public IActionResult index() 
        {
            var ListaPassword = new PasswordService();
            List<PasswordViewModel> Passwords = ListaPassword.GetPasswords();
            return View(Passwords);            
        }

        public IActionResult detail(string id)
        {
            var ListaPassword = new PasswordService();
            PasswordDetailViewModel Pass = ListaPassword.GetPassword(id);
            return View(Pass);
        }
    }
}