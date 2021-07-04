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
           foreach (var r in identityRoles)
           {
               model.Roles.Add(r.NormalizedName);
           }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole([Required] [FromBody] string userId,[Required] [FromBody] string rolename){
            if(!ModelState.IsValid)
                return BadRequest("Must specify role and userId");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if(user == null)
                return NotFound("UserId is not in dtb");

            var normalizedRoleName = rolename.ToUpper();

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.NormalizedName.Equals(normalizedRoleName));
            if(role == null)
                return NotFound("Role is not in dtb");

            var result = await _um.AddToRoleAsync(user, normalizedRoleName);

            if(!result.Succeeded)   
                return BadRequest("Valid state but failed to add user to role");

            return RedirectToAction("UserList");
        }


    }
}