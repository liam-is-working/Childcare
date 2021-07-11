using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Childcare.Areas.Identity.Data;

namespace Childcare.Models
{
    public class AddReservationViewModel{
        
        //For user to choose Service categorized by Specialties
        public IList<Specialty> Specialties {get;set;}

        public Customer Customer{get;set;}

        //To display a service detail
        
        public Service Service {get;set;}
        public int CustomerID { get; set; }
        [Required]
        public int? PatientID { get; set; }
        [Required]
        public int? ServiceID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Required]
        public DateTime ReservationDate{get;set;}
        [Required]
        public int? ReservationSlot{get;set;}
    }

}