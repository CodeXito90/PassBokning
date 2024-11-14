using System.ComponentModel.DataAnnotations;

namespace PassBokning.Models
{
    public class GymClass
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        public DateTime EndTime { get { return StartTime + Duration; } }

        [StringLength(200)]
        public string Description { get; set; }

        public ICollection<ApplicationUserGymClass> AttendingMembers { get; set; } = new List<ApplicationUserGymClass>();
    }
}