using System.Collections.Generic;
using PasswordManager.Models.InputModels;

namespace PasswordManager.Models.ViewModels
{
    public class PasswordListViewModel
    {
        public List<PasswordViewModel> Passwords {get; set;}
        public PasswordListInputModel Input {get; set;} 
    }
}