using Childcare.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Childcare.Controllers
{
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
    }
}