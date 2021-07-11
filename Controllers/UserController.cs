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
using System.Linq;

namespace Childcare.Controllers
{
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly ChildCareContext _db;
        private readonly UserManager<ChildCareUser> _um;
        private readonly SignInManager<ChildCareUser> _sm;

        private readonly RoleManager<IdentityRole> _rm;


        public UserController(ILogger<ServiceController> logger, ChildCareContext db,
                                UserManager<ChildCareUser> um, SignInManager<ChildCareUser> sm, RoleManager<IdentityRole> rm)
        {
            _logger = logger;
            _db = db;
            _um = um;
            _sm = sm;
            _rm = rm;
        }


        public async Task<IActionResult> UserList()
        {
            var model = new UserListViewModel();

            //Manager list
            model.Managers = await _db.Managers.Include(m => m.ChildCareUser).ToListAsync();
            //Customer list
            model.Customers = await _db.Customers.Include(c => c.ChildCareUser).ToListAsync();
            //Staff list
            model.Staffs = await _db.Staffs.Include(s => s.ChildCareUser).ToListAsync();

            var identityRoles = await _rm.Roles.ToListAsync();
            if (identityRoles.Count != 0)
                model.Roles = new List<string>();
            foreach (var r in identityRoles)
            {
                model.Roles.Add(r.NormalizedName);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignStaffRole([Required] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest("Must specify userId");


            //Delete customer record in case this is an new account  
            var cumstomer = await _db.Customers.Include(c => c.Patients).FirstOrDefaultAsync(u => u.ChildcareUserId == userId);
            if (cumstomer != null)
            {
                if (cumstomer.Patients.Count != 0)
                    return BadRequest("This customer account owns some patient profiles, cant become a staff");

                _db.Remove(cumstomer);
                _logger.LogInformation("Delete customer record from database");
            }


            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound("UserId not found");

            var addRoleResult = await _um.AddToRoleAsync(user, "Staff");

            if (!addRoleResult.Succeeded)
                _logger.LogWarning($"Failed to assign user({userId}) to Staff role, please check again");
            else
            {
                _logger.LogInformation($"Assigned user{userId} to role Staff at {DateTime.Now}");

                var tryFindOldStaffRecord = await _db.Staffs.FirstOrDefaultAsync(s => s.ChildcareUserId == userId);
                if (tryFindOldStaffRecord == null)
                {
                    //Create new Staff record
                    var newStaff = new Staff { ChildcareUserId = userId };
                    await _db.AddAsync(newStaff);
                    var addStaffRecordResult = await _db.SaveChangesAsync();
                    if (addStaffRecordResult != 2)
                        _logger.LogWarning($"Assigned user{userId} to role Staff success but failed to create new staff record OR delete customer record");
                }
            }


            return RedirectToAction("UserList");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveStaffRole([Required] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest("Must specify userId");

            var staff = await _db.Staffs.FirstOrDefaultAsync(u => u.ChildcareUserId == userId);
            if (staff == null)
                return NotFound("UserId is not in Staff dtb");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var removeRoleResult = await _um.RemoveFromRoleAsync(user, "Staff");

            //Staff record still exist 
            if (!removeRoleResult.Succeeded)
                _logger.LogWarning($"Failed to remove user({userId}) from Staff role, please check again");
            else
            {
                _logger.LogInformation($"Removed user{userId} from role Staff at {DateTime.Now}");
                
            }
            return RedirectToAction("UserList");
        }

        [HttpPost]
        public async Task<IActionResult> AssignSpecialtyToStaff([Required] int staffId, int? specialtyId)
        {
            if (!ModelState.IsValid)
                return BadRequest("Must specify staffId");

            var staff = await _db.Staffs.FirstOrDefaultAsync(u => u.StaffID == staffId);
            if (staff == null)
                return NotFound($"Cant find staff with id={staffId}");

            if (specialtyId.HasValue)
            {
                //check if specId is valid
                try
                {
                    var specialtiesId = await _db.Specialties.Select(s => s.SpecialtyID).FirstAsync(id => id == specialtyId);
                }
                catch (System.Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest($"Specialty id: {specialtyId} is not in dtb");
                }
            }

            staff.SpecialtyID = specialtyId;
            var result = await _db.SaveChangesAsync();

            if (result != 1)
                _logger.LogWarning("Valid staffID, specialtyId but failed to update to database");

            return RedirectToAction("UserList");
        }


    }
}