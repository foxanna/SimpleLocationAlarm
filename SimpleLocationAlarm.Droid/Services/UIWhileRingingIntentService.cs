using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using SimpleLocationAlarm.Droid.Screens;
using Android.Media;
using Android.Util;
using Android.Support.V4.App;
using Newtonsoft.Json;
using Android.Net;
using System;

namespace SimpleLocationAlarm.Droid.Services
{
	[BroadcastReceiver]
	public class ScreenOffBroadcastReceiver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			Log.Debug ("ScreenOffBroadcastReceiver", "OnReceive" + intent.Action);
			var serviceIntent = new Intent (context, typeof(UIWhileRingingIntentService)).SetAction (intent.Action);
			context.StartService (serviceIntent);
		}
	}

	[Service]
	public class UIWhileRingingIntentService : Service
	{
		const string TAG = "UIWhileRingingIntentService";

		public const string StartAlarmAction = "StartAlarmAction";

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug (TAG, "OnStartCommand" + intent.Action);

			switch (intent.Action) {
			case StartAlarmAction:
				StartAlarm (intent);
				break;
			case Intent.ActionScreenOff:	// vibration stops when devices is locked
				if (_vibrator != null) {	// continue vibrating if already was
					_vibrator = null;
					StartVibrating ();
				}
				break;
			}

			return StartCommandResult.RedeliverIntent;
		}

		public override IBinder OnBind (Intent intent)
		{
			return null;
		}

		void StartAlarm (Intent intent)
		{
			var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences (this);
			var shouldVibrate = sharedPreferences.GetBoolean (SettingsScreen.VibrateSettingKey, SettingsScreen.VibrateSettingDefaultValue);
			var shouldPlaySound = sharedPreferences.GetBoolean (SettingsScreen.PlaySoundSettingKey, SettingsScreen.PlaySoundSettingDefaultValue);
			var customSound = sharedPreferences.GetString (SettingsScreen.SoundSettingKey, "");

			if (shouldPlaySound) {
				PlaySound (customSound);
			}
			if (shouldVibrate) {
				StartVibrating ();
			}

			ShowNotification (JsonConvert.DeserializeObject<AlarmData> (intent.GetStringExtra (Constants.AlarmsData_Extra)));

			// this action cannot be declared in manifest
			RegisterReceiver (_screenOffBroadcastReceiver, new IntentFilter (Intent.ActionScreenOff));
		}

		ScreenOffBroadcastReceiver _screenOffBroadcastReceiver = new ScreenOffBroadcastReceiver ();

		void StopAlarm ()
		{
			UnregisterReceiver (_screenOffBroadcastReceiver);

			StopPlaying ();
			StopVibrating ();
			HideNotification ();
		}

		MediaPlayer _ringtone;

		void PlaySound (string customSound)
		{
			if (_ringtone != null) {
				return;
			}           

//			_ringtone = RingtoneManager.GetRingtone (Application.Context, sound);
//			_ringtone.Play ();

			try {
				var sound = GetRingtoneUri(customSound);

				_ringtone = new MediaPlayer();
				_ringtone.SetDataSource(this, sound);

				_ringtone.SetAudioStreamType(Stream.Alarm);

				_ringtone.Looping = true;
				_ringtone.Prepare();
				_ringtone.Start();
			} catch(Exception e) {
			}   
		}

        Android.Net.Uri GetRingtoneUri(string customSound)
        {
            if (!string.IsNullOrEmpty(customSound))
            {
                return Android.Net.Uri.Parse(customSound);
            }
            else
            {
                return (RingtoneManager.GetDefaultUri(RingtoneType.Alarm) != null ?
                    RingtoneManager.GetDefaultUri(RingtoneType.Alarm) : RingtoneManager.GetDefaultUri(RingtoneType.Ringtone));
            }
        }

		void StopPlaying ()
		{
			if (_ringtone != null) {
				_ringtone.Stop ();
				_ringtone = null;
			}
		}

		Vibrator _vibrator;

		void StartVibrating ()
		{
			if (_vibrator != null) {
				return;
			}

			_vibrator = (Vibrator) GetSystemService (Context.VibratorService);
			_vibrator.Vibrate (new long[] { 0, 1000, 1000 }, 0);
		}

		void StopVibrating ()
		{
			if (_vibrator != null) {
				_vibrator.Cancel ();
				_vibrator = null;
			}
		}

		int _notificationId = 67;

		void ShowNotification (AlarmData alarm)
		{
			var builder = new NotificationCompat.Builder (this);
			builder.SetSmallIcon (Resource.Drawable.marker_violet)
                .SetContentTitle (alarm.Name)
                .SetContentText (GetString (Resource.String.app_name))
                .SetAutoCancel (false);

			var intent = new Intent (this, typeof(AlarmScreen))
                .SetFlags (ActivityFlags.NewTask)
                .PutExtra (Constants.AlarmsData_Extra, JsonConvert.SerializeObject (alarm));
			builder.SetContentIntent (PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.UpdateCurrent));

			StartForeground (_notificationId, builder.Build ());
		}

		void HideNotification ()
		{
			StopForeground (true);
		}

		public override void OnDestroy ()
		{
			Log.Debug (TAG, "OnDestroy");
			StopAlarm ();
			base.OnDestroy ();
		}
	}
}