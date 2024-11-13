namespace PassBokning.Models
{
    public class GymClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        DateTime StartTime { get; set; }
        TimeSpan Duration { get; set; }
        DateTime EndTime { get {  return StartTime + Duration; } }
        public string Description { get; set; }
    }
}
