using System.ComponentModel.DataAnnotations;

namespace PasswordManager.Models.InputModels
{
    public class PasswordDeleteInputModel
    {
        [Required]
        public int Id {get; set; }

    }
}