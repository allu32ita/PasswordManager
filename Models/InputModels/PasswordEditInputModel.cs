using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Controllers;
using PasswordManager.Models.Entities;

namespace PasswordManager.Models.InputModels
{
    public class PasswordEditInputModel : IValidatableObject
    {   
        [Required]
        public int Id {get; set; }

        [Required(ErrorMessage = "La Password e' obbligatoria"),
        MinLength(2, ErrorMessage = "La Password dev'essere di almeno {1} caratteri"), 
        MaxLength(2000, ErrorMessage = "La Password dev'essere di al massimo {1} caratteri"),
        Display(Name = "Password in chiaro")]
        public string Password {get; set; }

        [Required(ErrorMessage = "La Descrizione e' obbligatoria"), 
        MinLength(2, ErrorMessage = "La Descrizione dev'essere di almeno {1} caratteri"), 
        MaxLength(2000, ErrorMessage = "La Descrizione dev'essere di al massimo {1} caratteri"),
        Remote(action: nameof(PasswordController.DescrizioneDuplicata), controller: "Password", ErrorMessage = "La Password esiste gia", AdditionalFields = "Id"),
        Display(Name = "Descrizione")]
        public string Descrizione {get; set; }

        [Display(Name = "Data Inserimento")]
        public string DataInserimento {get; set; }

        [Display(Name = "Utente loggato")]
        public string FkUtente {get; set; } 

        [Display(Name = "Sito Web")]
        public string Sito {get; set; }

        [Display(Name = "Tipo")]
        public string Tipo {get; set; }

        [Display(Name = "File")]
        public string PathFile {get; set; }

        [Display(Name = "Nuovo File...")]
        public IFormFile FilePassword {get; set; }

        public int RowVersion {get; set; }

        //serve per fare validazioni complesse e restituire errori nel ModelState se non si vuole fare DataAnnotatio personalizzate
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Tipo == "")
            {
                yield return new ValidationResult("Il Tipo di password deve essere valorizzato", new [] {nameof(Tipo)});
            }
        }

        internal static PasswordEditInputModel FromDataRow(DataRow var_PasswordRow)
        {
            var var_PasswordEditInputModel = new PasswordEditInputModel();
            var_PasswordEditInputModel.Id           = Convert.ToInt32(var_PasswordRow["Id"]);
            var_PasswordEditInputModel.Password     = var_PasswordRow["Password"].ToString();
            var_PasswordEditInputModel.Descrizione  = var_PasswordRow["Descrizione"].ToString();
            var_PasswordEditInputModel.DataInserimento = var_PasswordRow["DataInserimento"].ToString();
            var_PasswordEditInputModel.FkUtente = var_PasswordRow["FkUtente"].ToString();    
            var_PasswordEditInputModel.Sito = var_PasswordRow["Sito"].ToString();
            var_PasswordEditInputModel.Tipo = var_PasswordRow["Tipo"].ToString();
            var_PasswordEditInputModel.PathFile = var_PasswordRow["PathFile"].ToString();
            var_PasswordEditInputModel.RowVersion = Convert.ToInt32(var_PasswordRow["RowVersion"]);
            return var_PasswordEditInputModel;
        }

        public static PasswordEditInputModel FromEntity(Passwords par_password)
        {
            var var_password = new PasswordEditInputModel();
            var_password.Id                 = par_password.Id;
            var_password.Password           = par_password.Password;
            var_password.Descrizione        = par_password.Descrizione;
            var_password.DataInserimento    = par_password.DataInserimento;
            var_password.FkUtente           = par_password.FkUtente;
            var_password.Sito               = par_password.Sito;
            var_password.Tipo               = par_password.Tipo;
            var_password.PathFile           = par_password.PathFile;
            var_password.RowVersion         = par_password.RowVersion;
            return var_password;
        }
    }
}