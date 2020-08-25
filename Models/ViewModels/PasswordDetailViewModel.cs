using System;
using System.Data;
using PasswordManager.Models.Entities;

namespace PasswordManager.Models.ViewModels
{
    public class PasswordDetailViewModel : PasswordViewModel
    {
        public string decrizioneEstesa { get; set; }

        public static new PasswordDetailViewModel FromDataRow(DataRow passRow)
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
            var_PasswordDetail.PathFile        = passRow["PathFile"].ToString(); 
            
            return var_PasswordDetail;
        }

        public static PasswordDetailViewModel FromEntity(Passwords par_Password)
        {
            var var_PasswordDetailViewModel = new PasswordDetailViewModel();
            var_PasswordDetailViewModel.Id              = par_Password.Id;
            var_PasswordDetailViewModel.Password        = par_Password.Password;
            var_PasswordDetailViewModel.Descrizione     = par_Password.Descrizione;
            var_PasswordDetailViewModel.DataInserimento = par_Password.DataInserimento;
            var_PasswordDetailViewModel.FkUtente        = par_Password.FkUtente;
            var_PasswordDetailViewModel.Sito            = par_Password.Sito;
            var_PasswordDetailViewModel.Tipo            = par_Password.Tipo;
            var_PasswordDetailViewModel.PathFile        = par_Password.PathFile;
            return var_PasswordDetailViewModel;
        }
    }
}