using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Childcare.Areas.Identity.Data
{
    public partial class Staff
    {
        public Staff()
        {
            Blogs = new HashSet<Blog>();
            Services = new HashSet<Service>();
            Reservations = new HashSet<Reservation>();
        }

        public string ChildcareUserId{get;set;}
        public ChildCareUser ChildCareUser{get;set;}

        [Key]
        public int StaffID { get; set; }
       
        public int? SpecialtyID { get; set; }

        [ForeignKey(nameof(SpecialtyID))]
        [InverseProperty("Staffs")]
        public virtual Specialty Specialty { get; set; }

        [InverseProperty(nameof(Blog.Staff))]
        public virtual ICollection<Blog> Blogs { get; set; }

        [InverseProperty(nameof(Service.Staff))]
        public virtual ICollection<Service> Services { get; set; }

        [InverseProperty(nameof(Reservation.Staff))]
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
