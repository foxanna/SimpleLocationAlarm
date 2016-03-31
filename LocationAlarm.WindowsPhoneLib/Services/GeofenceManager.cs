using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using LocationAlarm.PCL.Services;
using LocationAlarm.PCL.Services.Logs;

namespace LocationAlarm.WindowsPhoneLib.Services
{
    internal class GeofenceManager : IGeofenceManager
    {
        private const string Tag = "GeofenceManager";

        private readonly ILogService _logService;
        private readonly IUIThreadProvider _uiThreadProvider;

        public GeofenceManager(IUIThreadProvider uiThreadProvider,
            ILogService logService)
        {
            _logService = logService;
            _uiThreadProvider = uiThreadProvider;
        }

        public async Task AddGeofence(string id, double latitude, double longitude, int radius)
        {
            try
            {
                await _uiThreadProvider.RunInUIThread(() =>
                {
                    var geoCircle = new Geocircle(new BasicGeoposition
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    }, radius);

                    var geofence = new Geofence(id, geoCircle, MonitoredGeofenceStates.Entered, false);
                    GeofenceMonitor.Current.Geofences.Add(geofence);
                });
            }
            catch (Exception e)
            {
                _logService.Log(Tag, e);
                throw;
            }
        }

        public async Task RemoveGeofence(string id)
        {
            try
            {
                await _uiThreadProvider.RunInUIThread(() =>
                {
                    var geofence = GeofenceMonitor.Current.Geofences.FirstOrDefault(p => p.Id == id);
                    if (geofence != null)
                        GeofenceMonitor.Current.Geofences.Remove(geofence);
                });
            }
            catch (Exception e)
            {
                _logService.Log(Tag, e);
                throw;
            }
        }
    }
}