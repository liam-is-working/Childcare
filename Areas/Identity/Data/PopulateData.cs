using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Childcare.Areas.Identity.Data
{

    public static class PopulateChildcareData
    {
        private static async Task<ChildCareUser> AddNewUser(IServiceProvider services, UserRegisterModel Input)
        {
            var _userManager = services.GetService<UserManager<ChildCareUser>>();
            if (_userManager == null)
                throw new Exception("UserManager is null");
            var user = new ChildCareUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                DOB = Input.DOB,
                CitizenID = Input.CitizenID,
                Address = Input.Address,
                FullName = Input.FullName,
                EmailConfirmed = true,
            };
            
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
                return user;
            else
                throw new Exception("AddNewUser: Fail to create user with email: " + Input.Email);
        }
        public static async Task PopulateManagerAsync(IServiceProvider services, UserRegisterModel Input)
        {
            var newUserAccount = await AddNewUser(services, Input);
            var _userManager = services.GetService<UserManager<ChildCareUser>>();
            if (_userManager == null)
                throw new Exception("UserManager is null");
            var addRoleResult = await _userManager.AddToRoleAsync(newUserAccount, "Manager");
            if (!addRoleResult.Succeeded)
                throw new Exception("Fail to assign manager role to user with email: " + Input.Email);

            var _dbContext = services.GetService<ChildCareContext>();
            if(_dbContext == null)
                throw new Exception("DbContext is null");
            await _dbContext.Managers.AddAsync(new Manager{ChildcareUserId = newUserAccount.Id});
            var addManagerRecordResult = await _dbContext.SaveChangesAsync();
            if(addManagerRecordResult != 1)
                throw new Exception("Fail to add new manager record with userID: " + newUserAccount.Id);
        }
    }

    public class UserRegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public string CitizenID { get; set; }
    }

    public static class PopulateIdentityData
    {
        public static async Task PopulateRolesAsync(IServiceProvider services, string[] roles)
        {

            if (services == null)
                throw new Exception("Service Provider is null");

            var roleManager = services.GetService<RoleManager<IdentityRole>>();
            if (roleManager == null)
                throw new Exception("Role manager service is null");

            IdentityResult result = null;
            foreach (string r in roles)
            {
                result = await roleManager.CreateAsync(new IdentityRole(r));
            }
        }
    }
}