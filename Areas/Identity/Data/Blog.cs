using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Childcare.Areas.Identity.Data
{
    public partial class Blog
    {
        [Key]
        public int BlogID { get; set; }
        public string Thumbnail { get; set; }
        public string Tile { get; set; }

        [Required]
        public int StaffID { get; set; }

        public string Description { get; set; }

        public DateTime CreadtedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
        [Required]
        public int StatusID { get; set; }
        [Required]
        public int CategoryID { get; set; }

        [ForeignKey(nameof(CategoryID))]
        [InverseProperty("Blogs")]
        public virtual BlogCategory BlogCategory { get; set; }

        [ForeignKey(nameof(StatusID))]
        [InverseProperty("Blogs")]
        public virtual Status Status { get; set; }

        [ForeignKey(nameof(StaffID))]
        [InverseProperty("Blogs")]
        public virtual Staff Staff { get; set; }
    }
}
