using System;
using System.Data;
using PasswordManager.Models.Entities;

namespace PasswordManager.Models.ViewModels
{
    public class PasswordViewModel
    {
        public int id {get; set;}

        public string descrizione {get; set;}

        public string Password {get; set;}

        public string sito {get;set;}

        public string tipo {get; set;}

        public static PasswordViewModel FromDataRow(DataRow passRow)
        {
            var PswdViewModel = new PasswordViewModel {
                id              = Convert.ToInt32(passRow["Id"]),
                descrizione     = passRow["Descrizione"].ToString(),
                Password        = passRow["Password"].ToString(),
                sito            = passRow["Password"].ToString(),
                tipo            = passRow["Tipo"].ToString()
            };
            return PswdViewModel;
        }
    }
}