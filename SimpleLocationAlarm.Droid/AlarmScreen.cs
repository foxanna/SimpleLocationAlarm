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
using Android.Gms.Location;
using Android.Util;

namespace SimpleLocationAlarm.Droid
{
    [Activity(Label = "AlarmScreen")]
    public class AlarmScreen : Activity
    {
        const string TAG = "AlarmScreen";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (LocationClient.HasError(Intent))
            {
                Log.Debug(TAG, "OnCreate : LocationClient HasError");
                StartService(new Intent(this, typeof(DBService)).SetAction(Constants.DatabaseService_DeleteAll_Action));

                SetContentView(Resource.Layout.AlarmError);
            }
            else
            {
                Log.Debug(TAG, "OnCreate trigered by geofences");
                var geofences = LocationClient.GetTriggeringGeofences(Intent);
            }
        }
    }
}