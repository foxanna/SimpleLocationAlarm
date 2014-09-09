using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLocationAlarm.Droid
{
    public class AlarmsEventArgs : EventArgs
    {
        public List<AlarmData> Data { get; private set; }

        public AlarmsEventArgs(List<AlarmData> data)
        {
            Data = data;
        }
    }

    public class DBManager
    {
        public event EventHandler<AlarmsEventArgs> DataUpdated;

        string dbPath = System.IO.Path.Combine(
                            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "alarms.db");

        SQLiteConnection CreateConnection()
        {
            var connection = new SQLiteConnection(dbPath);
            connection.CreateTable<AlarmData>();

            return connection;
        }

        object _lockObject = new object();

        public AlarmData GetAlarmByGeofenceRequestId(string id)
        {            
            using (var connection = CreateConnection())
            {
                lock (_lockObject)
                {
                    return connection.Table<AlarmData>().FirstOrDefault(alarm => id == alarm.RequestId);
                }
            }
        }

        public void AddAlarm(AlarmData alarm)
        {
            List<AlarmData> newAlarms = null;

            using (var connection = CreateConnection())
            {
                lock (_lockObject)
                {
                    var existingAlarm = connection.Table<AlarmData>().FirstOrDefault(a => a.RequestId.Equals(alarm.RequestId));

                    if (existingAlarm == null)
                    {
                        connection.Insert(alarm);
                    }
                    else
                    {
                        existingAlarm.Enabled = alarm.Enabled;
                        connection.Update(existingAlarm);
                    }

                    newAlarms = connection.Table<AlarmData>().ToList();
                }
            }

            OnDataUpdated(newAlarms);
        }
        
        public void DeleteAlarm(AlarmData alarmData)
        {
            List<AlarmData> newAlarms = null;

            using (var connection = CreateConnection())
            {
                lock (_lockObject)
                {
                    var alarm = connection.Table<AlarmData>().FirstOrDefault(a => a.Latitude == alarmData.Latitude && a.Longitude == alarmData.Longitude);
                    connection.Delete(alarm);

                    newAlarms = connection.Table<AlarmData>().ToList();
                }
            }

            OnDataUpdated(newAlarms);
        }

        public void DeleteAlarm(string requestID)
        {
            List<AlarmData> newAlarms = null;

            using (var connection = CreateConnection())
            {
                lock (_lockObject)
                {
                    var alarm = connection.Table<AlarmData>().FirstOrDefault(a => requestID == a.RequestId);
                    connection.Delete(alarm);

                    newAlarms = connection.Table<AlarmData>().ToList();
                }
            }

            OnDataUpdated(newAlarms);
        }

        public void DisableAll()
        {
            List<AlarmData> newAlarms = null;

            using (var connection = CreateConnection())
            {
                lock (_lockObject)
                {
                    foreach (var alarm in connection.Table<AlarmData>())
                    {
                        alarm.Enabled = false;
                        connection.Update(alarm);
                    }
                    
                    newAlarms = connection.Table<AlarmData>().ToList();
                }
            }

            OnDataUpdated(newAlarms);
        }

        public void InvokeDataUpdate()
        {
            List<AlarmData> newAlarms = null;

            using (var connection = CreateConnection())
            {
                lock (_lockObject)
                {
                    newAlarms = connection.Table<AlarmData>().ToList();
                }
            }

            OnDataUpdated(newAlarms);
        }

        void OnDataUpdated(List<AlarmData> alarms)
        {
            var handler = DataUpdated;
            if (handler != null)
            {
                handler(this, new AlarmsEventArgs(alarms));
            }
        }

        public void DisableAlarm(string requestID)
        {
            List<AlarmData> newAlarms = null;

            using (var connection = CreateConnection())
            {
                lock (_lockObject)
                {
                    var existingAlarm = connection.Table<AlarmData>().FirstOrDefault(a => a.RequestId.Equals(requestID));
                    existingAlarm.Enabled = false;
                    connection.Update(existingAlarm);
                   
                    newAlarms = connection.Table<AlarmData>().ToList();
                }
            }

            OnDataUpdated(newAlarms);
        }
    }
}
