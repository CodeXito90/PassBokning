using Microsoft.AspNetCore.Identity;

namespace PassBokning.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            AttendedClasses = new List<ApplicationUserGymClass>();
        }

        // Navigation property
        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }
    }
}
