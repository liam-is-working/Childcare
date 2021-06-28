using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Childcare.Areas.Identity.Data
{
    public partial class Administrator
    {
        public Administrator() { }

        [Key]
        public int AdminID { get; set; }
        
        public string ChildcareUserId{get;set;}
        public ChildCareUser ChildCareUser{get;set;}
        


        
        
    }
}
