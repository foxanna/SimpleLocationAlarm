using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LocationAlarm.PCL.Models;

namespace LocationAlarm.PCL.Services
{
	public class AlarmsManager : IAlarmsManager
	{
		const string Tag = "AlarmsManager";

		readonly IDatabaseManager DatabaseManager;
		readonly IGeofenceManager GeofenceManager;
		readonly ILogService LogService;

		public AlarmsManager(IDatabaseManager databaseManager,
			IGeofenceManager geofenceManager,
			ILogService logService)
		{
			LogService = logService;
			DatabaseManager = databaseManager;
			DatabaseManager.CreateTable<AlarmItem>();
			GeofenceManager = geofenceManager;
		}

		public IReadOnlyCollection<AlarmItem> Alarms
		{
			get { return DatabaseManager.GetAll<AlarmItem>().ToArray(); }
		}

		public event EventHandler AlarmsSetChanged;

		void OnAlarmsSetChanged()
		{
			var handler = AlarmsSetChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		public async Task Remove(AlarmItem alarm)
		{
			try
			{
				await GeofenceManager.RemoveGeofence(alarm.GeofenceId);
				DatabaseManager.Delete(alarm);
				OnAlarmsSetChanged();
			}
			catch (Exception e)
			{
				LogService.Log(Tag, e);
				throw;
			}
		}

		public async Task AddAlarm(AlarmItem alarm)
		{
			try
			{
				alarm.GeofenceId = Guid.NewGuid().ToString();

				await GeofenceManager.AddGeofence(alarm.GeofenceId, alarm.Latitude, alarm.Longitude, alarm.Radius);
				DatabaseManager.Add(alarm);
				OnAlarmsSetChanged();
			}
			catch (Exception e)
			{
				LogService.Log(Tag, e);
				throw;
			}
		}

		public async Task SwitchEnabled(AlarmItem alarm)
		{
			try
			{
				var newEnabledValue = !alarm.Enabled;

				if (newEnabledValue)
					await GeofenceManager.AddGeofence(alarm.GeofenceId, alarm.Latitude, alarm.Longitude, alarm.Radius);
				else
					await GeofenceManager.RemoveGeofence(alarm.GeofenceId);

				alarm.Enabled = newEnabledValue;
				DatabaseManager.Update(alarm);
				OnAlarmsSetChanged();
			}
			catch (Exception e)
			{ 
				LogService.Log(Tag, e);
				throw;
			}
		}
	}
}