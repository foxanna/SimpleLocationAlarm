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

        public void Remove(AlarmItem alarm)
        {
            DatabaseManager.Delete(alarm);
        }

        public void AddAlarm(AlarmItem alarm)
        {
            DatabaseManager.Add(alarm);
        }

        public void SwitchEnabled(AlarmItem alarm)
        {
            //DatabaseManager.GetById<AlarmItem>()
            alarm.Enabled = !alarm.Enabled;
            DatabaseManager.Update(alarm);
        }
    }
}