using System.Threading.Tasks;

namespace LocationAlarm.PCL.Services
{
    public interface IGeofenceManager
    {
        Task AddGeofence(string id, double latitude, double longitude, int radius);
		Task RemoveGeofence(string id);
    }
}
