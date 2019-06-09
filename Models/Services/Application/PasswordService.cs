using System;
using System.Collections.Generic;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Models.Services.Application
{
    public class PasswordService
    {
        public List<PasswordViewModel> GetPasswords()
        {
            var ListaPass = new List<PasswordViewModel>();
            for(int i = 1; i <= 20; i++)
            {
                var pass = new PasswordViewModel();
                pass.id = i;
                pass.descrizione = "Password numero " + i.ToString();
                pass.sito = "Sito numero " + i.ToString();
                pass.tipo = "Pass";        
                ListaPass.Add(pass);
            }
            return ListaPass;
        }
    }
}