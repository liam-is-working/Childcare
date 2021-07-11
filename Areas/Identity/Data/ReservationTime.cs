using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Childcare.Areas.Identity.Data
{
    public class ReservationTime{   
        public DateTime Date{get;set;}      
        public int ServiceID{get;set;}
        public int Slot{get;set;}
        public int AvailableStaff{get;set;}

        [ForeignKey(nameof(ServiceID))]
        [InverseProperty("ReservationTimes")]
        public virtual Service Service { get; set; }
        
    }
}