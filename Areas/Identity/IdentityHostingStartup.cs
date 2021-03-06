using System;
using Childcare.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Childcare.Areas.Identity.IdentityHostingStartup))]
namespace Childcare.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ChildCareContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("ChildCareContextConnection")));

                services.AddDefaultIdentity<ChildCareUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ChildCareContext>();
            });
        }
    }
}