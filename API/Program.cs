using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Infrastructure.Data;
using Infrastructure.Data.SeedData;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
          var host =  CreateHostBuilder(args).Build();
            // apply migrations or database if it does not exist
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = services.GetRequiredService<StoreContext>();
                 await context.Database.MigrateAsync();
                 // seed product data
                 await StoreContextSeed.SeedAsync(context, loggerFactory);

                 // seed user
                 var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                 var identityContext = services.GetRequiredService<AppIdentityDbContext>();
                 await identityContext.Database.MigrateAsync();
                 await AppIdentityDbContextSeed.SeedUserAsync(userManager);
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(e,"An error occured during migration");
                
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).
                ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var assemblyName = typeof(Startup).GetTypeInfo().Assembly.FullName;

                    // webBuilder.UseStartup<Startup>();
                    webBuilder.UseStartup(assemblyName);
                    webBuilder.UseUrls("http://+:5000");
                });
    }
}
