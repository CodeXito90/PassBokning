namespace PassBokning.Models.ViewModels
{
    public class IndexGymClassViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }

        public bool Attending { get; set; }
    }
}
