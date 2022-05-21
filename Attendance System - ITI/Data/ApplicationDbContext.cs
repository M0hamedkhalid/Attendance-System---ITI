using Attendance_System___ITI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Attendance_System___ITI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options )
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Department>()
            .HasOne(i => i.Manger)
            .WithOne(m => m.ManagedDepartment).HasForeignKey<Department>(d => d.MangerId);


            builder.Entity<Department>().HasMany(d => d.Instructors)
                        .WithOne(i => i.Department).HasForeignKey(i => i.DeptID);
            base.OnModelCreating(builder);

            builder.Entity<Student>().Property(std => std.Warning).HasDefaultValue(0);
        }
    }
}