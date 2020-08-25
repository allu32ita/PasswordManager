using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models.Exceptions;
using PasswordManager.Models.InputModels;
using PasswordManager.Models.Services.Application;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Controllers
{
    public class PasswordController : Controller
    {
        private readonly IPasswordService prop_PasswordService;
        public PasswordController(ICachedPasswordService var_PasswordService)
        {
            this.prop_PasswordService = var_PasswordService;
        }
        public async Task<IActionResult> index(PasswordListInputModel input)
        {
            ListViewModel<PasswordViewModel> passwords = await prop_PasswordService.GetPasswordsAsync(input);

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
            PasswordDetailViewModel Pass = await prop_PasswordService.GetPasswordAsync(id);
            ViewData["Title"] = "Dettaglio numero " + Pass.Id.ToString();
            return View(Pass);
        }

        //[HttpGet]  per identificare in modo esplicito che il Create() e di visualizzazione della view
        public IActionResult Create()
        {
            ViewData["Descrizione"] = "Nuova Password";
            var var_InputModel = new PasswordCreateInputModel();
            return View(var_InputModel);
        }

        [HttpPost]  //per identificare in modo esplicito che il Create(PasswordCreateInputModel Par_InputModel) e di reperimento info da bottone post nella view
        public async Task<IActionResult> Create(PasswordCreateInputModel par_InputModel)
        {
            if (ModelState.IsValid == true)
            {
                try
                {
                    PasswordDetailViewModel var_Password = await prop_PasswordService.CreatePasswordAsync(par_InputModel); 
                    return RedirectToAction(nameof(Index));
                }
                catch (PasswordDescrizioneDuplicataException)
                {
                    ModelState.AddModelError(nameof(PasswordDetailViewModel.Descrizione), "Questa Password gia esiste");
                }
                
            }
            ViewData["Descrizione"] = "Nuova Password";
            return View(par_InputModel); 
        }

        public async Task<IActionResult> DescrizioneDuplicata(string Descrizione)
        {
            bool var_Result = await prop_PasswordService.DescrizioneDuplicataAsync(Descrizione);
            return Json(var_Result);
        }

        public async Task<IActionResult> Edit(string Id)
        {
            ViewData["Title"] = "Modifica Password";
            PasswordEditInputModel var_InputModel = await prop_PasswordService.GetPasswordForEditingAsync(Convert.ToInt32(Id));
            return View(var_InputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PasswordEditInputModel par_InputModel)
        {
            if (ModelState.IsValid == true)
            {

                try
                {
                    PasswordDetailViewModel var_Password = await prop_PasswordService.EditPasswordAsync(par_InputModel); 
                    return RedirectToAction(nameof(Index));
                }
                catch (PasswordDescrizioneDuplicataException)
                {
                    ModelState.AddModelError(nameof(PasswordDetailViewModel.Descrizione), "Questa Password gia esiste");
                }
            }
            ViewData["Title"] = "Modifica Password";
            return View(par_InputModel);
        }

    }
}