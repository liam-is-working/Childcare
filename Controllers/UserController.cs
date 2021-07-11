using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Childcare.Areas.Identity.Data;
using Childcare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Childcare.Controllers
{
    [Authorize(Roles ="Manager")]
    public class UserController : Controller{
        private readonly ILogger<ServiceController> _logger;
        private readonly ChildCareContext _db;
        private readonly UserManager<ChildCareUser> _um;
        private readonly IAuthorizationService _autho;

        private readonly RoleManager<IdentityRole> _rm;

        public UserController(ILogger<ServiceController> logger, ChildCareContext db,
                                UserManager<ChildCareUser> um, IAuthorizationService autho, RoleManager<IdentityRole> rm)
        {
            _logger = logger;
            _db = db;
            _um = um;
            _autho = autho;
            _rm = rm;
        }
        
        
        public async Task<IActionResult> UserList(){
            var model = new UserListViewModel();
           
           //Manager list
           model.Managers = await _db.Managers.Include(m => m.ChildCareUser).ToListAsync();
           //Customer list
           model.Customers = await _db.Customers.Include(c => c.ChildCareUser).ToListAsync();
           //Staff list
           model.Staffs = await _db.Staffs.Include(s => s.ChildCareUser).ToListAsync();

            var identityRoles = await _rm.Roles.ToListAsync();
            if(identityRoles.Count!=0)
                model.Roles = new List<string>();
           foreach (var r in identityRoles)
           {
               model.Roles.Add(r.NormalizedName);
           }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignStaffRole([Required] string userId){
            if(!ModelState.IsValid)
                return BadRequest("Must specify userId");

            var cumstomer = await _db.Customers.Include(c => c.Patients).FirstOrDefaultAsync(u => u.ChildcareUserId == userId);
            if(cumstomer == null)
                return NotFound("UserId is not in dtb");

            if(cumstomer.Patients.Count != 0){
                return BadRequest("This customer account owns some patient profiles, cant become a staff");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var addRoleResult = await _um.AddToRoleAsync(user, "Staff");

            if(!addRoleResult.Succeeded)   
                _logger.LogWarning($"Failed to assign user({userId}) to Staff role, please check again");
            else{
                _logger.LogInformation($"Assigned user{userId} to role Staff at {DateTime.Now}");
                _db.Remove(cumstomer);
                var newStaff = new Staff{ChildcareUserId = userId};
                await _db.AddAsync(newStaff);
                var addStaffRecordResult = await _db.SaveChangesAsync();
                if(addStaffRecordResult != 2)
                    _logger.LogWarning($"Assigned user{userId} to role Staff success but failed to create new staff record OR delete customer record");

            }
                
            
            return RedirectToAction("UserList");
        }


    }
}