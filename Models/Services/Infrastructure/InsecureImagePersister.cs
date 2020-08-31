using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace PasswordManager.Models.Services.Infrastructure
{
    public class InsecureImagePersister : IImagePersister
    {
        private readonly IWebHostEnvironment par_Env;
        public InsecureImagePersister(IWebHostEnvironment par_Env)
        {
            this.par_Env = par_Env;

        }
        public async Task<string> SavePasswordFileAsync(int par_PasswordId, IFormFile par_FormFile)
        {
            //salvare file 
            string sExt = Path.GetExtension(par_FormFile.FileName);
            string sPath = $"/Passwords/{par_PasswordId}{sExt}";
            string sPhysicalPAth = Path.Combine(par_Env.WebRootPath, "Passwords", $"{par_PasswordId}{sExt}");
            using FileStream var_FileStream = File.OpenWrite(sPhysicalPAth);
            await par_FormFile.CopyToAsync(var_FileStream);

            //restiture il percorso del file
            return sPath;
        }
    }
}