using SimpleLocationAlarm.Phone.Models;
using Windows.Devices.Geolocation;

namespace SimpleLocationAlarm.Phone.ViewModels
{
    public class AlarmItemViewModel
    {
        public AlarmItemViewModel(AlarmItem item)
        {
            Title = item.Title;
            Enabled = item.Enabled;
            Radius = item.Radius;
            Location = new Geopoint(new BasicGeoposition() { Latitude = item.Latitude, Longitude = item.Longitude });
            Anchor = ".5,1";
        }

        public string Title { get; set; }
        public bool Enabled { get; set; }
        public double Radius { get; set; }        
        public Geopoint Location { get; private set; }
        public string Anchor { get; private set; }      
    }
}
