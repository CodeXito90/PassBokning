using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PassBokning.Models;

namespace PassBokning.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<GymClass> GymClasses { get; set; }
        public DbSet<ApplicationUserGymClass> ApplicationUserGymClasses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasKey(aug => new { aug.ApplicationUserId, aug.GymClassId });

            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasOne(aug => aug.ApplicationUser)
                .WithMany(u => u.AttendedClasses)
                .HasForeignKey(aug => aug.ApplicationUserId);

            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasOne(aug => aug.GymClass)
                .WithMany(gc => gc.AttendingMembers)
                .HasForeignKey(aug => aug.GymClassId);
        }
    }
}