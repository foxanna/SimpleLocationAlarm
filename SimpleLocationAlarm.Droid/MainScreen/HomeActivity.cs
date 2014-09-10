using System;

using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.OS;
using Android.Renderscripts;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps.Model;
using Android.Gms.Location;

namespace SimpleLocationAlarm.Droid.MainScreen
{
	[Activity (
		Label = "@string/app_name", 
        Icon = "@drawable/alarm_white",
		MainLauncher = true, 
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]

	public partial class HomeActivity : ActionBarActivity
	{
		const string TAG = "HomeActivity";

        DBManager _dbManager = new DBManager();
        GeofenceManager _geofenceManager = new GeofenceManager();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
		}

		protected override void OnStart ()
		{
			base.OnStart ();
            _dbManager.DataUpdated += OnDataUpdated;

            _geofenceManager.Error += OnGeofenceManagerError;
            _geofenceManager.Started += OnGeofenceManagerStarted;
            _geofenceManager.FailedToStart += OnGeofenceManagerFailedToStart;
            _geofenceManager.FailedToStartWithResolution += OnGeofenceManagerFailedToStartWithResolution;
            _geofenceManager.Stoped += OnGeofenceManagerStoped;
            _geofenceManager.GeofenceAdded += OnGeofenceManagerGeofenceAdded;
            _geofenceManager.GeofenceRemoved += OnGeofenceManagerGeofenceRemoved;

			FindMap ();
		}
                
        protected override void OnStop ()
		{
			LooseMap ();

            _geofenceManager.GeofenceRemoved -= OnGeofenceManagerGeofenceRemoved;
            _geofenceManager.GeofenceAdded -= OnGeofenceManagerGeofenceAdded;
            _geofenceManager.Stoped -= OnGeofenceManagerStoped;
            _geofenceManager.FailedToStart -= OnGeofenceManagerFailedToStart;
            _geofenceManager.FailedToStartWithResolution -= OnGeofenceManagerFailedToStartWithResolution;
            _geofenceManager.Started -= OnGeofenceManagerStarted;
            _geofenceManager.Error -= OnGeofenceManagerError;
            _dbManager.DataUpdated -= OnDataUpdated;
			base.OnStop ();
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			CheckGS ();
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			switch (requestCode) {
			case _googleServicesCheckRequestCode:
				OnActivityResultForGS (resultCode);
				break;
            case GeofenceManager.ConnectionFailedRequestCode:
                OnActivityResultForLM(resultCode);
                break;
			default:
				base.OnActivityResult (requestCode, resultCode, data);
				break;
			}
		}

		public override void OnBackPressed ()
		{
			if (Mode != Mode.None) {
				Mode = Mode.None;
			} else {
				base.OnBackPressed ();
			}
		}
	}
}