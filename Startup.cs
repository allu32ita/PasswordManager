using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using PasswordManager.Models.Services.Application;
using PasswordManager.Models.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PasswordManager.Models.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Identity;
using PasswordManager.Customizations.Identity;

namespace PasswordManager
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddRazorPages();
            services.AddMvc(options => {
                var HomeProfile = new CacheProfile();
                //HomeProfile.Duration = Configuration.GetValue<int>("ResponseCache:Home:Duration");
                //HomeProfile.Location = Configuration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");
                //HomeProfile.VaryByQueryKeys = new string[] {"page"};
                Configuration.Bind("ResponseCache:Home", HomeProfile);
                options.CacheProfiles.Add("Home", HomeProfile);
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            
            //ado net
            //services.AddTransient<IPasswordService, AdoNetPasswordService>();
            //services.AddTransient<IDatabaseAccessor, SqLiteDatabaseAccessor>();

            //ef core
            services.AddDefaultIdentity<IdentityUser>(var_Options => {
                var_Options.Password.RequireDigit           = true;
                var_Options.Password.RequiredLength         = 8;
                var_Options.Password.RequireUppercase       = true;
                var_Options.Password.RequireLowercase       = true;
                var_Options.Password.RequireNonAlphanumeric = true;
                var_Options.Password.RequiredUniqueChars    = 4;
            })
            .AddPasswordValidator<CommonPasswordValidator<IdentityUser>>()
            .AddEntityFrameworkStores<PasswordDbContext>();

            services.AddTransient<IPasswordService, EFCorePasswordService>();
            services.AddDbContextPool<PasswordDbContext>(optionsBuilder => {
                String ConnectionString = Configuration.GetSection("ConnectionStrings").GetValue<String>("Default");
                optionsBuilder.UseSqlite(ConnectionString);
            });

            services.AddTransient<ICachedPasswordService, MemoryCachedPasswordService>(); 
            services.AddSingleton<IImagePersister, MagickNetImagePersister>();
            
            //Options
            services.Configure<MemoryCacheOptions>(Configuration.GetSection("MemoryCache"));
            services.Configure<PasswordsOptions>(Configuration.GetSection("Passwords"));
            services.Configure<ConnectionStringsOptions>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<KestrelServerOptions>(Configuration.GetSection("Kestrel"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
                lifetime.ApplicationStarted.Register(() => {
                    string filepath = Path.Combine(env.ContentRootPath, "bin/reload.txt");
                    File.WriteAllText(filepath, DateTime.Now.ToString());
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            //endpoint routing middleware
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseCaching();

            //usare endpoint middleware
            app.UseEndpoints(routeBuilder => {
                routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routeBuilder.MapRazorPages();
            });
        }
    }
}
