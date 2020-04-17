using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Models.Services.Application
{
    public class DistributedCachePasswordService : ICachedPasswordService
    {
        private readonly IPasswordService passwordService;
        private readonly IDistributedCache distributedCache;
        public DistributedCachePasswordService(IPasswordService passwordService, IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
            this.passwordService = passwordService;
        }

        public async Task<PasswordDetailViewModel> GetPasswordAsync(string id)
        {
            string key = $"Password {id}";
            string serializedObject = await distributedCache.GetStringAsync(key);
            if (serializedObject != null) {
                return Deserialize<PasswordDetailViewModel>(serializedObject);
            }
            PasswordDetailViewModel psw = await passwordService.GetPasswordAsync(id);
            serializedObject = Serialize(psw);

            var cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

            await distributedCache.SetStringAsync(key, serializedObject, cacheOptions);
            return psw;
        }

        public async Task<List<PasswordViewModel>> GetPasswordsAsync(string search, int page, string orderby, bool ascending)
        {
            string key = $"Passwords{search} = {page} = {orderby} = {ascending}";
            string serializedObject = await distributedCache.GetStringAsync(key);
            if (serializedObject != null) {
                return Deserialize<List<PasswordViewModel>>(serializedObject);
            }
            List<PasswordViewModel> listpsw = await passwordService.GetPasswordsAsync(search, page, orderby, ascending);
            serializedObject = Serialize(listpsw);

            var cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(1));

            await distributedCache.SetStringAsync(key, serializedObject, cacheOptions);
            return listpsw;
        }

        private string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private T Deserialize<T>(string serializedObject)
        {
            return JsonConvert.DeserializeObject<T>(serializedObject);
        }
    }
}