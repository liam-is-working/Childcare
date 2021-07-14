using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Childcare.Areas.Identity.Data
{
    public class Reservation
    {
        public Reservation()
        {
            Feedbacks = new HashSet<Feedback>();
            MedicalExaminations = new HashSet<MedicalExamination>();
        }

        public int ReservationID { get; set; }
        public int CustomerID { get; set; }
        public int? PatientID { get; set; }
        public int ServiceID { get; set; }
        public int SpecialtyID { get; set; }
        public int? StaffAssignedID { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime ReservationDate{get;set;}
        public int ReservationSlot{get;set;}

        [ForeignKey(nameof(CustomerID))]
        [InverseProperty("Reservations")]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(PatientID))]
        [InverseProperty("Reservations")]
        public virtual Patient Patient { get; set; }

        [ForeignKey(nameof(ServiceID))]
        [InverseProperty("Reservations")]
        public virtual Service Service { get; set; }

        [ForeignKey(nameof(StaffAssignedID))]
        [InverseProperty("Reservations")]
        public virtual Staff Staff { get; set; }

        [InverseProperty(nameof(Feedback.Reservation))]
        public virtual ICollection<Feedback> Feedbacks { get; set; }


        [InverseProperty(nameof(MedicalExamination.Reservation))]
        public virtual ICollection<MedicalExamination> MedicalExaminations { get; set; }

    }
}
