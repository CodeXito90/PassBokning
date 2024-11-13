namespace PassBokning.Models
{
    public class ApplicationUserGymClass
    {
        public int ApplicationUserId { get; set; }
        public int GymClassId { get; set; }

        public GymClass GymClass { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
