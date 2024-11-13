using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PassBokning.Models;

namespace PassBokning.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet <GymClass> GymClass { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasKey(a => new { a.ApplicationUserId, a.GymClassId });

            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasOne(a => a.ApplicationUser)
                .WithMany(u => u.AttendedClasses)
                .HasForeignKey(a => a.ApplicationUserId);

            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasOne(a => a.GymClass)
                .WithMany(gc => gc.AttendingMembers)
                .HasForeignKey(a => a.GymClassId);
        }

    }
}
