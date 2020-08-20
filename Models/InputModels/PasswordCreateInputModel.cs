using System.ComponentModel.DataAnnotations;
namespace PasswordManager.Models.InputModels
{
    //le System.ComponentModel.DataAnnotations definisco le regole di validazione se non conformi
    //creano nel registro ModelState un errore di validazione per quella proprieta
    public class PasswordCreateInputModel
    {
        [Required, MinLength(2), MaxLength(2000)]
        public string Descrizione {get; set; }
    }
}