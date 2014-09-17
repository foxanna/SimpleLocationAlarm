using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Android.Content;
using Android.Widget;
using Android.Graphics;
using Android.Net;

namespace SimpleLocationAlarm.Droid.Screens
{
    [Activity(Label = "@string/settings_about",
        Icon = "@drawable/alarm_white",
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    [IntentFilter(new[] { "openaboutscreenaction" }, Categories = new[] { Intent.CategoryDefault})]
    public class AboutScreen : ActionBarActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AboutScreen);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var packageInfo = PackageManager.GetPackageInfo (PackageName, 0);
            if (packageInfo != null)
            {
                var versionTextView = FindViewById<TextView>(Resource.Id.version);
                versionTextView.Text = string.Format(Resources.GetString(Resource.String.settings_version), packageInfo.VersionName, packageInfo.VersionCode);
            }

            var rateTextView = FindViewById<TextView>(Resource.Id.rate);
            rateTextView.PaintFlags = PaintFlags.UnderlineText;
            rateTextView.Click += RateTextViewClick;

            var feedbackTextView = FindViewById<TextView>(Resource.Id.send_feedback);
            feedbackTextView.PaintFlags = PaintFlags.UnderlineText;
            feedbackTextView.Click += FeedbackTextViewClick;
        }

        void FeedbackTextViewClick(object sender, System.EventArgs e)
        {
            var intent = new Intent(Intent.ActionSendto);
            intent.SetData(Android.Net.Uri.FromParts("mailto", Constants.DeveloperEmail, null));
            StartActivity(intent);
        }

        void RateTextViewClick(object sender, System.EventArgs e)
        {
            var intent = new Intent(Intent.ActionView);
            intent.SetData(Uri.Parse("market://details?id=" + PackageName));
            StartActivity(intent);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}