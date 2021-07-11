using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Childcare.Areas.Identity.Data
{
    public partial class Status
    {
        public Status()
        {
            Blogs = new HashSet<Blog>();
            Services = new HashSet<Service>();
        }
        [Key]
        public int StatusID { get; set; }
        public string StatusName { get; set; }

        [InverseProperty(nameof(Blog.Status))]
        public virtual ICollection<Blog> Blogs { get; set; }

        [InverseProperty(nameof(Service.Status))]
        public virtual ICollection<Service> Services { get; set; }

    }

    public enum StatusName{
        Approved = 1,
        Rejected = 2,
        Pending = 3
    }
}
