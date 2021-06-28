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
            return View();
        }

        [HttpPost]
        //(not implemented) create model pls
        public async Task<IActionResult> PatientCreateAsync(Patient model){          
            if(!ModelState.IsValid){
                //Serverside validation
                return View(model);
            }
            var currentUserId = _um.GetUserId(User);
            if(currentUserId == null){
                return NotFound("Current user is not valid");
            }

            try{
                model.CustomerID = _db.Customers
                .First(c => c.ChildcareUserId == currentUserId).CustomerID;
            }
            catch (InvalidOperationException){
                return NotFound("Current user is not valid");
            }
            
            var result = await _db.Patients.AddAsync(model);
            if( !(result.State == EntityState.Added) ){
                _logger.LogWarning($"Add new patient unsuccessfully, result state: {result.State}");
                //(Not implemented) exception handle 
                return NotFound("Server failed to add new patient");
            }
            await _db.SaveChangesAsync();
            //(Not implemented) Success view or redirect
            return RedirectToAction("PatientList");
        }

        public IActionResult PatientList(int? custId)
        {
            //(Not implement) only creator and higher roles can see his patient list 
            if(custId == null)
                return NotFound("Please insert Cutomer id");
            var model = new PatientListViewModel();
            model.Patients = _db.Patients.Where(p => p.CustomerID==custId).ToList();
            if(model.Patients == null || model.Patients.Count == 0){
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
