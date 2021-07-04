using System.Collections.Generic;
using Childcare.Areas.Identity.Data;

namespace Childcare.Models
{
    public class UserListViewModel{
        public IList<Manager> Managers {get;set;}
        public IList<Customer> Customers {get;set;}
        public IList<Staff> Staffs {get;set;}

        //Use to assign new role
        public IList<string> Roles{get;set;}
    }
}