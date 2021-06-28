using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Childcare.Areas.Identity.Data
{
    public class MedicalExamination
    {
        public MedicalExamination() { }

        [Key]
        public int ExaminationID { get; set; }
        public int? ReservationID { get; set; }
        public int? ServiceID { get; set; }
        public string Prescription { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        [ForeignKey(nameof(ReservationID))]
        [InverseProperty("MedicalExaminations")]
        public virtual Reservation Reservation { get; set; }

        [ForeignKey(nameof(ServiceID))]
        [InverseProperty("MedicalExaminations")]
        public virtual Service Service { get; set; }
    }
}
