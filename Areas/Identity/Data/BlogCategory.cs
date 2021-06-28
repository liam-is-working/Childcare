using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Childcare.Areas.Identity.Data
{
    public partial class BlogCategory
    {
        public BlogCategory()
        {
            Blogs = new HashSet<Blog>();
        }
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        [InverseProperty(nameof(Blog.BlogCategory))]
        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
