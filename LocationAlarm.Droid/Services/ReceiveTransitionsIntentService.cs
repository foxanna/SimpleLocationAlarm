using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Util;
using Newtonsoft.Json;
using SimpleLocationAlarm.Droid.Screens;

namespace SimpleLocationAlarm.Droid.Services
{
    [Service]
    public class ReceiveTransitionsIntentService : IntentService
    {
        private const string TAG = "ReceiveTransitionsIntentService";

        protected override void OnHandleIntent(Intent intent)
        {
            if (LocationClient.HasError(intent))
            {
                Log.Debug(TAG, "Location Services error");
                StartActivity(new Intent(this, typeof (AlarmErrorScreen)).AddFlags(ActivityFlags.NewTask));
            }
            else
            {
                Log.Debug(TAG, "Location Services success");

                var geofences = LocationClient.GetTriggeringGeofences(intent);
                if (geofences == null || geofences.Count <= 0) return;
                var requestId = geofences[0].RequestId;

                var dbManager = new DBManager();
                var alarm = dbManager.GetAlarmByGeofenceRequestId(requestId);

                if (alarm == null) return;

                StartActivity(intent.SetClass(this, typeof (AlarmScreen))
                    .AddFlags(ActivityFlags.NewTask)
                    .PutExtra(Constants.AlarmsData_Extra, JsonConvert.SerializeObject(alarm)));
                StartService(new Intent(this, typeof (UIWhileRingingIntentService))
                    .SetAction(UIWhileRingingIntentService.StartAlarmAction)
                    .PutExtra(Constants.AlarmsData_Extra, JsonConvert.SerializeObject(alarm)));
            }
        }
    }
}