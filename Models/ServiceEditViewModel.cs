using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Childcare.Areas.Identity.Data;

namespace Childcare.Controllers
{
    public class ServiceEditViewModel{
        //To show current properties of a service
        public Service Service{get;set;}
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
    }
}