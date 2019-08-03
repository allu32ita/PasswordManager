using System;
using System.Data;

namespace PasswordManager.Models.ViewModels
{
    public class PasswordDetailViewModel : PasswordViewModel
    {
        public string decrizioneEstesa { get; set; }

        public new static PasswordDetailViewModel FromDataRow(DataRow passRow)
        {
            var PswdViewModelDetail = new PasswordDetailViewModel {
                id              = Convert.ToInt32(passRow["Id"]),
                descrizione     = passRow["Descrizione"].ToString(),
                Password        = passRow["Password"].ToString(),
                sito            = passRow["Sito"].ToString(),
                tipo            = passRow["Tipo"].ToString(),
                decrizioneEstesa = passRow["Descrizione"].ToString() + passRow["Sito"].ToString()
            };
            return PswdViewModelDetail;
        }
    }
}