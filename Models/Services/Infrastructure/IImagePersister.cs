using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PasswordManager.Models.Services.Infrastructure
{
    public interface IImagePersister
    {
        /// <returns> restituisce il percorso e nome file salvato es: /Password/file.ext </returns>
        Task<string> SavePasswordFileAsync(int par_PasswordId, IFormFile par_FormFile);

        Task<string> SavePasswordImageAsync(int par_PasswordId, IFormFile par_FormFile);
    }
}