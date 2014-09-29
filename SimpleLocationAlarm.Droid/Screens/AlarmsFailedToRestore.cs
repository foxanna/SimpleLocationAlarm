using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;

namespace SimpleLocationAlarm.Droid.Screens
{
	[Activity (
		Label = "@string/app_name",
		Icon = "@drawable/map_white",
		Theme = "@android:style/Theme.Translucent.NoTitleBar",
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class AlarmsFailedToRestore : Activity
	{
		Button _keepInactiveButton, _activateManuallyButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			//Resource.Dimension.abc_action_bar_default_height
			SetContentView (Resource.Layout.AlarmsFailedToRestore);

			_keepInactiveButton = FindViewById<Button> (Resource.Id.keep_inactive_button);
			_activateManuallyButton = FindViewById<Button> (Resource.Id.activate_manually_button);

			_keepInactiveButton.Click += (sender, e) => Finish ();
			_activateManuallyButton.Click += (sender, e) => {
				StartActivity (new Intent (this, typeof(HomeActivity)));
				Finish ();
			};
		}
	}
}

