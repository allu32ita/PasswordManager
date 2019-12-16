using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models.Exceptions;

namespace PasswordManager.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult index()
        {
            var Var_DettErrore = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            switch (Var_DettErrore.Error)
            {
                case PasswordNotFoundException exc:
                    ViewData["Title"] = "Password non trovata.";
                    Response.StatusCode = 404;
                    return View("PasswordNotFound");

                default:
                    ViewData["Title"] = "Errore";
                    return View();
            }

            
        }
    }
}