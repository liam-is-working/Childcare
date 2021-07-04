using System.Collections.Generic;
using System.Threading.Tasks;
using Childcare.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Childcare.Controllers
{
    
    public class CustomerController : Controller{
        private readonly ILogger<ServiceController> _logger;
        private readonly ChildCareContext _db;
        private readonly UserManager<ChildCareUser> _um;
        private readonly IAuthorizationService _autho;

        public CustomerController(ILogger<ServiceController> logger, ChildCareContext db,
                                UserManager<ChildCareUser> um, IAuthorizationService autho)
        {
            _logger = logger;
            _db = db;
            _um = um;
            _autho = autho;
        }
        
        [Authorize(Roles ="Manager")]
        public async Task<IActionResult> CustomerList(){
            var model = new List<Customer>();
            //List of customer map with childcareUserProfile
            model = await _db.Customers.Include(c => c.ChildCareUser).ToListAsync();
            if (model == null || model.Count == 0){
                return NotFound("There is no customer in database yet");
            }
            return View(model);
        }


    }
}