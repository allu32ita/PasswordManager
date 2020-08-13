using System.Collections.Generic;

namespace PasswordManager.Models.ViewModels
{
    public class HomeViewModel : PasswordViewModel
    {
        public List<PasswordViewModel> List_UltimePassword { get; set; }        
    }
}