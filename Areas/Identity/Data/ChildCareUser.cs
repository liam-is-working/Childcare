using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Childcare.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ChildCareUser class
    public class ChildCareUser : IdentityUser
    {
        [PersonalData]
        public string FullName{get;set;}
        [PersonalData]
        public DateTime DOB {get;set;}
        [PersonalData]
        public string CitizenID{get;set;}
        [PersonalData]
        public string Address{get;set;}
    }
}
