using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Childcare.Areas.Identity.Data;

namespace Childcare.Models
{
    public class PatientCreateViewModel
    {   
        [Required]
        [StringLength(50)]
        public string PatientName { get; set; }
        public int Gender { get; set; }
        
    }
}
