namespace LocationAlarm.PCL.Services
{
    public interface IGeofenceManager
    {
        void AddGeofence(string id, double latitude, double longitude, int radius);
        void RemoveGeofence(string id);
    }
}
