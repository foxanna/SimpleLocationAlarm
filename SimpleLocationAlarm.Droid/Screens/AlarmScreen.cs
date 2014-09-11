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
using Android.Gms.Maps;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Android.Gms.Maps.Model;
using System.Threading.Tasks;
using Android.Preferences;
using Android.Media;

namespace SimpleLocationAlarm.Droid.Screens
{
	[Activity (
		Icon = "@drawable/alarm_white",
		NoHistory = true,
		ExcludeFromRecents = true,
		LaunchMode = Android.Content.PM.LaunchMode.SingleTask,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class AlarmScreen : BaseAlarmActivity
	{
        protected override string TAG
        {
            get
            {
                return "AlarmScreen";
            }
        }

		bool _success;
        string _requestId;
		Button _goToSettingsButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			if (LocationClient.HasError (Intent)) {
				Log.Debug (TAG, "OnCreate : LocationClient HasError");
				_dbManager.DisableAll ();                   

				SetContentView (Resource.Layout.AlarmError);          
      
				_goToSettingsButton = FindViewById<Button> (Resource.Id.go_to_settings);
				_goToSettingsButton.Click += GoToSettingsButton_Click;
			} else {
				Log.Debug (TAG, "OnCreate trigered by geofences");
                
				_success = true;                

				var geofences = LocationClient.GetTriggeringGeofences (Intent);
				if (geofences != null && geofences.Count > 0) {
                    _requestId = geofences[0].RequestId;
                    FindAlarm();
				}

				SetContentView (Resource.Layout.AlarmGeofenceTriggered);

                if (_selectedAlarm != null)
                {
                    SupportActionBar.Title = _selectedAlarm.Name;
				}

                InitPreferences();
				PlaySound ();
                StartVibrating();
			}
		}

		

		void GoToSettingsButton_Click (object sender, EventArgs e)
		{
			var intent = new Intent (Android.Provider.Settings.ActionLocationSourceSettings);
			StartActivity (intent);
		}

		GoogleMap _map;

		protected override void OnStart ()
		{
			base.OnStart ();
            
			if (_success) {
				_map = (SupportFragmentManager.FindFragmentById (Resource.Id.map) as SupportMapFragment).Map;

				if (_map != null) {
					_map.MyLocationEnabled = true;

                    if (_selectedAlarm != null)
                    {
                        RedrawAlarm();
					}
				}
			}
		}

		protected override void OnStop ()
		{
			if (_success) {
				if (_map != null) {
                    _selectedMarker = null;
					_map.Clear ();
				}

				_map = null;
            }
            else
            {
                _goToSettingsButton.Click -= GoToSettingsButton_Click;
            }

            StopPlaying();
            StopVibrating();

			base.OnStop ();
		}
        
		IMenuItem _deleteAlarmMenuItem, _disableAlarmMenuItem, _enableAlarmMenuItem;

		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.alarm_screen, menu);

			_deleteAlarmMenuItem = menu.FindItem (Resource.Id.delete);
			_disableAlarmMenuItem = menu.FindItem (Resource.Id.disable_alarm);
			_enableAlarmMenuItem = menu.FindItem (Resource.Id.enable_alarm);

            CorrectOptionsMenuVisibility();

			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.delete:
				DeleteSelectedMarker ();
				return true;
			case Resource.Id.enable_alarm:
                EnableAlarm(_selectedAlarm, true);
				return true;
			case Resource.Id.disable_alarm:
                EnableAlarm(_selectedAlarm, false);
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}
		}

        void DeleteSelectedMarker()
        {
            var alarm = _selectedAlarm;
            _selectedAlarm = null;

            CorrectOptionsMenuVisibility();
            RemoveGeofence(alarm, ActionOnAlarm.Delete);

            _selectedMarker.Remove();
            _selectedMarker = null;

            ShowUndoBar(() => AddGeofence(alarm), Finish);
        }

        void FindAlarm()
        {
            _selectedAlarm = _dbManager.GetAlarmByGeofenceRequestId(_requestId);
        }

        protected override void OnDataUpdated(object sender, AlarmsEventArgs e)
        {
            FindAlarm();            
            RedrawAlarm();
            CorrectOptionsMenuVisibility();
        }

        void RedrawAlarm()
        {
            if (_selectedMarker != null)
            {
                _selectedMarker.Remove();
                _selectedMarker = null;
            }

            _map.Clear();

            if (_selectedAlarm != null)
            {
                var position = new LatLng(_selectedAlarm.Latitude, _selectedAlarm.Longitude);

                var circle = _map.AddCircle(new CircleOptions()
                    .InvokeCenter(position)
                    .InvokeRadius(_selectedAlarm.Radius)
                    .InvokeFillColor(Resources.GetColor(Resource.Color.light)));

                circle.StrokeColor = Resources.GetColor(Resource.Color.dark);
                circle.StrokeWidth = 1.0f;

                _selectedMarker = _map.AddMarker(new MarkerOptions()
                                .SetPosition(position)
                                .InvokeIcon(BitmapDescriptorFactory.FromResource(_selectedAlarm.Enabled ? Resource.Drawable.alarm_violet : Resource.Drawable.alarm_grey)));
                _map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(position, _map.MaxZoomLevel - 6));
            }
        }

        void CorrectOptionsMenuVisibility()
        {
            _deleteAlarmMenuItem.SetVisible(_selectedAlarm != null);
            _disableAlarmMenuItem.SetVisible(_selectedAlarm != null && _selectedAlarm.Enabled);
            _enableAlarmMenuItem.SetVisible(_selectedAlarm != null && !_selectedAlarm.Enabled);
        }

        Vibrator _vibrator;

        void StartVibrating()
        {
            if (!_shouldVibrate)
            {
                return;
            }

            _vibrator = (Vibrator)GetSystemService(Context.VibratorService);
            long[] pattern = { 0, 100, 1000 };
            _vibrator.Vibrate(pattern, 0);
        }

        void StopVibrating()
        {
            if (_vibrator != null)
            {
                _vibrator.Cancel();
            }
        }

        //Android.Media.MediaPlayer _mediaPlayer;
        Ringtone _ringtone;

        void PlaySound()
        {
            if (!_shouldPlaySound)
            {
                return;
            }

            var sound = string.IsNullOrEmpty(_customSound) ? (RingtoneManager.GetDefaultUri(RingtoneType.Alarm) != null ?
                RingtoneManager.GetDefaultUri(RingtoneType.Alarm) : RingtoneManager.GetDefaultUri(RingtoneType.Ringtone)) : Android.Net.Uri.Parse(_customSound);

           _ringtone = RingtoneManager.GetRingtone(this, sound);
           _ringtone.Play();
            //_mediaPlayer = Android.Media.MediaPlayer.Create(this, sound);
            //_mediaPlayer.Start();
        }

        void StopPlaying()
        {
            //if (_mediaPlayer != null)
            //{
            //    _mediaPlayer.Stop();
            //    _mediaPlayer.Release();
            //}
            if (_ringtone != null)
            {
                _ringtone.Stop();
            }
        }

        bool _shouldVibrate, _shouldPlaySound;
        string _customSound;

        void InitPreferences()
        {
            var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            _shouldVibrate = sharedPreferences.GetBoolean(SettingsScreen.VibrateSettingKey, SettingsScreen.VibrateSettingDefaultValue);
            _shouldPlaySound = sharedPreferences.GetBoolean(SettingsScreen.PlaySoundSettingKey, SettingsScreen.PlaySoundSettingDefaultValue);
            _customSound = sharedPreferences.GetString(SettingsScreen.SoundSettingKey, "");
        }
    }
}