using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Childcare.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Childcare
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    //Populate Roles
                    await PopulateIdentityData.PopulateRolesAsync(services,new string[] {"Manager", "Staff", "Admin"});

                    //Seed data
                    var managerAccount = new UserRegisterModel{
                        Email = "manager@gmail.com",
                        Password = "Passw0rd!",
                        FullName = "Manager",
                        DOB = DateTime.Today.AddYears(-20),
                        CitizenID = "00000000000",
                        Address = "FPT HCM"
                    };
                    await PopulateChildcareData.PopulateManagerAsync(services, managerAccount);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred populating the DB or data has already been populated!");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
