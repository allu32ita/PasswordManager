﻿using System;
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
using Microsoft.Extensions.Caching.StackExchangeRedis;

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
            services.AddMvc(options => {
                var HomeProfile = new CacheProfile();
                //HomeProfile.Duration = Configuration.GetValue<int>("ResponseCache:Home:Duration");
                //HomeProfile.Location = Configuration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");
                //HomeProfile.VaryByQueryKeys = new string[] {"page"};
                Configuration.Bind("ResponseCache:Home", HomeProfile);
                options.CacheProfiles.Add("Home", HomeProfile);
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<IPasswordService, AdoNetPasswordService>();
            //services.AddTransient<IPasswordService, EFCorePasswordService>();
            services.AddTransient<IDatabaseAccessor, SqLiteDatabaseAccessor>();
            services.AddTransient<ICachedPasswordService, MemoryCachedPasswordService>(); 
            
            
            //services.AddTransient<ICachedPasswordService, DistributedCachePasswordService>(); 
            
            services.AddDbContextPool<PasswordDbContext>(optionsBuilder => {
                String ConnectionString = Configuration.GetSection("ConnectionStrings").GetValue<String>("Default");
                optionsBuilder.UseSqlite(ConnectionString);
            });
            
            //services.AddStackExchangeRedisCache(options => {
            //    Configuration.Bind("DistributedCache:Redis", options);
            //});

            //services.AddDistributedSqlServerCache(options => {
            //    Configuration.Bind("DistributedCache:SqlServer", options);
            //});

            services.Configure<MemoryCacheOptions>(Configuration.GetSection("MemoryCache"));

            //Options
            services.Configure<PasswordsOptions>(Configuration.GetSection("Passwords"));
            services.Configure<ConnectionStringsOptions>(Configuration.GetSection("ConnectionStrings"));
            
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
            else
            {
                app.UseExceptionHandler("/Error");
            }
            //app.UseExceptionHandler("/Error");
            app.UseStaticFiles();

            app.UseResponseCaching();
            app.UseMvc(routebuilder => {
                routebuilder.MapRoute("default", "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
