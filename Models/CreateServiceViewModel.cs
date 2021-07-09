using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Childcare.Areas.Identity.Data;

namespace Childcare.Models
{
    public class CreateServiceViewModel{

        //Specialty for user to choose from
        public IList<Specialty> Specialties {get;set;}

        //For user to fill
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public int? SpecialtyID { get; set; }
        //[Required]
        public string Thumbnail { get; set; }
        //[Required]
        public string Description { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public DateTime StartTime {get;set;}
        [Required]
        public DateTime EndTime {get;set;}
        [Required]
        //in minute
        public int ServiceTime{get;set;}

    }
}