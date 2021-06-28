using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Childcare.Areas.Identity.Data
{
    public class Feedback
    {
        public Feedback()
        {

        }

        public int FeedbackID { get; set; }
        public int? ServiceID { get; set; }
        public int? CustomerID { get; set; }
        public string Comment { get; set; }
        public int Rate { get; set; }
        public int? ReservationID { get; set; }

        [ForeignKey(nameof(CustomerID))]
        [InverseProperty("Feedbacks")]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(ServiceID))]
        [InverseProperty("Feedbacks")]
        public virtual Service Service { get; set; }

        [ForeignKey(nameof(ReservationID))]
        [InverseProperty("Feedbacks")]
        public virtual Reservation Reservation { get; set; }
    }
}
