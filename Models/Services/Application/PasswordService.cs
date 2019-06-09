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
        public PasswordDetailViewModel GetPassword(string id)
        {
            var pass = new PasswordDetailViewModel();
            pass.id = Convert.ToInt32(id);
            pass.decrizioneEstesa = "descrizione estesa del id " + id;
            pass.descrizione = "Password numero " + id;
            pass.sito = "Sito numero " + id;
            pass.tipo = "Pass";   
            return pass;
        }
    }
}