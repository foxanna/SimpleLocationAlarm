using System;
using Android.App;
using Android.Util;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Newtonsoft.Json;
using System.ComponentModel.Design.Serialization;
using SQLite;

namespace SimpleLocationAlarm.Droid
{
	[Service]
	[IntentFilter (new String[] { 
		Constants.DatabaseService_SendDatabaseState_Action,
		Constants.DatabaseService_AddAlarm_Action,
        Constants.DatabaseService_DeleteAlarm_Action
	})]
	public class DBService : IntentService
	{
		const string TAG = "DBService";

		public DBService () 
		// : base ("DBService") is said to be error generating 
		// http://developer.xamarin.com/guides/android/advanced_topics/limitations/
		{
		}

		string dbPath = System.IO.Path.Combine (
			                System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "alarms.db");

		SQLiteConnection CreateConnection ()
		{
			var connection = new SQLiteConnection (dbPath);
			connection.CreateTable<AlarmData> ();

			return connection;
		}

		protected override void OnHandleIntent (Intent intent)
		{
			Log.Debug (TAG, "OnHandleIntent start because of " + intent.Action);

			switch (intent.Action) {
			case Constants.DatabaseService_SendDatabaseState_Action:
				BroadcastAlarmsData ();
				break;
			case Constants.DatabaseService_AddAlarm_Action:
				AddAlarm (JsonConvert.DeserializeObject<AlarmData> (intent.GetStringExtra (Constants.AlarmsData_Extra)));
				break;
            case Constants.DatabaseService_DeleteAlarm_Action:
                DeleteAlarm(JsonConvert.DeserializeObject<AlarmData>(intent.GetStringExtra(Constants.AlarmsData_Extra)));
                break;
            case Constants.DatabaseService_DeleteAll_Action:
                DeleteAll();
                break;
			}
		
			Log.Debug (TAG, "OnHandleIntent end");
		}
        
		void BroadcastAlarmsData ()
		{
			var alarms = ReadAlarmsFromDatabase ();
            SendBroadcast(new Intent(Constants.DatabaseUpdated_Action)
				.PutExtra (Constants.AlarmsData_Extra, JsonConvert.SerializeObject (alarms)));

            Log.Debug(TAG, "New alarms broadcasted");
		}

		List<AlarmData> ReadAlarmsFromDatabase ()
		{
            var alarms = new List<AlarmData>();

			using (var connection = CreateConnection ()) {
				alarms = connection.Table<AlarmData> ().ToList ();
			}

			return alarms;
		}

		void AddAlarm (AlarmData alarm)
		{
			using (var connection = CreateConnection ()) {
				connection.Insert (alarm);
			}

			BroadcastAlarmsData ();
		}
    
        void DeleteAlarm(AlarmData alarmData)
        {
            using (var connection = CreateConnection())
            {
                var alarm = connection.Table<AlarmData>().FirstOrDefault(a => a.Latitude == alarmData.Latitude && a.Longitude == alarmData.Longitude);
                connection.Delete(alarm);
            }

            BroadcastAlarmsData();
        }

        void DeleteAll()
        {
            using (var connection = CreateConnection())
            {
                connection.DeleteAll<AlarmData>();
            }

            BroadcastAlarmsData();
        }
    }
}