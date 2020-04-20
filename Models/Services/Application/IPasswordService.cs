using System.Collections.Generic;
using System.Threading.Tasks;
using PasswordManager.Models.InputModels;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Models.Services.Application
{
    public interface IPasswordService
    {
         Task<List<PasswordViewModel>> GetPasswordsAsync(PasswordListInputModel model);
         Task<PasswordDetailViewModel> GetPasswordAsync(string id);
    }
}