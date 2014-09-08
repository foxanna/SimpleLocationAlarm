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
using Newtonsoft.Json;

namespace SimpleLocationAlarm.Droid
{
    [BroadcastReceiver]
    [IntentFilter(new String[] { 
        Android.Content.Intent.ActionBootCompleted,
        Constants.DatabaseUpdated_Action 
    })]
    public class GeofencesServiceBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug("GeofencesServiceBroadcastReceiver", "OnReceive " + intent.Action);
            context.StartService(new Intent(context, typeof(GeofencesAlarmService))
                .SetAction(intent.Action)
                .PutExtras(intent.Extras));
        }
    }

    [Service]
    public class GeofencesAlarmService : IntentService
    {
        const string TAG = "GeofencesAlarmService";

        public GeofencesAlarmService()
        {

        }

        //  private LocationClient _locationClient;

        protected override void OnHandleIntent(Intent intent)
        {
            Log.Debug(TAG, "OnHandleIntent start because of " + intent.Action);

            switch (intent.Action)
            {
                case Android.Content.Intent.ActionBootCompleted:
                    AskForDBData();
                    break;
                case Constants.DatabaseUpdated_Action:
                    var alarms = JsonConvert.DeserializeObject<List<AlarmData>>(
                                 intent.GetStringExtra(Constants.AlarmsData_Extra));
                    ProceedNewAlarms(alarms);
                    break;
            }

            Log.Debug(TAG, "OnHandleIntent end");
        }

        void AskForDBData()
        {
            StartService(new Intent(Constants.DatabaseService_SendDatabaseState_Action));
        }

        void ProceedNewAlarms(List<AlarmData> alarms)
        {
            Log.Debug(TAG, "ProceedNewAlarms, count = " + alarms.Count);
        }
    }
}