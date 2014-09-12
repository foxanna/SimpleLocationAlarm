using Android.App;
using Android.OS;
using Android.Views;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using SimpleLocationAlarm.Droid.Services;
using Newtonsoft.Json;

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

        string _requestId;
        
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.AlarmGeofenceTriggered);

            OnNewIntent(Intent);
		}

        protected override void OnNewIntent(Android.Content.Intent intent)
        {
            base.OnNewIntent(intent);

            _selectedAlarm = JsonConvert.DeserializeObject<AlarmData>(intent.GetStringExtra(Constants.AlarmsData_Extra));
            _requestId = _selectedAlarm.RequestId;

            SupportActionBar.Title = _selectedAlarm.Name;            
        }

		GoogleMap _map;

		protected override void OnStart ()
		{
			base.OnStart ();
            		
			_map = (SupportFragmentManager.FindFragmentById (Resource.Id.map) as SupportMapFragment).Map;

			if (_map != null) {
				_map.MyLocationEnabled = true;

                RedrawAlarm();
	        }			
		}

		protected override void OnStop ()
		{
			if (_map != null) {
                _selectedMarker = null;
				_map.Clear ();
			}

			_map = null;

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
                StopRinging();
				return true;
			case Resource.Id.enable_alarm:
                EnableAlarm(_selectedAlarm, true);
				return true;
			case Resource.Id.disable_alarm:
                EnableAlarm(_selectedAlarm, false);
                StopRinging();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}
		}

        void DeleteSelectedMarker()
        {
            var alarm = _selectedAlarm;
            alarm.Enabled = true;
            _selectedAlarm = null;

            CorrectOptionsMenuVisibility();
            RemoveGeofence(alarm, ActionOnAlarm.Delete);

            _selectedMarker.Remove();
            _selectedMarker = null;
            
            ShowUndoBar(() => AddGeofence(alarm), Finish);
        }
        
        protected override void OnDataUpdated(object sender, AlarmsEventArgs e)
        {
            _selectedAlarm = _dbManager.GetAlarmByGeofenceRequestId(_requestId);       
  
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
                    .InvokeRadius(_selectedAlarm.Radius));

                circle.FillColor = Resources.GetColor(_selectedAlarm.Enabled ? Resource.Color.light : Resource.Color.light_grey);
                circle.StrokeColor = Resources.GetColor(_selectedAlarm.Enabled ? Resource.Color.dark : Resource.Color.dark_grey);
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
    }
}