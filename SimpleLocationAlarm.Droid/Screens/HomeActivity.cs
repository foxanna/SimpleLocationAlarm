using Android.App;
using Android.Content;
using Android.OS;
using SimpleLocationAlarm.Droid.Services;

namespace SimpleLocationAlarm.Droid.Screens
{
	[Activity (
		Label = "@string/app_name", 
		Icon = "@drawable/alarm_white",
		MainLauncher = true, 
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]

    public partial class HomeActivity : BaseAlarmActivity
	{
        protected override string TAG
        {
            get
            {
                return "HomeActivity";
            }
        }

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			FindMap ();
		}

		protected override void OnStop ()
		{
			LooseMap ();

			if (UndoBar != null) {
				UndoBar.Hide ();
			}

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
				OnActivityResultForLM (resultCode);
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

        void OpenSettings()
        {
            var intent = new Intent(this, typeof(SettingsScreen));
            StartActivity(intent);
        }
	}
}