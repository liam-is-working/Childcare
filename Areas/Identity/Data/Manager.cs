using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Childcare.Areas.Identity.Data
{
    public partial class Manager
    {
        public Manager() { }

        [Key]
        public int ManagerID { get; set; }

        public string ChildcareUserId{get;set;}
        public ChildCareUser ChildCareUser{get;set;}
        
    }
}
