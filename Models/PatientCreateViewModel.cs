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
        
    }
}
