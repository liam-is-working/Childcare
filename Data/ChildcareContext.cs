using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Childcare.Areas.Identity.Data;

namespace Childcare.Data
{
    public class ChildcareContext : DbContext
    {
        public ChildcareContext(DbContextOptions<ChildcareContext> options)
            : base(options)
        {
        }


        public DbSet<Childcare.Areas.Identity.Data.Administrator> Administrators { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.BlogCategory> BlogCategories { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.Blog> Blogs { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.Customer> Customers { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.Feedback> Feedbacks { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.Manager> Managers { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.MedicalExamination> MedicalExaminations { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.Patient> Patients { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.Reservation> Reservations { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.Service> Services { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.Specialty> Specialties { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.Staff> Staffs { get; set; }
        public DbSet<Childcare.Areas.Identity.Data.Status> Statuses { get; set; }
    }
}
