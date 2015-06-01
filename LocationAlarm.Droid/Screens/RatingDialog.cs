using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Preferences;
using SimpleLocationAlarm.Droid.Services;

namespace SimpleLocationAlarm.Droid.Screens
{
	[Activity (
		Label = "@string/app_name",
		Icon = "@drawable/map_white",
		Theme = "@android:style/Theme.Translucent.NoTitleBar",
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class RatingDialog : Activity
	{
		Button _cancelButton, _rateButton, _remindLaterButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.RatingDialog);

			_cancelButton = FindViewById<Button> (Resource.Id.never_ask);
			_rateButton = FindViewById<Button> (Resource.Id.rate_now);
			_remindLaterButton = FindViewById<Button> (Resource.Id.rate_later);

			var prefs = PreferenceManager.GetDefaultSharedPreferences (this);

			_cancelButton.Click += (sender, e) => {
				prefs.Edit ().PutBoolean (SettingsScreen.ShouldAskForRating, false).Commit ();
				Finish ();
			};
			_rateButton.Click += (sender, e) => {
				prefs.Edit ().PutBoolean (SettingsScreen.ShouldAskForRating, false).Commit ();

				var intent = new Intent (Intent.ActionView);
				intent.SetData (Android.Net.Uri.Parse ("market://details?id=" + PackageName));
				StartActivity (intent);

				Finish ();
			};
			_remindLaterButton.Click += (sender, e) => {
				prefs.Edit ().PutInt (SettingsScreen.StartsCount, 0).Commit ();
				Finish ();
			};
		}
	}
}

