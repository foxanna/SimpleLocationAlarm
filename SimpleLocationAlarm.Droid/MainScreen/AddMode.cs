using System;
using System.Linq;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Content;
using Newtonsoft.Json;
using Android.Views.InputMethods;

namespace SimpleLocationAlarm.Droid.MainScreen
{
    public enum Mode
    {
        MarkerSelected,
        Add,
        None
    }

    public partial class HomeActivity : IMenuItemOnActionExpandListener
    {
        Mode _mode = Mode.None;

        Mode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                
                ManageMenuItemsVisibilityForMode();

                switch (_mode)
                {
                    case Mode.Add:
                        PrepareToAdd();
                        break;
                    case Mode.None:
                        CancelAnything();
                        break;
                }
            }
        }

        IMenuItem _addAlarmMenuButton, _cancelMenuButton, _acceptMenuButton, _alarmNameMenuItem, _deleteAlarmMenuItem;
        EditText _alarmNameEditText;

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_screen, menu);

            _addAlarmMenuButton = menu.FindItem(Resource.Id.add_alarm);
            _cancelMenuButton = menu.FindItem(Resource.Id.accept);
            _acceptMenuButton = menu.FindItem(Resource.Id.cancel);
            _alarmNameMenuItem = menu.FindItem(Resource.Id.alarm_name);
            _deleteAlarmMenuItem = menu.FindItem(Resource.Id.delete);

            _alarmNameEditText = MenuItemCompat.GetActionView(_alarmNameMenuItem) as EditText;
            _alarmNameEditText.Hint = Resources.GetString(Resource.String.alarm_name);

            ManageMenuItemsVisibilityForMode();

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                case Resource.Id.add_alarm:
                    Mode = Mode.Add;
                    return true;
                case Resource.Id.cancel:
                    Mode = Mode.None;
                    return true;
                case Resource.Id.accept:
                    if (Mode == Mode.Add)
                    {
                        if (AcceptAdd())
                        {
                            Mode = Mode.None;
                        }
                    }
                    return true;
                case Resource.Id.delete:
                    DeleteSelectedMarker();
                    Mode = Mode.None;
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        void ManageMenuItemsVisibilityForMode()
        {
            switch (Mode)
            {
                case Mode.None:
                    _addAlarmMenuButton.SetVisible(true);

                    _cancelMenuButton.SetVisible(false);
                    _acceptMenuButton.SetVisible(false);
                    _alarmNameMenuItem.CollapseActionView();
                    _alarmNameMenuItem.SetOnActionExpandListener(null);

                    (this.GetSystemService(Context.InputMethodService) as InputMethodManager).HideSoftInputFromWindow(_alarmNameEditText.WindowToken, 0);
                    
                    _alarmNameMenuItem.SetVisible(false);
                    _deleteAlarmMenuItem.SetVisible(false);

                    SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                    
                    break;
                case Mode.Add:
                    _addAlarmMenuButton.SetVisible(false);

                    _cancelMenuButton.SetVisible(true);
                    _acceptMenuButton.SetVisible(true);
                    _alarmNameMenuItem.SetVisible(true);

                    _alarmNameMenuItem.ExpandActionView();
                    _alarmNameMenuItem.SetOnActionExpandListener(this);

                    _alarmNameEditText.Text = string.Empty;

                    break;
                case Mode.MarkerSelected:
                    _addAlarmMenuButton.SetVisible(false);

                    _deleteAlarmMenuItem.SetVisible(true);

                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);

                    break;
            }
        }

        void CancelAnything()
        {
            ClearMap();
         
            _alarmToAdd = null;

            _selectedMarker = null;

            RedrawMapData();
            ZoomToMyLocationAndAlarms();
        }

        void PrepareToAdd()
        {
            ClearMap();
        }

        Random random = new Random();

        bool AcceptAdd()
        {
            if (_alarmToAdd == null)
            {
                Toast.MakeText(this, Resource.String.click_on_map_to_set_alarm, ToastLength.Short).Show();
                return false;
            }
            else if (string.IsNullOrEmpty(_alarmNameEditText.Text))
            {
                _alarmNameEditText.RequestFocus();
                _alarmNameEditText.SetError(
                    new Java.Lang.String(Resources.GetString(Resource.String.enter_alarm_name)), null);
                return false;
            }
            else
            {
                var newAlarm = new AlarmData()
                    {
                        Latitude = _alarmToAdd.Position.Latitude,
                        Longitude = _alarmToAdd.Position.Longitude,
                        Radius = 200,
                        Name = _alarmNameEditText.Text,
                        Enabled = true,
                        RequestId = string.Format("{0};{1}_{2}", _alarmToAdd.Position.Latitude, _alarmToAdd.Position.Longitude, random.NextDouble())
                    };
                
                AddGeofence(newAlarm);

                return true;
            }
        }

        private void DeleteSelectedMarker()
        {
            RemoveGeofence(_mapData.FirstOrDefault(m => m.Latitude == _selectedMarker.Position.Latitude && m.Longitude == _selectedMarker.Position.Longitude));
            
            _selectedMarker.Remove();
            _selectedMarker = null;
        }

        public bool OnMenuItemActionCollapse(Android.Views.IMenuItem item)
        {
            if (Mode != Mode.None)
            {
                OnBackPressed();
            }
            return true;
        }

        public bool OnMenuItemActionExpand(Android.Views.IMenuItem item)
        {
            return true;
        }
    }
}