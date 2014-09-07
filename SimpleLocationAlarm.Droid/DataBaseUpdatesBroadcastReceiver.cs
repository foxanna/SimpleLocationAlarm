using System;
using System.Linq;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Newtonsoft.Json;

namespace SimpleLocationAlarm.Droid
{
	public class AlarmsEventArgs : EventArgs
	{
		public List<AlarmData> Data { get; private set; }

		public AlarmsEventArgs (List<AlarmData> data)
		{
			Data = data;
		}
	}

	public class DataBaseUpdatesBroadcastReceiver : BroadcastReceiver
	{
		EventHandler<AlarmsEventArgs> _dataBaseUpdated;

		public EventHandler<AlarmsEventArgs> DataBaseUpdated {
			get {
				return _dataBaseUpdated;
			}
			set {
				if (_dataBaseUpdated != null) {
					Android.App.Application.Context.UnregisterReceiver (this); 
				}

				_dataBaseUpdated = value;

				if (_dataBaseUpdated != null) {
					Android.App.Application.Context.RegisterReceiver (this, 
						new IntentFilter (Constants.DatabaseUpdatesBroadcastReceiverAction)); 
				}
			}
		}

		public override void OnReceive (Context context, Intent intent)
		{
			if (Constants.DatabaseUpdatesBroadcastReceiverAction.Equals (intent.Action)) {	
				var alarms = JsonConvert.DeserializeObject<List<AlarmData>> (
					             intent.GetStringExtra (Constants.AlarmsData_Extra));
				var handler = DataBaseUpdated;
				if (handler != null) {
					handler (this, new AlarmsEventArgs (alarms));
				}
			}
		}
	}
}