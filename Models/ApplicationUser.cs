using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PassBokning.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public DateTime TimeOfRegistration { get; set; }
        // Navigation property
        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }
    }
}
