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

namespace Childcare.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChildCareContext _db;
        private readonly UserManager<ChildCareUser> _um;

        public PatientController(ILogger<HomeController> logger, ChildCareContext db, UserManager<ChildCareUser> um)
        {
            _logger = logger;
            _db = db;
            _um = um;
        }

        [HttpGet]
        public IActionResult PatientCreate(){
            //return form to create new patient
            return View();
        }

        [HttpPost]
        //(not implemented) create model pls
        public async Task<IActionResult> PatientCreateAsync(PatientCreateViewModel model){          
            if(!ModelState.IsValid){
                //Serverside validation
                return View(model);
            }

            var currentUserId = _um.GetUserId(User);
            if(currentUserId == null){
                return NotFound("Current user is not valid");
            }

            Patient newPatient = new Patient();

            int currentCustomerId;
            try{
                currentCustomerId = _db.Customers
                .First(c => c.ChildcareUserId == currentUserId).CustomerID;
                newPatient.CustomerID = currentCustomerId;
            }
            catch (InvalidOperationException){
                return NotFound("Current user is not valid");
            }

            newPatient.Gender = model.Gender;
            newPatient.PatientName = model.PatientName;
            newPatient.Birthday = model.Birthday;
            newPatient.CreatedDate = DateTime.Now;
            newPatient.UpdatedDate = newPatient.CreatedDate;
            
            var result = await _db.Patients.AddAsync(newPatient);
            if( !(result.State == EntityState.Added) ){
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

            _logger.LogInformation($"New patient {newPatient.PatientName} has been added to Customer {currentCustomerId}");
            
            //(Not implemented) Success view or redirect
            return RedirectToAction("PatientList", new {id = currentCustomerId} );
        }

        public IActionResult PatientList(int? custId)
        {
            //(Not implement) only creator and higher roles can see his patient list 
            if(custId == null)
                return NotFound("Please insert Cutomer id");
            var model = new PatientListViewModel();
            model.Patients = _db.Patients.Where(p => p.CustomerID==custId).ToList();
            if(model.Patients == null || model.Patients.Count == 0){
                //(Not implement) return some view
                 return NotFound("Customer doesnt have any patient profile yet");
            }
            return View(model);
        }

        public IActionResult PatientDetail(int? patientId)
        {
            //User.
           //(Not implement) only creator and higher roles can see his patient list 
            if(patientId == null)
                return NotFound("Please insert patient id");
            var model = new PatientDetailViewModel();
            try
            {
                model.Patient = _db.Patients.Where(p=>p.PatientID==patientId).Include(p=>p.Reservations).ToArray()[0];
            }
            catch (InvalidOperationException)
            {
                
                return NotFound("Patient Id does not exist");
            }
            
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
