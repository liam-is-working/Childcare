using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Childcare.Areas.Identity.Data
{
    public class Specialty
    {
        public Specialty()
        {
            Staffs = new HashSet<Staff>();
            Services = new HashSet<Service>();
        }
        [Key]
        public int SpecialtyID { get; set; }
        public string SpecialtyName { get; set; }

        [InverseProperty(nameof(Staff.Specialty))]
        public virtual ICollection<Staff> Staffs { get; set; }

        [InverseProperty(nameof(Service.Specialty))]
        public virtual ICollection<Service> Services { get; set; }
    }
}
