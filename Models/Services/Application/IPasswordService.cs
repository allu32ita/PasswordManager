using System.Collections.Generic;
using System.Threading.Tasks;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Models.Services.Application
{
    public interface IPasswordService
    {
         Task<List<PasswordViewModel>> GetPasswordsAsync();
         Task<PasswordDetailViewModel> GetPasswordAsync(string id);
    }
}