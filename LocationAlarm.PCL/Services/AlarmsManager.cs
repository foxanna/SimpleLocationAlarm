using System;
using System.Collections.Generic;
using System.Linq;
using LocationAlarm.PCL.Models;

namespace LocationAlarm.PCL.Services
{
    public class AlarmsManager : IAlarmsManager
    {
        readonly IDatabaseManager DatabaseManager;

        public AlarmsManager(IDatabaseManager databaseManager)
        {
            DatabaseManager = databaseManager;
            DatabaseManager.CreateTable<AlarmItem>();
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
            DatabaseManager.Delete(alarm);
            OnAlarmsSetChanged();
        }

        public void AddAlarm(AlarmItem alarm)
        {
            DatabaseManager.Add(alarm);
            OnAlarmsSetChanged();
        }

        public void SwitchEnabled(AlarmItem alarm)
        {
            alarm.Enabled = !alarm.Enabled;
            DatabaseManager.Update(alarm);
            OnAlarmsSetChanged();
        }
    }
}