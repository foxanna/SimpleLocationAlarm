using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocationAlarm.PCL.Models;
using LocationAlarm.PCL.Services.Database;
using LocationAlarm.PCL.Services.Logs;

namespace LocationAlarm.PCL.Services.Alarms
{
    public class AlarmsManager : IAlarmsManager
    {
        private const string Tag = "AlarmsManager";

        private readonly IDatabaseManager _databaseManager;
        private readonly IGeofenceManager _geofenceManager;
        private readonly ILogService _logService;

        public AlarmsManager(IDatabaseManager databaseManager,
            IGeofenceManager geofenceManager,
            ILogService logService)
        {
            _logService = logService;
            _databaseManager = databaseManager;
            _geofenceManager = geofenceManager;

            _databaseManager.CreateTable<AlarmItem>();
        }

        public IReadOnlyCollection<AlarmItem> Alarms => _databaseManager.GetAll<AlarmItem>().ToArray();

        public event EventHandler AlarmsSetChanged;

        public async Task Remove(AlarmItem alarm)
        {
            try
            {
                await _geofenceManager.RemoveGeofence(alarm.GeofenceId);
                _databaseManager.Delete(alarm);
                OnAlarmsSetChanged();
            }
            catch (Exception e)
            {
                _logService.Log(Tag, e);
                throw;
            }
        }

        public async Task AddAlarm(AlarmItem alarm)
        {
            try
            {
                alarm.GeofenceId = Guid.NewGuid().ToString();

                await _geofenceManager.AddGeofence(alarm.GeofenceId, alarm.Latitude, alarm.Longitude, alarm.Radius);
                _databaseManager.Add(alarm);
                OnAlarmsSetChanged();
            }
            catch (Exception e)
            {
                _logService.Log(Tag, e);
                throw;
            }
        }

        public async Task SwitchEnabled(AlarmItem alarm)
        {
            try
            {
                var newEnabledValue = !alarm.Enabled;

                if (newEnabledValue)
                    await _geofenceManager.AddGeofence(alarm.GeofenceId, alarm.Latitude, alarm.Longitude, alarm.Radius);
                else
                    await _geofenceManager.RemoveGeofence(alarm.GeofenceId);

                alarm.Enabled = newEnabledValue;
                _databaseManager.Update(alarm);
                OnAlarmsSetChanged();
            }
            catch (Exception e)
            {
                _logService.Log(Tag, e);
                throw;
            }
        }

        private void OnAlarmsSetChanged()
        {
            AlarmsSetChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}