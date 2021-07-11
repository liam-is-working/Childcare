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
    public class ReservationController : Controller
    {
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

        //After choosing serviceId and datetime, slot number -> these 2 data will be binded into model
        //Get reservation form which provide Customer Services categorized by Specialties and his list of Patients
        [HttpGet]
        public async Task<IActionResult> AddReservation(AddReservationViewModel model, int dummy)
        {


            var serviceId = model.ServiceID;
            if(!serviceId.HasValue)
                return BadRequest("Must specify a service Id");
            if(!model.ReservationSlot.HasValue)
                return BadRequest("Must specify a slot number");
            if(model.ReservationDate == null)
                return BadRequest("Must specify reservation date");

            //Only Customer can make a reservation
            if (User.IsInRole("Manager") || User.IsInRole("Staff"))
                return Forbid("Only Customer can make a reservation");

            var currentCustomerId = await GetCurrentCustomerIdAsync();
            if (currentCustomerId == null)
                return BadRequest("Current customer is not found on dtb");

            //For customer to choose his patient profile
            var currentCustomer = await _db.Customers.Include(c => c.Patients)
                                        .FirstOrDefaultAsync(c => c.CustomerID == currentCustomerId);

            model.Customer = currentCustomer;

            //verify serviceId and get service
            var service = _db.Services.Include(c => c.Specialty).FirstOrDefault(s => s.ServiceID == serviceId);
            if (service == null)
                return NotFound("ServiceId cant be found");

            model.Service = service;

            //serive, current customer && ReservationDatetime, slotNumber binded  model                  
            return View(model);
        }

        
        public async Task<IActionResult> ShowAvailableSlot(ShowAvailableSlotViewModel model)
        {

            var chosenService = await _db.Services.FirstOrDefaultAsync(s => s.ServiceID == model.ServiceId);
            if (chosenService == null)
                return NotFound("ServiceId is not found in dtb");

            var chosenDate = model.ChosenDate;
            if (chosenDate == null)
                return BadRequest("Must specify a date");

            //Apply reservation time policy
            if (!ValidateReservationTimePolicy(chosenDate))
                return BadRequest("Chosen date must be between 12 hours and 10 days after today");

            var unavailableSlotNumber = _db.ReservationTimes
                                .Where(rt => rt.SpecialtyID == chosenService.SpecialtyID 
                                && rt.Date == chosenDate && rt.AvailableStaff==0)
                                .Select(rt => rt.Slot).ToListAsync();
            

            SlotGenerator(await unavailableSlotNumber,_logger, chosenService, chosenDate, out Dictionary<int, Slot> availableSlots);

            //stop if there's no available slot
            if (availableSlots.Count == 0)
                return PartialView(model);

            //Create a model and pass available slots into it
            model.Slots = availableSlots;

            return PartialView(model);

        }

        //Bind patient id from customer's choice to model
        [HttpPost]
        public async Task<IActionResult> AddReservation(AddReservationViewModel model)
        {

            if (User.IsInRole("Staff") || User.IsInRole("Admin"))
                return BadRequest("Only customer can make a reservation");

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }

            //Validate form data
            var requestingCustomer = await _db.Customers.Include(c => c.ChildcareUserId)
                                                .Include(c => c.Patients)
                                                .FirstOrDefaultAsync(c => c.CustomerID == model.CustomerID);
            if (requestingCustomer == null)
                return NotFound("Customer in form is not valid");

            //Check if form is submit for and from the current user
            if (requestingCustomer.ChildcareUserId != _um.GetUserId(User))
                return BadRequest("Current user doesnt match user in form");

            if (requestingCustomer.Patients.FirstOrDefault(p => p.CustomerID == requestingCustomer.CustomerID) == null)
                return BadRequest("Chosen patient is not owned by chosen customer");

            var chosenService = await _db.Services.FirstOrDefaultAsync(s => s.ServiceID == model.ServiceID);
            if (chosenService == null)
                return NotFound("ServiceId is not found");

            if (!ValidateReservationTimePolicy(model.ReservationDate))
                return BadRequest("Invalid Reservation date");

            if(!model.ReservationSlot.HasValue)
                return BadRequest("Must specify reservation slot number");  

            var unavailableSlotNumber = _db.ReservationTimes
                                .Where(rt => rt.SpecialtyID == chosenService.SpecialtyID 
                                && rt.Date == model.ReservationDate && rt.AvailableStaff==0)
                                .Select(rt => rt.Slot).ToListAsync();

            //check if slot is valid
            SlotGenerator(await unavailableSlotNumber,_logger, chosenService, model.ReservationDate, out Dictionary<int, Slot> availableSlots);
            if (!availableSlots.TryGetValue((int)model.ReservationSlot, out Slot chosenSlot))
                return BadRequest("Invalid slot, try again or choose another slot");

            //If all fields are valid               

            //Modify reservation time first
            var reservationTime = await _db.ReservationTimes
                                .FirstOrDefaultAsync(rt => rt.SpecialtyID == chosenService.SpecialtyID && rt.Date == model.ReservationDate && rt.Slot == model.ReservationSlot);

            if (reservationTime != null)
            {
                if (reservationTime.AvailableStaff == 0)
                    return BadRequest("There is no available staff for the chosen slot, please choose another one");

                reservationTime.AvailableStaff -= 1;
                _db.Update(reservationTime);
            }
            else
            {
                var availableStaffs = _db.Staffs.CountAsync(s => s.SpecialtyID == chosenService.SpecialtyID);
                reservationTime = new ReservationTime
                {
                    SpecialtyID = (int)chosenService.SpecialtyID,
                    Slot = (int)model.ReservationSlot,
                    Date = model.ReservationDate,
                    AvailableStaff = await availableStaffs,
                };
            }

            var newReservation = new Reservation
            {
                PatientID = model.PatientID,
                ServiceID = (int)model.ServiceID,
                CustomerID = model.CustomerID,
                ReservationDate = model.ReservationDate,
                ReservationSlot = (int)model.ReservationSlot,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                //fix later
                CheckInTime = DateTime.Now
            };

            var addReservationTime = await _db.AddAsync(reservationTime);
            var addReservation = await _db.AddAsync(newReservation);


            if (addReservation.State == EntityState.Added && addReservationTime.State == EntityState.Added)
            {
                var result = await _db.SaveChangesAsync();
                if (result != 2)
                {
                    _logger.LogError("Please check dtb Reservation and ReservationTime");
                    return BadRequest("Valid data but failed to update to database");
                }
            }



            //Create a success view
            return View();
        }


        [NonAction]
        private static bool ValidateReservationTimePolicy(DateTime chosenDate)
        {
            var lowerBoundHourPolicy = 12;
            var upperBoundHourPolicy = 10 * 24;

            var lowerAvailTime = DateTime.Now.AddHours(lowerBoundHourPolicy);
            var upperAvailTime = DateTime.Now.AddHours(upperBoundHourPolicy);

            if (lowerAvailTime.Date > chosenDate.Date || upperAvailTime.Date < chosenDate.Date)
                return false;

            return true;
        }

        //Return all time-available slots
        [NonAction]
        private static void SlotGenerator(List<int> unavailableSlotNumbers,ILogger _log, Service service, DateTime chosenDate, out Dictionary<int, Slot> slots)
        {

            slots = new Dictionary<int, Slot>();


            var startTime = service.StartTime;
            var endTime = service.EndTime;
            var interval = service.ServiceTime;

            //Total slots of this service
            int numOfSlots = ((int)(endTime - startTime).TotalMinutes) / (interval);

            //Base on policy, some slot are overdue
            var lowerBoundHourPolicy = 12;
            var upperBoundHourPolicy = 24 * 10;
            var lowerTimeOnPolicy = DateTime.Now.AddHours(lowerBoundHourPolicy);
            var upperTimeOnPolicy = DateTime.Now.AddHours(upperBoundHourPolicy);

            if (lowerTimeOnPolicy > chosenDate)
            {
                chosenDate = lowerTimeOnPolicy;
                //Start time is start time of the available slot
                while (lowerTimeOnPolicy.Hour > startTime.Hour)
                {
                    startTime = startTime.AddMinutes(interval);
                }
            }

            if (upperTimeOnPolicy < chosenDate)
            {
                chosenDate = upperTimeOnPolicy;
                //Start time is start time of the available slot
                while (upperTimeOnPolicy.Hour < startTime.Hour)
                {
                    endTime = endTime.AddMinutes(-interval);
                }
            }



            //round down the number of slots avalable now
            int numOfSlotsAvailable = ((int)(endTime - startTime).TotalMinutes) / (interval);

            var startingSlotCount = numOfSlots - numOfSlotsAvailable;

            //No slot available
            if (numOfSlotsAvailable <= 0)
            {
                _log.LogWarning($"Slot Generator: No slot is available");
                return;
            }

            //deletelater
            _log.LogInformation("Slot Generator: Slots found!");

            //Add all time-available slots
            for (int i = 0; i < numOfSlotsAvailable; i++)
            {
                var start = startTime.AddMinutes(interval * i);
                var end = start.AddMinutes(interval);
                var slotNumber = i + 1 + startingSlotCount;

                slots.Add(slotNumber, new Slot
                {
                    StartTime = start,
                    EndTime = end
                });
            }
            //Remove all unavailable slots
            foreach(var unavailableSlotNum in unavailableSlotNumbers){
                slots.Remove(unavailableSlotNum);
            }
        }

        [NonAction]
        private async Task<int?> GetCurrentCustomerIdAsync()
        {
            var currentUSerId = _um.GetUserId(User);
            if (currentUSerId == null)
                return null;

            var queryResult = await _db.Customers.Select(c => new { c.CustomerID, c.ChildcareUserId })
                                .FirstOrDefaultAsync(a => a.ChildcareUserId == currentUSerId);

            return queryResult.CustomerID;

        }
    }
}