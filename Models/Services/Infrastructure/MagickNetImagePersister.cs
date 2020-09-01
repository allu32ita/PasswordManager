using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PasswordManager.Models.Options;

namespace PasswordManager.Models.Services.Infrastructure
{
    public class MagickNetImagePersister : IImagePersister
    {
        private readonly IWebHostEnvironment par_WebHostEnvironment;
        private readonly IConfiguration configuration;

        private readonly SemaphoreSlim semaphore;
        public MagickNetImagePersister(IWebHostEnvironment par_WebHostEnvironment, IConfiguration configuration)
        {
            ResourceLimits.Height = 4000;    //dimensione massima di pixel dell'immagine MagickNet
            ResourceLimits.Width  = 4000;    //dimensione massima di pixel dell'immagine MagickNet
            semaphore = new SemaphoreSlim(2);
            this.configuration = configuration;
            this.par_WebHostEnvironment = par_WebHostEnvironment;
        }
        public async Task<string> SavePasswordFileAsync(int par_PasswordId, IFormFile par_FormFile)
        {
            string sExt = Path.GetExtension(par_FormFile.FileName);
            string sPath = $"/Passwords/{par_PasswordId}{sExt}";
            string sPhysicalPath = Path.Combine(par_WebHostEnvironment.WebRootPath, "Passwords", $"{par_PasswordId}{sExt}");

            using FileStream var_FileStream = File.OpenWrite(sPhysicalPath);
            await par_FormFile.CopyToAsync(var_FileStream);

            //restiture il percorso del file
            return sPath;
        }

        public async Task<string> SavePasswordImageAsync(int par_PasswordId, IFormFile par_FormFile)
        {
            await semaphore.WaitAsync();
            try {
                string sExt = Path.GetExtension(par_FormFile.FileName);
                string sPath = $"/Passwords/{par_PasswordId}{sExt}";
                string sPhysicalPath = Path.Combine(par_WebHostEnvironment.WebRootPath, "Passwords", $"{par_PasswordId}{sExt}");

                using Stream var_InputStream = par_FormFile.OpenReadStream();
                using MagickImage var_Image = new MagickImage(var_InputStream);

                //manipolare immagine
                int iHeightImage = configuration.GetSection("SizeImage").GetValue<int>("Height");
                int iWidthImage  = configuration.GetSection("SizeImage").GetValue<int>("Width");

                MagickGeometry var_MagickGeometry = new MagickGeometry(iWidthImage, iHeightImage);
                var_MagickGeometry.FillArea = true;
                var_Image.Resize(var_MagickGeometry);
                var_Image.Crop(iWidthImage, iHeightImage, Gravity.Northwest);
                var_Image.Quality = 70;
                var_Image.Write(sPhysicalPath, MagickFormat.Jpg);
                
                //restiture il percorso del file
                return sPath;
            } finally {
                semaphore.Release();
            }
            
        }
    }
}