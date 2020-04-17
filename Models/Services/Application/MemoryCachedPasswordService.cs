using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
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

        public Task<List<PasswordViewModel>> GetPasswordsAsync(string search, int page, string orderby, bool ascending)
        {
            return memoryChache.GetOrCreateAsync($"Passwords{search} = {page} = {orderby} = {ascending}", cacheEntry => {
                cacheEntry.SetSize(2);
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(1));
                return passwordService.GetPasswordsAsync(search, page, orderby, ascending);
            });
        }
    }
}