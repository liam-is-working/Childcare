using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Childcare.Areas.Identity.Data
{
    public class Service
    {
        public Service()
        {
            Feedbacks = new HashSet<Feedback>();
            MedicalExaminations = new HashSet<MedicalExamination>();
            Reservations = new HashSet<Reservation>();
        }

        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public int? SpecialtyID { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        
        public float Price { get; set; }
        public int? StatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? StaffID { get; set; }
        [DataType(DataType.Time)]
        public DateTime StartTime {get;set;}
        [DataType(DataType.Time)]
        public DateTime EndTime {get;set;}
        //in minute
        public int ServiceTime{get;set;}

        [ForeignKey(nameof(StaffID))]
        [InverseProperty("Services")]
        public virtual Staff Staff { get; set; }

        [ForeignKey(nameof(SpecialtyID))]
        [InverseProperty("Services")]
        public virtual Specialty Specialty { get; set; }

        [ForeignKey(nameof(StatusID))]
        [InverseProperty("Services")]
        public virtual Status Status { get; set; }

        [InverseProperty(nameof(Feedback.Service))]
        public virtual ICollection<Feedback> Feedbacks { get; set; }

        [InverseProperty(nameof(Reservation.Service))]
        public virtual ICollection<Reservation> Reservations { get; set; }

        [InverseProperty(nameof(MedicalExamination.Service))]
        public virtual ICollection<MedicalExamination> MedicalExaminations { get; set; }
    }
}
