using System;
using System.Collections.Generic;
using System.Linq;
using LocationAlarm.PCL.Models;

namespace LocationAlarm.PCL.Services
{
    public class AlarmsManager : IAlarmsManager
    {
        readonly IDatabaseManager DatabaseManager;
        readonly IGeofenceManager GeofenceManager;

        public AlarmsManager(IDatabaseManager databaseManager,
            IGeofenceManager geofenceManager)
        {
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

        public void Remove(AlarmItem alarm)
        {
            try
            {
                GeofenceManager.RemoveGeofence(alarm.GeofenceId);
                DatabaseManager.Delete(alarm);
                OnAlarmsSetChanged();
            }
            catch { }
        }

        public void AddAlarm(AlarmItem alarm)
        {
            try
            {
                alarm.GeofenceId = Guid.NewGuid().ToString();

                GeofenceManager.AddGeofence(alarm.GeofenceId, alarm.Latitude, alarm.Longitude, alarm.Radius);
                DatabaseManager.Add(alarm);
                OnAlarmsSetChanged();
            }
            catch { }
        }

        public void SwitchEnabled(AlarmItem alarm)
        {
            try
            {
                var newEnabledValue = !alarm.Enabled;

                if (newEnabledValue)
                    GeofenceManager.AddGeofence(alarm.GeofenceId, alarm.Latitude, alarm.Longitude, alarm.Radius);
                else
                    GeofenceManager.RemoveGeofence(alarm.GeofenceId);
                
                alarm.Enabled = newEnabledValue;
                DatabaseManager.Update(alarm);
                OnAlarmsSetChanged();
            }
            catch { }
        }
    }
}