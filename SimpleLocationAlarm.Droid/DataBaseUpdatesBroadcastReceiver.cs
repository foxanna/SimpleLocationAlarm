using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;

namespace SimpleLocationAlarm.Droid
{
	public class DataBaseUpdatesBroadcastReceiver : BroadcastReceiver
	{
		EventHandler _dataBaseUpdated;

		public EventHandler DataBaseUpdated {
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
						new IntentFilter (Constants.DataBaseUpdatesBroadcastReceiverAction)); 
				}
			}
		}

		public override void OnReceive (Context context, Intent intent)
		{
			if (Constants.DataBaseUpdatesBroadcastReceiverAction.Equals (intent.Action)) {			
				var handler = DataBaseUpdated;
				if (handler != null) {
					handler (this, EventArgs.Empty);
				}
			}
		}
	}
}