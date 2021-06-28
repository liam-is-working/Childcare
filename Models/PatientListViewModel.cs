using System;
using System.Collections.Generic;
using Childcare.Areas.Identity.Data;

namespace Childcare.Models
{
    public class PatientListViewModel
    {
        public IList<Patient> Patients {get;set;}
    }
}
