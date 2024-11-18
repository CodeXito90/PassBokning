using System.ComponentModel.DataAnnotations;

namespace PassBokning.Models.ViewModels
{
    public class CreateGymClassViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Namn måste anges")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Namnet måste vara mellan 2 och 50 tecken")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Starttid måste anges")]
        [Display(Name = "Start Tid")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Längd måste anges")]
        //[Range(typeof(TimeSpan), "00:15", "04:00", ErrorMessage = "Längden måste vara mellan 15 minuter och 4 timmar")]
        public TimeSpan Duration { get; set; }

        [Required(ErrorMessage = "Beskrivning måste anges")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Beskrivningen måste vara mellan 10 och 200 tecken")]
        public string Description { get; set; }

        public bool IsBooked { get; set; }
        public bool IsPassed => StartTime < DateTime.Now;
        public int AttendeeCount { get; set; }
    }
}