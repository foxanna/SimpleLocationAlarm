using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace SimpleLocationAlarm.Droid.Screens
{
    [Activity(
        Icon = "@drawable/alarm_white",
        LaunchMode = Android.Content.PM.LaunchMode.SingleTask,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class AlarmErrorScreen : ActionBarActivity
    {
        Button _goToSettingsButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AlarmError);
            
            _goToSettingsButton = FindViewById<Button>(Resource.Id.go_to_settings);
            _goToSettingsButton.Click += GoToSettingsButton_Click;
        }

        void GoToSettingsButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
            StartActivity(intent);
        }
    }
}