using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Childcare.Areas.Identity.Data;

namespace Childcare.Models
{
    public class EditServiceViewModel{
        //Controller provide a list of specialty to choose from
        public List<Specialty> Specialties{get;set;}

        //Below fields are used to update a service
        [Required]
        public int ServiceId {get;set;}
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public int? SpecialtyID { get; set; }
        public string Thumbnail { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Time)]  
        public DateTime StartTime {get;set;}
        [Required]
        [DataType(DataType.Time)]  
        [TimeCustom("StartTime")]
        public DateTime EndTime {get;set;}
        [Required]
        [Display(Name = "ServiceTime(in minutes)")]
        [Range(0,24*60)]
        //in minute
        public int ServiceTime{get;set;}
        [Required]
        [DataType(DataType.Currency)]
        public float Price {get;set;}
        [Required]
        public int StaffOwnerId{get;set;}
    }
}