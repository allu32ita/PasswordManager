using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PasswordManager
{
    
    public class Program
    {
        
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(Var_webHostBuilder => {
                Var_webHostBuilder.UseStartup<Startup>();
                //Var_webHostBuilder.UseUrls("http://aluigiandrea.ddns.net:5000"); per farlo andare su un host o ip diverso dal default che e localhost
            //.ConfigureServices  per configurare la DI di una applicazione console configurare i servizi qui e non ho piu la Startup.cs
            });
    }
}
