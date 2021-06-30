using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Childcare.Areas.Identity.Data;

namespace Childcare.Models
{
    public class PatientDetailViewModel
    {
        //For get(view) patient detail
        public Patient Patient {get;set;}

        //For post(edit) patient detail
        [Required]
        public int PatientID {get;set;}
        [Required]
        public string PatientName { get; set; }
        [Required]
        public int Gender { get; set; }
        [Required]
        public int CustomerID { get; set; }
        [Required]
        public DateTime Birthday { get; set; }   
    }
}
