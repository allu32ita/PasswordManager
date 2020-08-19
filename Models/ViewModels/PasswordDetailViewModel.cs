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
                Id              = Convert.ToInt32(passRow["Id"]),
                Password        = passRow["Password"].ToString(),
                Descrizione     = passRow["Descrizione"].ToString(),
                DataInserimento = passRow["DataInserimento"].ToString(),
                FkUtente        = Convert.ToInt32(passRow["FkUtente"]),
                Sito            = passRow["Sito"].ToString(),
                Tipo            = passRow["Tipo"].ToString(),
                decrizioneEstesa = passRow["Descrizione"].ToString() + passRow["Sito"].ToString()
            };
            return PswdViewModelDetail;
        }
    }
}