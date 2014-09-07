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
		Constants.DatabaseService_AddAlarm_Action
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
			Log.Debug (TAG, "OnHandleIntent start");

			switch (intent.Action) {
			case Constants.DatabaseService_SendDatabaseState_Action:
				BroadcastAlarmsData ();
				break;
			case Constants.DatabaseService_AddAlarm_Action:
				AddAlarm (JsonConvert.DeserializeObject<AlarmData> (intent.GetStringExtra (Constants.AlarmsData_Extra)));
				break;
			}
		
			Log.Debug (TAG, "OnHandleIntent end");
		}

		void BroadcastAlarmsData ()
		{
			var alarms = ReadAlarmsFromDatabase ();
			SendBroadcast (new Intent (Constants.DatabaseUpdatesBroadcastReceiverAction)
				.PutExtra (Constants.AlarmsData_Extra, JsonConvert.SerializeObject (alarms)));
		}

		List<AlarmData> ReadAlarmsFromDatabase ()
		{
			List<AlarmData> alarms = null;

			using (var connection = CreateConnection ()) {
				//alarms = new List<AlarmData> () {
				//	new AlarmData () { Latitude = 48, Longitude = 35, Radius = new Random ().Next (50, 100) * 200 },
				//};

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
	}
}