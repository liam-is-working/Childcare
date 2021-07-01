using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Childcare.Models;
using Childcare.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Childcare.Authorization;

namespace Childcare.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;
        private readonly ChildCareContext _db;
        private readonly UserManager<ChildCareUser> _um;
        private readonly IAuthorizationService _autho;

        [TempData]
        public int CustomerId { get; set; }

        public PatientController(ILogger<PatientController> logger, ChildCareContext db, UserManager<ChildCareUser> um,
                                DefaultAuthorizationService autho)
        {
            _logger = logger;
            _db = db;
            _um = um;
            _autho = autho;
        }

        [HttpGet]
        public IActionResult PatientCreate()
        {
            //For higher role to create patient profile
            if (User.IsInRole("Manager") || User.IsInRole("Staff"))
            {
                var model = new PatientCreateViewModel();
                model.Customers = _db.Customers.ToList();
                //Add 'choose owner' option into view
                return View(model);
            }
            else
            {
                //Save customer id in tempdata for view user
                CustomerId = _db.Customers.First(c => c.ChildcareUserId == _um.GetUserId(User)).CustomerID;
            }
            //return form to create new patient
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PatientCreateAsync(PatientCreateViewModel model)
        {

            if (!ModelState.IsValid)
            {
                //Serverside validation
                return View(model);
            }

            var currentUserId = _um.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound("Current user is not valid");
            }

            Patient newPatient = new Patient();

            newPatient.CustomerID = model.OwnerId;
            newPatient.Gender = model.Gender;
            newPatient.PatientName = model.PatientName;
            newPatient.Birthday = model.Birthday;
            newPatient.CreatedDate = DateTime.Now;
            newPatient.UpdatedDate = newPatient.CreatedDate;

            var result = await _db.Patients.AddAsync(newPatient);
            if (!(result.State == EntityState.Added))
            {
                _logger.LogWarning($"Add new patient unsuccessfully, result state: {result.State}");
                //(Not implemented) exception handle 
                return NotFound("Server failed to add new patient");
            }

            try
            {
                var saveChangeResult = await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Server failed to add new patient");
                //(Not implemented) exception handle 
                return NotFound("Server failed to add new patient");
            }

            _logger.LogInformation($"New patient {newPatient.PatientName} has been added to Customer {model.OwnerId}");

            //(Not implemented) Success view or redirect
            return RedirectToAction("PatientList", new { id = model.OwnerId });
        }

        [HttpGet]
        public async Task<IActionResult> PatientListAsync(int? custId)
        {
            //Customer request for list but no ownerId specified
            if (custId == null && !User.IsInRole("Manager") && !User.IsInRole("Staff"))
                return NotFound("Please insert Cutomer id");



            var model = new PatientListViewModel();

            //Manager and staff request for list(s) of patient
            if (User.IsInRole("Manager") || User.IsInRole("Staff"))
            {
                //Request for all patient => no parameter 
                if (custId == null)
                {
                    model.Patients = await _db.Patients.ToListAsync();
                    //Empty list approach
                    return View(model);
                }

                //Request for a user's patient list
                model.Patients = await _db.Patients.Where(p => p.CustomerID == custId).ToListAsync();
                //Empty list approach
                return View(model);
            }
            //Customer request for a list of patient
            else
            {

                var ownerId = (await _db.Customers
                        .FirstAsync(cust => cust.ChildcareUserId == _um.GetUserId(User)))
                        .CustomerID;
                //Customer request for list that he doesnt own
                if (custId != ownerId)
                    return Forbid();

                //Customer request for his list of Patients
                model.Patients = await _db.Patients.Where(p => p.CustomerID == custId).ToListAsync();
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PatientDetailAsync(PatientDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //(Not implement) Server validation
                return View();
            }

            Patient oldPatient;
            try
            {
                oldPatient = await _db.Patients.FirstAsync(p => p.PatientID == model.PatientID);
            }
            catch (InvalidOperationException)
            {

                return NotFound("Patient Id does not exist");
            }

            //Authorize
            var isAuthorized = User.IsInRole("Manager") || User.IsInRole("Staff");

            if (!isAuthorized)
            {
                var custId = await GetCurrentCustomerIdAsync();
                if (custId != model.Patient.CustomerID)
                    return Forbid();
            }

            //Update patient
            var newPatient = new Patient
            {
                //unchangable
                PatientID = oldPatient.PatientID,
                CustomerID = oldPatient.CustomerID,
                CreatedDate = oldPatient.CreatedDate,
                //changable
                PatientName = model.PatientName,
                Birthday = model.Birthday,
                Gender = model.Gender,
                UpdatedDate = DateTime.Now
                
            };

            try
            {
                _db.Update(newPatient);
                var result = await _db.SaveChangesAsync();
                if (result != 1)
                    _logger.LogWarning("Update Patient but no change is made in database");

            }
            catch (Exception e)
            {
                _logger.LogError($"Update Patient: {e.Message}");
                return Error();
            }

            return RedirectToAction("PatientDetail", new { patientId = model.PatientID });
        }

        public async Task<IActionResult> PatientDeleteAsync([Required] int[] patientIds)
        {
            var patients = new List<Patient>();
            try
            {
                foreach (var pId in patientIds)
                {
                    patients.Add(await _db.Patients.Include(p => p.Customer).FirstAsync(p => p.PatientID == pId));
                }
            }
            catch (InvalidOperationException)
            {
                return NotFound($"Patient id is not valid");
            }
            catch (Exception e)
            {
                _logger.LogError($"Delete patient error: {e.Message}");
                return Error();
            }

            if (User.IsInRole("Manager") || User.IsInRole("Staff"))
            {
                foreach (var p in patients)
                {
                    _db.Remove(p);
                }
                await _db.SaveChangesAsync();
                //Redirect to patient list
                return RedirectToAction("PatientList");
            }

            foreach (var patient in patients)
            {
                var isAuthorized = await _autho.AuthorizeAsync(User, patient, PatientOperations.Delete);
                if (!isAuthorized.Succeeded)
                    return Forbid();
                _db.Remove(patient);
            }

            //Redirect cust to his own list
            return RedirectToAction("PatientList", new { custId = patients[0].CustomerID });
        }

        [HttpGet]
        public async Task<IActionResult> PatientDetailAsync(int? patientId)
        {
            if (patientId == null)
                return NotFound("Please insert patient id");

            var model = new PatientDetailViewModel();
            try
            {
                model.Patient = await _db.Patients.FirstAsync(p => p.PatientID == patientId);
            }
            catch (InvalidOperationException)
            {

                return NotFound("Patient Id does not exist");
            }

            var isAuthorized = User.IsInRole("Manager") || User.IsInRole("Staff");

            if (!isAuthorized)
            {
                var custId = await GetCurrentCustomerIdAsync();
                if (custId != model.Patient.CustomerID)
                    return Forbid();
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        async Task<int> GetCurrentCustomerIdAsync()
        {
            var userId = _um.GetUserId(User);
            try
            {
                var customerId = (await _db.Customers.Select(c => new { c.CustomerID, c.ChildcareUserId })
                                .FirstAsync(a => a.ChildcareUserId.Equals(userId))).CustomerID;
                return customerId;
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning("User is not a customer or an UserId is invalid");
                throw new Exception("User is not a customer or an UserId is invalid");
            }
        }

    }
}
