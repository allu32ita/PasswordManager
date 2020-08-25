using System;
using System.Data;
using PasswordManager.Models.Entities;

namespace PasswordManager.Models.ViewModels
{
    public class PasswordViewModel
    {
        public int Id {get; set;}

        public string Password {get; set;}

        public string Descrizione {get; set;}

        public string DataInserimento {get; set;}

        public int FkUtente {get; set;}

        public string Sito {get;set;}

        public string Tipo {get; set;}

        public string PathFile {get; set; }

        public static PasswordViewModel FromDataRow(DataRow passRow)
        {
            var PswdViewModel = new PasswordViewModel {
                Id              = Convert.ToInt32(passRow["Id"]),
                Descrizione     = passRow["Descrizione"].ToString(),
                Password        = passRow["Password"].ToString(),
                Sito            = passRow["Password"].ToString(),
                Tipo            = passRow["Tipo"].ToString(),
                PathFile        = passRow["PathFile"].ToString()
            };
            return PswdViewModel;
        }
    }
}