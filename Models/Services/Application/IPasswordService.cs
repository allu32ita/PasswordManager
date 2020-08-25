using System.Collections.Generic;
using System.Threading.Tasks;
using PasswordManager.Models.InputModels;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Models.Services.Application
{
    public interface IPasswordService
    {
        Task<ListViewModel<PasswordViewModel>> GetPasswordsAsync(PasswordListInputModel model);
        Task<PasswordDetailViewModel> GetPasswordAsync(string id);
        Task<List<PasswordViewModel>> GetListUltimePasswordAsync();
        Task<PasswordDetailViewModel> CreatePasswordAsync(PasswordCreateInputModel par_InputModel);
        Task<PasswordEditInputModel> GetPasswordForEditingAsync(int id);
        Task<PasswordDetailViewModel> EditPasswordAsync(PasswordEditInputModel par_InputModel);
        Task<bool> DescrizioneDuplicataAsync(string par_Descrizione);
        
    }
}