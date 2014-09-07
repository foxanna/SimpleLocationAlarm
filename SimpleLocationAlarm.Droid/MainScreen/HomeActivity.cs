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

		void OnDataUpdated (object sender, EventArgs e)
		{
			Toast.MakeText (this, "OnDataUpdated", ToastLength.Short).Show ();

			RedrawMapData ();
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