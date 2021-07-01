using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Childcare.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Childcare.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly ChildCareContext _db;
        private readonly UserManager<ChildCareUser> _um;
        private readonly IAuthorizationService _autho;

        public ServiceController(ILogger<ServiceController> logger, ChildCareContext db,
                                UserManager<ChildCareUser> um, IAuthorizationService autho)
        {
            _logger = logger;
            _db = db;
            _um = um;
            _autho = autho;
        }

        public async Task<IActionResult> GetServicesAsync()
        {
            var isAuthorized = (User.IsInRole("Manager") || User.IsInRole("Staff"));
            //Model contains Services and their specialty
            if (!isAuthorized)
            {
                //Hardcode map Status table and statusName enum
                //Customer can only see approved services
                var model = await _db.Services.Where(s => s.StatusID == ((int)StatusName.Approved))
                                        .Include(s => s.Specialty)
                                        .ToListAsync();
                return View(model);
            }
            else
            {
                //Staff and Manager can see all services
                var model = await _db.Services.Include(s => s.Specialty).Include(s => s.Status).ToListAsync();
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Staff")]
        public async Task<IActionResult> EditService(int? serviceId)
        {
            if (!serviceId.HasValue)
                return BadRequest("Must specify service Id");
            var model = new ServiceEditViewModel();



            try
            {
                model.Service = await _db.Services.Include(s => s.Specialty)
                                    .Include(s => s.Staff)
                                    .Include(s => s.Status)
                                    .FirstAsync(s => s.ServiceID == serviceId);
            }
            catch (InvalidOperationException)
            {
                return NotFound($"ServiceId {serviceId} doesnt exist");
            }


            //Only owner (staff create this service) or managers can edit 
            if (User.IsInRole("Staff"))
            {
                var currentStaffId = (await _db.Staffs.Select(staff => new { staff.StaffID, staff.ChildcareUserId })
                                            .FirstOrDefaultAsync(a => a.ChildcareUserId == _um.GetUserId(User)))
                                            .StaffID;
                if (currentStaffId == 0)
                    return BadRequest("Staff is not valid!");

                if (currentStaffId != model.Service.StaffID)
                    return BadRequest("Current staff is not author of this service");
            }

            //(Not implemented) consider using cached 
            model.Specialties = await _db.Specialties.ToListAsync();


            //Model is a complete Service instance with Specialties to choose from 
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, Staff")]
        public async Task<IActionResult> EditService(ServiceEditViewModel model)
        {

            //server validation
            if (!ModelState.IsValid)
                return BadRequest("Invalid model");

            var oldService = await _db.Services
                                    .Include(s => s.Status)
                                    .FirstOrDefaultAsync(s => s.ServiceID == model.ServiceId);

            if (oldService.ServiceID == 0)
                return NotFound($"Can't find service with id: {model.ServiceId}");


            //After manager edit service information, it will be approved
            var newStatus = StatusName.Approved;

            //Only owner (staff create this service) or managers can edit 
            if (User.IsInRole("Staff"))
            {
                var currentStaffId = (await _db.Staffs.Select(staff => new { staff.StaffID, staff.ChildcareUserId })
                                            .FirstOrDefaultAsync(a => a.ChildcareUserId == _um.GetUserId(User)))
                                            .StaffID;
                if (currentStaffId == 0)
                    return BadRequest("Staff is not valid!");

                if (currentStaffId != model.Service.StaffID)
                    return BadRequest("Current staff is not author of this service");

                //If owner edit service information, it will be pended
                newStatus = StatusName.Pending;
            }


            var updatedService = new Service
            {
                ServiceID = oldService.ServiceID,
                CreatedDate = oldService.CreatedDate,
                StaffID = oldService.StaffID,
                ServiceName = model.ServiceName,
                SpecialtyID = model.SpecialtyID,
                //hardcode map status id with StatusName enum
                StatusID = ((int)newStatus),
                UpdatedDate = DateTime.Now,
                Description = model.Description,
                Thumbnail = model.Thumbnail,
            };

            _db.Update(updatedService);

            var result = await _db.SaveChangesAsync();

            if (result != 1)
                return BadRequest("System failed to update service");

            //Redirect to service list
            return RedirectToAction("GetServices");
        }
    }
}