using System;
using System.Data;

namespace PasswordManager.Models.ViewModels
{
    public class PasswordDetailViewModel : PasswordViewModel
    {
        public string decrizioneEstesa { get; set; }

        public new static PasswordDetailViewModel FromDataRow(DataRow passRow)
        {
            var var_PasswordDetail = new PasswordDetailViewModel();
            var_PasswordDetail.Id              = Convert.ToInt32(passRow["Id"]);
            var_PasswordDetail.Password        = passRow["Password"].ToString();
            var_PasswordDetail.Descrizione     = Convert.ToString(passRow["Descrizione"]);
            var_PasswordDetail.DataInserimento = passRow["DataInserimento"].ToString();
            if (passRow["FkUtente"].ToString() != "")    //va in errore perche non riesce a convertire un DataRow con valore nullo in intero
            {
                var_PasswordDetail.FkUtente        = Convert.ToInt32(passRow["FkUtente"]);    
            }
            var_PasswordDetail.Sito            = passRow["Sito"].ToString();
            var_PasswordDetail.Tipo            = passRow["Tipo"].ToString();
            var_PasswordDetail.decrizioneEstesa = passRow["Descrizione"].ToString() + passRow["Sito"].ToString();
            
            return var_PasswordDetail;
        }
    }
}