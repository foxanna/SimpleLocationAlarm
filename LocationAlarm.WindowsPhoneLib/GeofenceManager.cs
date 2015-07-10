using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LocationAlarm.PCL.Services;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace LocationAlarm.WindowsPhoneLib
{
	class GeofenceManager : IGeofenceManager
	{
		const string Tag = "GeofenceManager";

		readonly IUIThreadProvider UIThreadProvider;
		readonly ILogService LogService;

		//readonly TaskScheduler TaskScheduler;

		public GeofenceManager(IUIThreadProvider uiThreadProvider,
			ILogService logService)
		{
			LogService = logService;
			UIThreadProvider = uiThreadProvider;
			//TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
		}

		public async Task AddGeofence(string id, double latitude, double longitude, int radius)
		{
			try
			{
				await UIThreadProvider.RunInUIThread(() =>
				//await Task.Factory.StartNew(() =>
				{
					var geoCircle = new Geocircle(new BasicGeoposition
					{
						Latitude = latitude,
						Longitude = longitude
					}, radius);

					var geofence = new Geofence(id, geoCircle, MonitoredGeofenceStates.Entered, false);
					GeofenceMonitor.Current.Geofences.Add(geofence);
					//}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler);
				});

			}
			catch (Exception e)
			{
				LogService.Log(Tag, e);
				throw;
			}
		}

		public async Task RemoveGeofence(string id)
		{
			try
			{
				await UIThreadProvider.RunInUIThread(() =>
				//await Task.Factory.StartNew(() =>
				{
					var geofence = GeofenceMonitor.Current.Geofences.Where(p => p.Id == id).FirstOrDefault();
					if (geofence != null)
						GeofenceMonitor.Current.Geofences.Remove(geofence);
					//}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler);
				});
			}
			catch (Exception e)
			{
				LogService.Log(Tag, e);
				throw;
			}
		}
	}
}
