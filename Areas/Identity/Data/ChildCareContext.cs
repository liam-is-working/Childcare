using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Childcare.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Childcare.Areas.Identity.Data
{
    public class ChildCareContext : IdentityDbContext<ChildCareUser>
    {
        public ChildCareContext(DbContextOptions<ChildCareContext> options)
            : base(options)
        {
        }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<MedicalExamination> MedicalExaminations { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Staff> Staffs { get; set; }

        public DbSet<ReservationTime> ReservationTimes {get;set;}
        public DbSet<Status> Statuses { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<ChildCareUser>().Property(c => c.Id).HasColumnName("ChildCareUserId");
            builder.Entity<ReservationTime>().HasKey("Date", "ServiceID", "Slot");

            // builder.Entity<Administrator>()
            //    .HasOne(t => t.ChildcareUserId);
            // builder.Entity<Manager>()
            //     .HasOne(t => t.ChildcareUserId);
            // builder.Entity<Customer>()
            //     .HasOne(t => t.ChildcareUserId);
            // builder.Entity<Staff>()
            //     .HasOne(t => t.ChildcareUserId);
        }
    }
}
