using System.Collections.Generic;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Models.Services.Application
{
    public interface IPasswordService
    {
         List<PasswordViewModel> GetPasswords();
         PasswordDetailViewModel GetPassword(string id);
    }
}