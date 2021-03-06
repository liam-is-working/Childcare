using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        [Route("services")]
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

        public async Task<IActionResult> GetServiceDetailAsync(int? serviceId)
        {
            if (!serviceId.HasValue)
                return BadRequest("Must specify service Id");

            var isAuthorized = (User.IsInRole("Manager") || User.IsInRole("Staff"));
            //Model contains a service with its feedbacks and its specialty
            if (!isAuthorized)
            {
                //Hardcode map Status table and statusName enum
                //Customer can only see approved service
                var serviceQueryResult = await _db.Services
                                                .Where(s => s.ServiceID == serviceId && s.StatusID == ((int)StatusName.Approved))
                                                .Include(s => s.Feedbacks)
                                                .Include(s => s.Specialty)
                                                .ToArrayAsync();
                if (serviceQueryResult.Count() != 1)
                    return BadRequest("No approved serviceId found");

                var model = serviceQueryResult[0];

                return View(model);
            }
            else
            {
                //Staff and Manager can see all services
                var model = await _db.Services
                    .Include(s => s.Specialty)
                    .Include(s => s.Status)
                    .FirstOrDefaultAsync(s => s.ServiceID == serviceId);

                if(model==null)
                    return BadRequest("No serviceId found");

                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Staff")]
        public async Task<IActionResult> EditServiceAsync([FromQuery]int? serviceId)
        {
            if (!serviceId.HasValue)
                return BadRequest("Must specify service Id");
            var model = new EditServiceViewModel();


            //Check if service exists in dtb
            var service = new Service();
            try
            {
                service = await _db.Services.Include(s => s.Specialty)
                                    .Include(s => s.Staff)
                                    .Include(s => s.Status)
                                    .FirstAsync(s => s.ServiceID == serviceId);
            }
            catch (InvalidOperationException)
            {
                return NotFound($"ServiceId {serviceId} doesnt exist");
            }

            //Map service data to model data
            model.Price = service.Price;
            model.ServiceId = service.ServiceID;
            model.ServiceName = service.ServiceName;
            model.ServiceTime = service.ServiceTime;
            model.StartTime = service.StartTime;
            model.EndTime = service.EndTime;
            model.SpecialtyID = service.SpecialtyID;
            model.Thumbnail = service.Thumbnail;
            model.Description = service.Description;
            model.StaffOwnerId = (int)service.StaffID;

            //Only owner (staff create this service) or managers can edit 
            if (User.IsInRole("Staff"))
            {
                //Staff can only edit rejected or pending services
                if (service.StatusID == ((int)StatusName.Approved))
                    return BadRequest("Staff can only edit rejected or pending services");

                var currentStaffId = (await _db.Staffs.Select(staff => new { staff.StaffID, staff.ChildcareUserId })
                                            .FirstOrDefaultAsync(a => a.ChildcareUserId == _um.GetUserId(User)))
                                            .StaffID;
                if (currentStaffId == 0)
                    return BadRequest("Staff is not valid!");

                if (currentStaffId != service.StaffID)
                    return BadRequest("Current staff is not author of this service");
            }

            //(Not implemented) consider using cached 
            model.Specialties = await _db.Specialties.ToListAsync();
            


            //Model is a complete Service instance with Specialties to choose from 
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, Staff")]
        public async Task<IActionResult> EditServiceAsync(EditServiceViewModel model)
        {
            
            //server validation
            if (!ModelState.IsValid)
                return BadRequest("Invalid model");

            var updatedService = await _db.Services
                                    .Include(s => s.Status)
                                    .FirstOrDefaultAsync(s => s.ServiceID == model.ServiceId);

            if (updatedService.ServiceID == 0)
                return NotFound($"Can't find service with id: {model.ServiceId}");


            //After manager edit service information, it will be approved
            var newStatus = StatusName.Approved;

            //Only owner (staff create this service) or managers can edit 
            if (User.IsInRole("Staff"))
            {
                //Staff can only edit rejected or pending services
                if (updatedService.StatusID == ((int)StatusName.Approved))
                    return BadRequest("Staff can only edit rejected or pending services");

                var currentStaffIdTuple = (await _db.Staffs.Select(staff => new { staff.StaffID, staff.ChildcareUserId })
                                            .FirstOrDefaultAsync(a => a.ChildcareUserId == _um.GetUserId(User)));
                                            
                if (currentStaffIdTuple == null)
                    return BadRequest("Staff is not valid!");

                if (currentStaffIdTuple.StaffID != model.StaffOwnerId)
                    return BadRequest("Current staff is not author of this service");

                //If owner edit service information, it will be pended
                newStatus = StatusName.Pending;
            }

            //Update service
            if(updatedService.Description != model.Description)
                updatedService.Description = model.Description;
            if(updatedService.ServiceName != model.ServiceName)
                updatedService.ServiceName = model.ServiceName;
            if(updatedService.Thumbnail != model.Thumbnail)
                updatedService.Thumbnail = model.Thumbnail;
            if(updatedService.SpecialtyID != model.SpecialtyID)
                updatedService.SpecialtyID = model.SpecialtyID;
            if(updatedService.Price != model.Price)
                updatedService.Price = model.Price;
            if(updatedService.StartTime != model.StartTime)
                updatedService.StartTime = model.StartTime;
            if(updatedService.EndTime != model.EndTime)
                updatedService.EndTime = model.EndTime;
            if(updatedService.ServiceName != model.ServiceName)
                updatedService.ServiceName = model.ServiceName;
            if(updatedService.StartTime != model.StartTime)
                updatedService.StartTime = model.StartTime;
            updatedService.UpdatedDate = DateTime.Now;
            updatedService.StatusID = (int?)newStatus;

            _db.Update(updatedService);

            var result = await _db.SaveChangesAsync();

            if (result < 1)
                return BadRequest("System failed to update service");

            //Redirect to service list
            return RedirectToAction("GetServices");
        }

        [HttpGet]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CreateServiceAsync()
        {
            var specialties = await _db.Specialties.ToListAsync();
            //pass specialty list for user to choose
            var model = new CreateServiceViewModel
            {
                Specialties = specialties,
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CreateServiceAsync(CreateServiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //(not implemented) Server validate
                return View();
            }

            var currentStaffId = await GetCurrentStaffIdAsync();
            var timeStamp = DateTime.Now;

            

            //New service's status is Pending
            var newService = new Service
            {
                StaffID = currentStaffId,
                CreatedDate = timeStamp,
                UpdatedDate = timeStamp,
                ServiceName = model.ServiceName,
                SpecialtyID = model.SpecialtyID,
                Price = model.Price,
                Description = model.Description,
                Thumbnail = model.Thumbnail,
                StatusID = ((int)StatusName.Pending),
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                ServiceTime = model.ServiceTime
            };

            await _db.AddAsync(newService);
            var result = await _db.SaveChangesAsync();

            if (result != 1)
            {
                _logger.LogWarning("Failed to add new service to dtb");
                return BadRequest("Valid service to create but Failed to add new service to dtb");
            }

            return RedirectToAction("GetServices");
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ChangeServiceState(int? serviceId, int? statusId)
        {
            if (!serviceId.HasValue || !statusId.HasValue)
                return BadRequest("Must specify service and status");

            var service = await _db.Services.FindAsync(serviceId);
            if (service == null)
                return NotFound("Service doesn't exist in dtb");

            //nothing happend if reassasign current status with itself
            if (service.StatusID == statusId)
               return RedirectToAction("GetServices");

            //check status
            bool validStat = false;
            foreach (int stat in Enum.GetValues(typeof(StatusName)))
            {
                if (statusId == stat)
                {
                    validStat = true;
                    break;
                }
            }

            if (validStat)
            {
                service.StatusID = statusId;
                service.UpdatedDate = DateTime.Now;
            }

            else
            {
                _logger.LogWarning("Attemp to assign invaid status to service");
                return BadRequest("Status code is not valid");
            }

            _db.Update(service);
            var result = await _db.SaveChangesAsync();

            if (result != 1)
            {
                _logger.LogWarning("Valid state but dtb failed to update");
                return
                BadRequest("Valid state but dtb failed to update");
            }

            _logger.LogInformation("Change service state");
            return RedirectToAction("GetServices");

        }
        async Task<int> GetCurrentStaffIdAsync()
        {
            if (!User.IsInRole("Staff"))
            {
                _logger.LogWarning("User is not a staff but attempt to access staff func");
                throw new Exception("User is not a staff but attempt to access staff func");
            }

            var userId = _um.GetUserId(User);
            try
            {
                var staffId = (await _db.Staffs.Select(c => new { c.StaffID, c.ChildcareUserId })
                                .FirstAsync(a => a.ChildcareUserId.Equals(userId))).StaffID;
                return staffId;
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning("Can't find staffId in dtb");
                throw new Exception("Can't find staffId in dtb but user attempt to access staff func");
            }
        }

        async Task<int> GetCurrentManagerIdAsync()
        {
            if (!User.IsInRole("Manager"))
            {
                _logger.LogWarning("User is not a Manager but attempt to access staff func");
                throw new Exception("User is not a Manager but attempt to access staff func");
            }

            var userId = _um.GetUserId(User);
            try
            {
                var managerId = (await _db.Managers.Select(c => new { c.ManagerID, c.ChildcareUserId })
                                .FirstAsync(a => a.ChildcareUserId.Equals(userId))).ManagerID;
                return managerId;
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning("Can't find ManagerId in dtb");
                throw new Exception("Can't find ManagerId in dtb but user attempt to access Manager func");
            }
        }

    }
}