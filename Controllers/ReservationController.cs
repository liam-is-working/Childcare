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

        public async Task<IActionResult> ShowAvailableSlotPartial(DateTime chosenDate, Service chosenService){
            if(chosenDate == null)
                return BadRequest("Chosen date must be between 12 hours and 10 days after today");

            //Apply reservation time policy
            if(!ValidateReservationTimePolicy(chosenDate))
                return BadRequest("Chosen date must be between 12 hours and 10 days after today");

            SlotGenerator(chosenService,chosenDate,out Dictionary<int, Slot> availableSlots);

            //not pass any model if there's no available slot
            if(availableSlots.Count==0)
                return View();

            //Get unavailable (reserved or unactive)
            var unavailableSlotNumbers = await _db.ReservationTimes
                                                .Where(rt => rt.Date == chosenDate && rt.AvailableStaff == 0)
                                                .Select(rt => rt.Slot).ToArrayAsync();

            //remove unavailable slots from choices
            foreach (var slotNum in unavailableSlotNumbers)
            {
                availableSlots.Remove(slotNum);
            }
            //Create a model and pass available slots into it
            var model = new ShowAvailableSlotPartialViewModel{Slots = availableSlots};

            return View(model);

        }
        
        [HttpPost]
        public async Task<IActionResult> AddReservation(AddReservationViewModel model){

            if(User.IsInRole("Staff") || User.IsInRole("Admin"))
                return BadRequest ("Only customer can make a reservation");

            if(!ModelState.IsValid){
                return BadRequest("Invalid model");
            }

            //Validate form data
            var requestingCustomer = await _db.Customers.Include(c => c.ChildcareUserId)
                                                .Include(c => c.Patients)
                                                .FirstOrDefaultAsync(c => c.CustomerID == model.CustomerID);
            if(requestingCustomer == null)
                return NotFound("Customer in form is not valid");

            if(requestingCustomer.ChildcareUserId != _um.GetUserId(User))
                return BadRequest("Current user doesnt match user in form");

            if(requestingCustomer.Patients.FirstOrDefault(p => p.CustomerID == requestingCustomer.CustomerID) == null )
                return BadRequest("Chosen patient is not owned by chosen customer");

            if ((await GetCurrentCustomerIdAsync()) != model.CustomerID)
                return NotFound("Current customer does not match with customer in form");

            var chosenService = await _db.Services.FirstOrDefaultAsync(s => s.ServiceID == model.ServiceID);
            if(chosenService == null)
                return NotFound("ServiceId is not found");

            if (!ValidateReservationTimePolicy(model.ReservationDate))
                return BadRequest("Invalid Reservation date");

            //check if slot is valid
            SlotGenerator(chosenService, model.ReservationDate, out Dictionary<int,Slot> availableSlots);
            if(!availableSlots.TryGetValue(model.ReservationSlot, out Slot chosenSlot))
                return BadRequest("Invalid slot");

            //If all fields are valid               
            
            //Modify reservation time first
            var reservationTime =await _db.ReservationTimes
                                .FirstOrDefaultAsync(rt => rt.ServiceID == model.ServiceID && rt.Date==model.ReservationDate && rt.Slot == model.ReservationSlot);

            if(reservationTime != null){
                if(reservationTime.AvailableStaff == 0)
                    return BadRequest("There is no available staff for the chosen slot, please choose another one");
                
                reservationTime.AvailableStaff-=1;
                _db.Update(reservationTime);
            }else
            {
                var availableStaffs = _db.Staffs.CountAsync(s => s.SpecialtyID == chosenService.SpecialtyID);
                reservationTime = new ReservationTime{
                    ServiceID = model.ServiceID,
                    Slot = model.ReservationSlot,
                    Date = model.ReservationDate,
                    AvailableStaff = await availableStaffs,
                };
            }

            var newReservation = new Reservation{
                PatientID = model.PatientID,
                ServiceID = model.ServiceID,
                CustomerID = model.CustomerID,
                ReservationDate = model.ReservationDate,
                ReservationSlot = model.ReservationSlot,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                //fix later
                CheckInTime = DateTime.Now
            };

            var addReservationTime =await _db.AddAsync(reservationTime);
            var addReservation =await _db.AddAsync(newReservation);

            
            if(addReservation.State == EntityState.Added && addReservationTime.State == EntityState.Added){
                var result = await _db.SaveChangesAsync();
                if(result != 2){
                    _logger.LogError("Please check dtb Reservation and ReservationTime");
                    return BadRequest("Valid data but failed to update to database");
                }                  
            }
            
            

            //Create a success view
            return View();
        }


        [NonAction]
        private static bool ValidateReservationTimePolicy(DateTime chosenDate){
            var hoursDistance = (chosenDate - DateTime.Now).Hours;

            //Hardcode reservation time policy
            if(hoursDistance < 12 || hoursDistance > 10*24){
                return false;
            }

            return true;
        }

        //Return all time-available slots
        [NonAction]
        private static void SlotGenerator(Service service, DateTime chosenDate, out Dictionary<int,Slot> slots){
            slots = new Dictionary<int,Slot>();
            
            var startTime = service.StartTime;
            var endTime = service.EndTime;
            var interval = service.ServiceTime;

            //Total slots of this service
            int numOfSlots = ((int)(endTime-startTime).TotalMinutes)/(interval);
            
            //If today was chosen, some slots are overdue 
            if(chosenDate.Date == DateTime.Today){
                var availableTimeRn = DateTime.Now.Hour + 12;
                //No slot is available today
                if(availableTimeRn == 24)
                    return;
                //Start time is start time of the available slot
                while(availableTimeRn > startTime.Hour){
                    startTime = startTime.AddMinutes(interval);
                }
            }

            

            //round down the number of slots avalable now
            int numOfSlotsAvailable = ((int)(endTime-startTime).TotalMinutes)/(interval);

            var startingSlotCount = numOfSlots - numOfSlotsAvailable;

            //No slot available
            if(numOfSlotsAvailable <= 0)
                return;           
            
            for(int i = 0; i<numOfSlotsAvailable;i++){
                var start = startTime.AddMinutes(interval*i);
                var end = start.AddMinutes(interval);
                var slotNumber =i+1+startingSlotCount;

                slots.Add(slotNumber, new Slot{
                    StartTime = start,
                    EndTime = end
                });               
            }          
        }

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