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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddTransient<IPasswordService, AdoNetPasswordService>();
            services.AddTransient<IPasswordService, EFCorePasswordService>();
            //services.AddTransient<IDatabaseAccessor, SqLiteDatabaseAccessor>();

            //services.AddScoped<PasswordDbContext>();
            //services.AddDbContext<PasswordDbContext>();
            services.AddDbContextPool<PasswordDbContext>(optionsBuilder => 
            {
                String ConnectionString = Configuration.GetSection("ConnectionStrings").GetValue<String>("Default");
                optionsBuilder.UseSqlite(ConnectionString);
            });

            //Options
            services.Configure<ConnectionStringsOptions>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<PasswordsOptions>(Configuration.GetSection("Passwords"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();

                lifetime.ApplicationStarted.Register(() => {
                    string filepath = Path.Combine(env.ContentRootPath, "bin/reload.txt");
                    File.WriteAllText(filepath, DateTime.Now.ToString());
                });
            }
            app.UseStaticFiles();
            app.UseMvc(routebuilder => {
                routebuilder.MapRoute("default", "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
