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

namespace SimpleLocationAlarm.Droid.MainScreen
{
	[Activity (
		Label = "@string/app_name", 
		MainLauncher = true, 
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]

	public partial class HomeActivity : ActionBarActivity
	{
		const string TAG = "HomeActivity";

		DataBaseUpdatesBroadcastReceiver DataBaseUpdatesListener = new DataBaseUpdatesBroadcastReceiver ();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

            _alarm_marker_normal = BitmapDescriptorFactory.FromResource(Resource.Drawable.alarm_marker_normal);
            _alarm_normal_selected = BitmapDescriptorFactory.FromResource(Resource.Drawable.alarm_marker_selected);
		}

		protected override void OnStart ()
		{
			base.OnStart ();
			DataBaseUpdatesListener.DataBaseUpdated = OnDataUpdated;

			FindMap ();
		}

		protected override void OnStop ()
		{
			LooseMap ();

			DataBaseUpdatesListener.DataBaseUpdated = null;
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