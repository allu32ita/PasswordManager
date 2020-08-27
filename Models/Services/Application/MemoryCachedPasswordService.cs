using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PasswordManager.Models.InputModels;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Models.Services.Application
{
    public class MemoryCachedPasswordService : ICachedPasswordService
    {
        private readonly IPasswordService prop_PasswordService;
        private readonly IMemoryCache memoryChache;

        public MemoryCachedPasswordService(IPasswordService var_PasswordService, IMemoryCache memoryChache)
        {
            this.memoryChache = memoryChache;
            this.prop_PasswordService = var_PasswordService;
        }

        public Task<PasswordDetailViewModel> GetPasswordAsync(string id)
        {
            return memoryChache.GetOrCreateAsync($"Password{id}", cacheEntry => {
                cacheEntry.SetSize(1);
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return prop_PasswordService.GetPasswordAsync(id);
            });
        }

        public Task<ListViewModel<PasswordViewModel>> GetPasswordsAsync(PasswordListInputModel model)
        {
            bool canCache = model.Page <= 5 && string.IsNullOrEmpty(model.Search);
            if (canCache)
            {
                return memoryChache.GetOrCreateAsync($"Passwords{model.Page} = {model.Orderby} = {model.Ascending}", cacheEntry => {
                    cacheEntry.SetSize(2);
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                    return prop_PasswordService.GetPasswordsAsync(model);
                });
            }
            return prop_PasswordService.GetPasswordsAsync(model);
        }

        public Task<List<PasswordViewModel>> GetListUltimePasswordAsync()
        {
            return memoryChache.GetOrCreateAsync($"UltimePassword", cacheEntry => {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return prop_PasswordService.GetListUltimePasswordAsync();
            });
        }

        public Task<PasswordDetailViewModel> CreatePasswordAsync(PasswordCreateInputModel par_InputModel)
        {
            return prop_PasswordService.CreatePasswordAsync(par_InputModel);
        }

        public Task<bool> DescrizioneDuplicataAsync(string par_Descrizione, int par_Id)
        {
            return prop_PasswordService.DescrizioneDuplicataAsync(par_Descrizione, par_Id);
        }

        public Task<PasswordEditInputModel> GetPasswordForEditingAsync(int id)
        {
            return prop_PasswordService.GetPasswordForEditingAsync(id);
        }

        public async Task<PasswordDetailViewModel> EditPasswordAsync(PasswordEditInputModel par_InputModel)
        {
            PasswordDetailViewModel var_Password = await prop_PasswordService.EditPasswordAsync(par_InputModel);
            memoryChache.Remove($"Password{par_InputModel.Id}");
            return var_Password;
        }
    }
}