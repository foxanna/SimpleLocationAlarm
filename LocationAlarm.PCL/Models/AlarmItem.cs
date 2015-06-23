using SQLite.Net.Attributes;

namespace LocationAlarm.PCL.Models
{
    public class AlarmItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; protected set; }

        public string Title { get; set; }
        public bool Enabled { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int Radius { get; set; }
        public string GeofenceId { get; set; }
    }
}
