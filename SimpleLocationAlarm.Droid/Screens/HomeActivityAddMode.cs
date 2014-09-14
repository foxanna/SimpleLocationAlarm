using System;
using System.Linq;
using Android.Views;
using Android.App;
using Android.Widget;
using Android.Support.V4.View;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Content;
using Newtonsoft.Json;
using Android.Views.InputMethods;
using Android.Preferences;

namespace SimpleLocationAlarm.Droid.Screens
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

		Mode Mode {
			get {
				return _mode;
			}
			set {
				_mode = value;
                
				ManageMenuItemsVisibilityForMode ();

				switch (_mode) {
				case Mode.Add:
					PrepareToAdd ();
					break;
				case Mode.None:
					CancelAnything ();
					break;
				}
			}
		}

		IMenuItem _addAlarmMenuButton, _cancelMenuButton, _acceptMenuButton, _alarmNameMenuItem, _deleteAlarmMenuItem, _disableAlarmMenuItem, _enableAlarmMenuItem, _settingsMenuItem;
		EditText _alarmNameEditText;

		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main_screen, menu);

			_addAlarmMenuButton = menu.FindItem (Resource.Id.add_alarm);
			_cancelMenuButton = menu.FindItem (Resource.Id.accept);
			_acceptMenuButton = menu.FindItem (Resource.Id.cancel);
			_alarmNameMenuItem = menu.FindItem (Resource.Id.alarm_name);
			_deleteAlarmMenuItem = menu.FindItem (Resource.Id.delete);
			_disableAlarmMenuItem = menu.FindItem (Resource.Id.disable_alarm);
			_enableAlarmMenuItem = menu.FindItem (Resource.Id.enable_alarm);

			_alarmNameEditText = MenuItemCompat.GetActionView (_alarmNameMenuItem) as EditText;
			_alarmNameEditText.Hint = Resources.GetString (Resource.String.alarm_name);
			_settingsMenuItem = menu.FindItem (Resource.Id.action_settings);

			ManageMenuItemsVisibilityForMode ();

			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Android.Resource.Id.Home:
				OnBackPressed ();
				return true;
			case Resource.Id.add_alarm:
				Mode = Mode.Add;
				return true;
			case Resource.Id.cancel:
				Mode = Mode.None;
				return true;
			case Resource.Id.accept:
				if (Mode == Mode.Add) {
					if (AcceptAdd ()) {
						Mode = Mode.None;
					}
				}
				return true;
			case Resource.Id.delete:
				DeleteSelectedMarker ();
				StopRinging ();
				Mode = Mode.None;
				return true;
			case Resource.Id.enable_alarm:				
				EnableAlarm (_selectedAlarm, true);
				Mode = Mode.MarkerSelected;
				return true;
			case Resource.Id.disable_alarm:				
				EnableAlarm (_selectedAlarm, false);
				StopRinging ();
				Mode = Mode.MarkerSelected;
				return true;
			case Resource.Id.action_settings:
				OpenSettings ();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}
		}

		void ManageMenuItemsVisibilityForMode ()
		{
			switch (Mode) {
			case Mode.None:
				HideAllActionbarButtons ();

				_addAlarmMenuButton.SetVisible (true);
				_settingsMenuItem.SetVisible (true);
                    
				SupportActionBar.SetDisplayHomeAsUpEnabled (false);
                    
				break;
			case Mode.Add:
				HideAllActionbarButtons ();

				_cancelMenuButton.SetVisible (true);
				_acceptMenuButton.SetVisible (true);
				_alarmNameMenuItem.SetVisible (true);

				_alarmNameMenuItem.ExpandActionView ();
				_alarmNameMenuItem.SetOnActionExpandListener (this);
                    
				break;
			case Mode.MarkerSelected:
				HideAllActionbarButtons ();

				_deleteAlarmMenuItem.SetVisible (true);

				_enableAlarmMenuItem.SetVisible (!_selectedAlarm.Enabled);
				_disableAlarmMenuItem.SetVisible (_selectedAlarm.Enabled);
                    
				SupportActionBar.SetDisplayHomeAsUpEnabled (true);

				break;
			}
		}

		void HideAllActionbarButtons ()
		{
			_addAlarmMenuButton.SetVisible (false);

			_cancelMenuButton.SetVisible (false);
			_acceptMenuButton.SetVisible (false);
			_alarmNameMenuItem.CollapseActionView ();
			_alarmNameMenuItem.SetOnActionExpandListener (null);
			_alarmNameMenuItem.SetVisible (false);
			_deleteAlarmMenuItem.SetVisible (false);
			_enableAlarmMenuItem.SetVisible (false);
			_disableAlarmMenuItem.SetVisible (false);
			_settingsMenuItem.SetVisible (false);

			_alarmNameEditText.Text = string.Empty;

			(this.GetSystemService (Context.InputMethodService) as InputMethodManager).HideSoftInputFromWindow (_alarmNameEditText.WindowToken, 0);
		}

		void CancelAnything ()
		{
			ClearMap ();
         
			_alarmToAdd = null;

			_selectedMarker = null;

			RedrawMapData ();
			ZoomToMyLocationAndAlarms ();
		}

		void PrepareToAdd ()
		{
			ClearMap ();
		}

		Random random = new Random ();

		bool AcceptAdd ()
		{
			if (_alarmToAdd == null) {
				Toast.MakeText (this, Resource.String.click_on_map_to_set_alarm, ToastLength.Short).Show ();
				return false;
			} else if (string.IsNullOrEmpty (_alarmNameEditText.Text)) {
				_alarmNameEditText.RequestFocus ();
				_alarmNameEditText.SetError (
					new Java.Lang.String (Resources.GetString (Resource.String.enter_alarm_name)), null);
				return false;
			} else {
				var defaultRadius = PreferenceManager.GetDefaultSharedPreferences (Application.Context).GetInt (SettingsScreen.DefaultRadiusKey, SettingsScreen.DefaultRadiusValue);

				var newAlarm = new AlarmData () {
					Latitude = _alarmToAdd.Position.Latitude,
					Longitude = _alarmToAdd.Position.Longitude,
					Radius = defaultRadius,
					Name = _alarmNameEditText.Text,
					Enabled = true,
					RequestId = string.Format ("{0};{1}_{2}", _alarmToAdd.Position.Latitude, _alarmToAdd.Position.Longitude, random.NextDouble ())
				};
                
				AddGeofence (newAlarm);

				return true;
			}
		}

		void DeleteSelectedMarker ()
		{
			RemoveGeofence (_selectedAlarm, ActionOnAlarm.Delete);
                        
			_selectedMarker.Remove ();
			_selectedMarker = null;

			var alarm = _selectedAlarm;
			alarm.Enabled = true;

			ShowUndoBar (() => AddGeofence (alarm));
		}

		public bool OnMenuItemActionCollapse (Android.Views.IMenuItem item)
		{
			if (Mode != Mode.None) {
				OnBackPressed ();
			}

			return true;
		}

		public bool OnMenuItemActionExpand (Android.Views.IMenuItem item)
		{
			return true;
		}
	}
}