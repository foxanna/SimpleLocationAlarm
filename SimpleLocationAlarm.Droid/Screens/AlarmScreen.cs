using Android.App;
using Android.OS;
using Android.Views;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using SimpleLocationAlarm.Droid.Services;
using Newtonsoft.Json;
using Android.Widget;
using Android.Support.V4.View;

namespace SimpleLocationAlarm.Droid.Screens
{
    [Activity(
        Icon = "@drawable/alarm_white",
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

        protected override string AdId
        {
            get
            {
                return Resources.GetString(Resource.String.alarm_screen_ad);
            }
        }

        string _requestId;
        bool _firstStart;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AlarmGeofenceTriggered);

            _firstStart = true;

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

        protected override void OnStart()
        {
            base.OnStart();

            _map = (SupportFragmentManager.FindFragmentById(Resource.Id.map) as SupportMapFragment).Map;

            if (_map != null)
            {
                _map.MyLocationEnabled = true;

                RedrawAlarm();
            }
        }

        protected override void OnStop()
        {
            if (_map != null)
            {
                _selectedMarker = null;
                _map.Clear();
            }

            _map = null;

            base.OnStop();
        }

        IMenuItem _deleteAlarmMenuItem, _disableAlarmMenuItem;
        ToggleButton _enableAlarmToggleButton;

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.alarm_screen, menu);

            _deleteAlarmMenuItem = menu.FindItem(Resource.Id.delete);
            _disableAlarmMenuItem = menu.FindItem(Resource.Id.switch_button);

            _enableAlarmToggleButton = MenuItemCompat.GetActionView(_disableAlarmMenuItem) as ToggleButton;
            _enableAlarmToggleButton.SetBackgroundResource(Resource.Drawable.toggle_button);
            _enableAlarmToggleButton.Checked = _selectedAlarm.Enabled;
            _enableAlarmToggleButton.CheckedChange += AlarmEnabledChange;

            CorrectOptionsMenuVisibility();

            return base.OnCreateOptionsMenu(menu);
        }

        void AlarmEnabledChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            GoogleAnalyticsManager.ReportEvent(GACategory.AlarmsScreen, GAAction.Click, "alarm " + (e.IsChecked ? "enabled" : "disabled"));

            EnableAlarm(_selectedAlarm, e.IsChecked);
            if (!e.IsChecked)
            {
                StopRinging();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.delete:
                    GoogleAnalyticsManager.ReportEvent(GACategory.AlarmsScreen, GAAction.Click, "alarm deleted");
                    DeleteSelectedMarker();
                    StopRinging();
                    return true;
                case Resource.Id.stop_noise:
                    GoogleAnalyticsManager.ReportEvent(GACategory.AlarmsScreen, GAAction.Click, "sound muted");
                    StopRinging();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
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
                if (_firstStart)
                {
                    _map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(position, _map.MaxZoomLevel - 6));
                    _firstStart = false;
                }
                else
                {
                    _map.AnimateCamera(CameraUpdateFactory.NewLatLng(position));
                }
            }
        }

        void CorrectOptionsMenuVisibility()
        {
            _deleteAlarmMenuItem.SetVisible(_selectedAlarm != null);
            _disableAlarmMenuItem.SetVisible(_selectedAlarm != null);
        }
    }
}