using System.Linq;
using LocationAlarm.PCL.Services;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace LocationAlarm.WindowsPhoneLib
{
    class GeofenceManager : IGeofenceManager
    {
        public void AddGeofence(string id, double latitude, double longitude, int radius)
        {
            var geoCircle = new Geocircle(new BasicGeoposition
            {
                Latitude = latitude,
                Longitude = longitude
            }, radius);

            var geofence = new Geofence(id, geoCircle, MonitoredGeofenceStates.Entered, false);
            GeofenceMonitor.Current.Geofences.Add(geofence);
        }

        public void RemoveGeofence(string id)
        {
            var geofence = GeofenceMonitor.Current.Geofences.Where(p => p.Id == id).FirstOrDefault();
            if (geofence != null)
                GeofenceMonitor.Current.Geofences.Remove(geofence);
        }
    }
}
