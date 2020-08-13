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
        private readonly IPasswordService passwordService;
        private readonly IMemoryCache memoryChache;

        public MemoryCachedPasswordService(IPasswordService passwordService, IMemoryCache memoryChache)
        {
            this.memoryChache = memoryChache;
            this.passwordService = passwordService;
        }

        public Task<PasswordDetailViewModel> GetPasswordAsync(string id)
        {
            return memoryChache.GetOrCreateAsync($"Password {id}", cacheEntry => {
                cacheEntry.SetSize(1);
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return passwordService.GetPasswordAsync(id);
            });
        }

        public Task<ListViewModel<PasswordViewModel>> GetPasswordsAsync(PasswordListInputModel model)
        {
            bool canCache = model.Page <= 5 && string.IsNullOrEmpty(model.Search);
            if (canCache)
            {
                return memoryChache.GetOrCreateAsync($"Passwords{model.Page} = {model.Orderby} = {model.Ascending}", cacheEntry => {
                    cacheEntry.SetSize(2);
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(1));
                    return passwordService.GetPasswordsAsync(model);
                });
            }
            return passwordService.GetPasswordsAsync(model);
        }

        public Task<List<PasswordViewModel>> GetListUltimePasswordAsync()
        {
            return memoryChache.GetOrCreateAsync($"UltimePassword", cacheEntry => {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(1));
                return passwordService.GetListUltimePasswordAsync();
            });
        }
    }
}