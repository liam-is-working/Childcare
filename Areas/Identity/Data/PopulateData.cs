using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Childcare.Areas.Identity.Data{
    public static class PopulateIdentityData{
        public static async Task PopulateRolesAsync(IServiceProvider services,string[] roles){

            if(services==null)
                throw new Exception("Service Provider is null");
            
            var roleManager = services.GetService<RoleManager<IdentityRole>>();
            if(roleManager == null)
                throw new Exception("Role manager is null");
            
            if(roleManager.Roles.GetEnumerator().Current != null){
                throw new Exception("You must clear all roles and userroles before populate Roles");
            }

            IdentityResult result = null;
            foreach(string r in roles){
                result = await roleManager.CreateAsync(new IdentityRole(r));
                if(!result.Succeeded)
                    throw new Exception("Populate roles failed");
            }
            
        }
    }
}