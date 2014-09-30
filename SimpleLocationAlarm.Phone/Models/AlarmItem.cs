namespace SimpleLocationAlarm.Phone.Models
{
    public class AlarmItem
    {
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public bool Enabled { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Radius { get; set; }
    }
}
