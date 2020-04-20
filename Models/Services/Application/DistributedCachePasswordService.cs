using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PasswordManager.Models.InputModels;
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

        public async Task<List<PasswordViewModel>> GetPasswordsAsync(PasswordListInputModel model)
        {
            bool canCache = model.Page <= 5 && string.IsNullOrEmpty(model.Search);
            if (canCache == true)
            {
                string key = $"Passwords{model.Page} = {model.Orderby} = {model.Ascending}";
                string serializedObject = await distributedCache.GetStringAsync(key);
                if (serializedObject != null) {
                    return Deserialize<List<PasswordViewModel>>(serializedObject);
                }
                List<PasswordViewModel> listpsw = await passwordService.GetPasswordsAsync(model);
                serializedObject = Serialize(listpsw);

                var cacheOptions = new DistributedCacheEntryOptions();
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(1));

                await distributedCache.SetStringAsync(key, serializedObject, cacheOptions);
                return listpsw;
            }
            else
            {
                List<PasswordViewModel> listpsw = await passwordService.GetPasswordsAsync(model);    
                return listpsw;
            }
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