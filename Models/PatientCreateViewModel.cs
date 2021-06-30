using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Childcare.Areas.Identity.Data;

namespace Childcare.Models
{
    public class PatientCreateViewModel
    {   
        [Required]
        public string PatientName { get; set; }
        [Required]
        public int Gender { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        //Set ownerId in view if Customer create Patient profile    
        [Required]
        public int OwnerId {get;set;}
        //Use this property to get Customer list for Staff/Manager to choose
        public List<Customer> Customers { get; internal set; }
    }
}
