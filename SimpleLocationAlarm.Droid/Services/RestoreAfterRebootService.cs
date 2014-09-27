using System;
using System.Linq;
using Android.Content;
using Android.App;
using System.Collections.Generic;
using Android.Util;
using SimpleLocationAlarm.Droid.Screens;
using Android.Widget;

namespace SimpleLocationAlarm.Droid.Services
{
	[BroadcastReceiver]
	[IntentFilter (new []{ Intent.ActionBootCompleted, "android.intent.action.QUICKBOOT_POWERON" }
		, Categories = new[] { Intent.CategoryDefault }
	)]
	public class PhoneBootedBroadcastReceiver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			Log.Debug ("PhoneBootedBroadcastReceiver", "ActionBootCompleted received");

			var activeAlarms = new DBManager ().GetAll ().Where (alarm => alarm.Enabled).ToList ();

			if (activeAlarms != null && activeAlarms.Count > 0) {
				context.StartService (new Intent (context, typeof(RestoreAfterRebootService)));
			}
		}
	}

	[Service]
	public class RestoreAfterRebootService : Service
	{
		const string TAG = "RestoreAfterRebootService";

		public RestoreAfterRebootService ()
		{
		}

		public override Android.OS.IBinder OnBind (Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug (TAG, "ActionBootCompleted received");

			RestoreGeofences ();
			return StartCommandResult.Sticky;
		}

		List<AlarmData> _activeAlarms;
		GeofenceManager _geofenceManager;
		DBManager _dbManager;

		bool _successfullyRestored;

		void RestoreGeofences ()
		{
			_dbManager = new DBManager ();
			_activeAlarms = _dbManager.GetAll ().Where (alarm => alarm.Enabled).ToList ();
			_dbManager.EnableAlarms (_activeAlarms, false);

			_geofenceManager = new GeofenceManager ();
			_geofenceManager.Error += (sender, e) => AnyError ();
			_geofenceManager.Started += HandleStarted;
			_geofenceManager.FailedToStart += (sender, e) => AnyError ();
			_geofenceManager.FailedToStartWithResolution += (sender, e) => AnyError ();
			_geofenceManager.Stoped += HandleStoped;
			_geofenceManager.GeofenceAdded += HandleGeofenceAdded;

			_geofenceManager.Start ();
		}

		void HandleStarted (object sender, EventArgs e)
		{			
			Log.Debug (TAG, "GeofenceManager started");
			var geofences = _activeAlarms.Select (alarm => 
				_geofenceManager.PrepareGeofence (alarm.RequestId, alarm.Latitude, alarm.Longitude, alarm.Radius)).ToList ();
			_geofenceManager.AddGeofences (geofences);
		}

		void HandleGeofenceAdded (object sender, GeofenceChangeEventArgs e)
		{
			_successfullyRestored = e.RequestId.SequenceEqual (_activeAlarms.Select (alarm => alarm.RequestId).ToArray ());
			Log.Debug (TAG, "HandleGeofenceAdded " + _successfullyRestored);
			_dbManager.EnableAlarms (
				_activeAlarms.Where (alarm => e.RequestId.Contains (alarm.RequestId)).ToList (), true);

			_geofenceManager.Stop ();
		}

		void HandleStoped (object sender, EventArgs e)
		{
			Log.Debug (TAG, "HandleStoped");

			if (!_successfullyRestored) {
				AnyError ();
			} else {
				StopSelf ();
			}
		}

		void AnyError ()
		{
			Log.Debug (TAG, "AnyError");
			StartActivity (new Intent (this, typeof(AlarmsFailedToRestore)).AddFlags (ActivityFlags.NewTask));

			StopSelf ();
		}

		public override void OnDestroy ()
		{
			Log.Debug (TAG, "OnDestroy");

			if (_geofenceManager != null) {
				_geofenceManager.GeofenceAdded -= HandleGeofenceAdded;
				_geofenceManager.Stoped -= HandleStoped;
				_geofenceManager.Started -= HandleStarted;

				_geofenceManager = null;
			}

			_dbManager = null;

			base.OnDestroy ();
		}
	}
}