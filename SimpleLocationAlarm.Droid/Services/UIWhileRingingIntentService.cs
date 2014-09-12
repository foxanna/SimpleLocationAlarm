using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using SimpleLocationAlarm.Droid.Screens;
using Android.Media;
using Android.Util;
using Android.Support.V4.App;
using Newtonsoft.Json;

namespace SimpleLocationAlarm.Droid.Services
{
    [Service]
    public class UIWhileRingingIntentService : IntentService
    {
        public const string StartAlarmAction = "StartAlarmAction";
        public const string StopAlarmAction = "StopAlarmAction";
        
        const string TAG = "UIWhileRingingIntentService";

        protected override void OnHandleIntent(Intent intent)
        {
            Log.Debug(TAG, "OnHandleIntent" + intent.Action);

            switch (intent.Action)
            {
                case StartAlarmAction:
                    StartAlarm(intent);
                    break;
                case StopAlarmAction:
                    StopAlarm();
                    break;
            }
        }

        void StartAlarm(Intent intent)
        {
            var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            var shouldVibrate = sharedPreferences.GetBoolean(SettingsScreen.VibrateSettingKey, SettingsScreen.VibrateSettingDefaultValue);
            var shouldPlaySound = sharedPreferences.GetBoolean(SettingsScreen.PlaySoundSettingKey, SettingsScreen.PlaySoundSettingDefaultValue);
            var customSound = sharedPreferences.GetString(SettingsScreen.SoundSettingKey, "");

            if (shouldPlaySound)
            {
                PlaySound(customSound);
            }
            if (shouldVibrate)
            {
                StartVibrating();
            }

            ShowNotification(JsonConvert.DeserializeObject<AlarmData>(intent.GetStringExtra(Constants.AlarmsData_Extra)));
        }
        void StopAlarm()
        {
            StopPlaying();
            StopVibrating();
            HideNotification();
        }

        static Ringtone _ringtone;

        void PlaySound(string customSound)
        {
            if (_ringtone != null)
            {
                return;
            }

            var sound = string.IsNullOrEmpty(customSound) ? (RingtoneManager.GetDefaultUri(RingtoneType.Alarm) != null ?
                RingtoneManager.GetDefaultUri(RingtoneType.Alarm) : RingtoneManager.GetDefaultUri(RingtoneType.Ringtone)) : Android.Net.Uri.Parse(customSound);

            _ringtone = RingtoneManager.GetRingtone(Application.Context, sound);
            _ringtone.Play();
        }

        void StopPlaying()
        {
            if (_ringtone != null)
            {
                _ringtone.Stop();
                _ringtone = null;
            }
        }

        static Vibrator _vibrator;

        void StartVibrating()
        {
            if (_vibrator != null)
            {
                return;
            }

            _vibrator = (Vibrator) GetSystemService(Context.VibratorService);
            _vibrator.Vibrate(new long[] { 0, 1000, 1000 }, 0);
        }

        void StopVibrating()
        {
            if (_vibrator != null)
            {
                _vibrator.Cancel();
                _vibrator = null;
            }
        }

        int _notificationId = 67;

        void ShowNotification(AlarmData alarm)
        {
            var builder = new NotificationCompat.Builder(this);
            builder.SetSmallIcon(Resource.Drawable.alarm_white)
                .SetContentTitle(alarm.Name)
                .SetContentText(GetString(Resource.String.app_name))
                .SetAutoCancel(false);

            var intent = new Intent(this, typeof(AlarmScreen))
                .SetFlags(ActivityFlags.NewTask)
                .PutExtra(Constants.AlarmsData_Extra, JsonConvert.SerializeObject(alarm));
            builder.SetContentIntent(PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent));

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(_notificationId, builder.Build());
        }

        void HideNotification()
        {
            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Cancel(_notificationId);
        }        
    }
}