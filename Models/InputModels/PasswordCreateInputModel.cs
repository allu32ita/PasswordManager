using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Controllers;

namespace PasswordManager.Models.InputModels
{
    //le System.ComponentModel.DataAnnotations definisco le regole di validazione se non conformi
    //creano nel registro ModelState un errore di validazione per quella proprieta
    public class PasswordCreateInputModel
    {
        [Required(ErrorMessage = "La Descrizione e' obbligatoria"), 
        MinLength(2, ErrorMessage = "La Descrizione dev'essere di almeno {1} caratteri"), 
        MaxLength(2000, ErrorMessage = "La Descrizione dev'essere di al massimo {1} caratteri"),
        Remote(action: nameof(PasswordController.DescrizioneDuplicata), controller: "Password", ErrorMessage = "La Password esiste gia")]
        public string Descrizione {get; set; }
    }
}