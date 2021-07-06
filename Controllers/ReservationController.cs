using System;
using System.Collections.Generic;
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
    [Authorize]
    public class ReservationController : Controller{
        private readonly ILogger<ServiceController> _logger;
        private readonly ChildCareContext _db;
        private readonly UserManager<ChildCareUser> _um;
        private readonly IAuthorizationService _autho;

        public ReservationController(ILogger<ServiceController> logger, ChildCareContext db,
                                UserManager<ChildCareUser> um, IAuthorizationService autho)
        {
            _logger = logger;
            _db = db;
            _um = um;
            _autho = autho;
        }

        //Get reservation form which provide Customer Services categorized by Specialties and his list of Patients
        [HttpGet]
        public async Task<IActionResult> AddReservation(int? serviceId){

            //Only Customer can make a reservation
            if(User.IsInRole("Manager") || User.IsInRole("Staff"))
                return Forbid("Only Customer can make a reservation");

            var currentCustomerId = await GetCurrentCustomerIdAsync();
            if(currentCustomerId == null)
                return BadRequest("Current user is not found on dtb");


            //For customer to choose his patient profile
            var currentCustomer =await _db.Customers.Include(c => c.Patients)
                                        .FirstOrDefaultAsync(c => c.CustomerID == currentCustomerId);

            if(currentCustomer == null)
                return BadRequest("Current user is not found on dtb");

            var model = new AddReservationViewModel();

            model.Customer = currentCustomer;

            //Click button on a specific Service
            if(serviceId.HasValue){
                var service = _db.Services.Include(c => c.Specialty).FirstOrDefault(s => s.ServiceID == serviceId);
                if(service == null)
                    return NotFound("ServiceId cant be found");

                model.Service = service;
            }     

            //For customer to choose another service
            model.Specialties = await _db.Specialties.Include(s => s.Services).ToListAsync();             
                                      
            return View(model);               
        }

        // [HttpGet]
        // public async Task<IActionResult> ShowAvailableSlotPartial(DateTime chosenDate){
        //     if(chosenDate == null)
        //         return BadRequest("Chosen date must be between 12 hours and 10 days after today");

        //     var hoursDistance = (chosenDate - DateTime.Now).Hours;

        //     //Hardcode reservation time policy
        //     if(hoursDistance < 12 || hoursDistance > 10*24){
        //         return BadRequest("Chosen date must be between 12 hours and 10 days after today");
        //     }


        // }

        [NonAction]
        private async Task<int?> GetCurrentCustomerIdAsync(){
            var currentUSerId = _um.GetUserId(User);
            if(currentUSerId == null)
                return null;

            var queryResult =await _db.Customers.Select(c =>new {c.CustomerID, c.ChildcareUserId})
                                .FirstOrDefaultAsync(a => a.ChildcareUserId == currentUSerId);

            return queryResult.CustomerID;
            
        }
    }
}