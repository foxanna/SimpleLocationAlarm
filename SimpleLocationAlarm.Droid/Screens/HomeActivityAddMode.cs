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
using System.Security.Cryptography;
using Android.Text;
using Android.Gms.Common;
using SimpleLocationAlarm.Droid.Services;

namespace SimpleLocationAlarm.Droid.Screens
{
	public enum Mode
	{
		MarkerSelected,
		Add,
		None
	}

    public partial class HomeActivity : MenuItemCompat.IOnActionExpandListener
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

        IMenuItem _addAlarmMenuButton, _acceptMenuButton, _alarmNameMenuItem, _deleteAlarmMenuItem, _disableAlarmMenuItem, _settingsMenuItem, _alarmRadiusMenuItem;
		EditText _alarmNameEditText;
        Spinner _alarmRadiusSpinner;
		ToggleButton _enableAlarmToggleButton;

		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main_screen, menu);

			_addAlarmMenuButton = menu.FindItem (Resource.Id.add_alarm);
            _acceptMenuButton = menu.FindItem(Resource.Id.accept);
			_alarmNameMenuItem = menu.FindItem (Resource.Id.alarm_name);
			_deleteAlarmMenuItem = menu.FindItem (Resource.Id.delete);
            _disableAlarmMenuItem = menu.FindItem(Resource.Id.switch_button);
            _alarmRadiusMenuItem = menu.FindItem(Resource.Id.alarm_radius);
			_settingsMenuItem = menu.FindItem (Resource.Id.action_settings);
            
			_alarmNameEditText = MenuItemCompat.GetActionView (_alarmNameMenuItem) as EditText;
			_alarmNameEditText.Hint = Resources.GetString (Resource.String.alarm_name);
            _alarmNameEditText.SetWidth(Resources.GetDimensionPixelSize(Resource.Dimension.abc_search_view_preferred_width));

            _enableAlarmToggleButton = MenuItemCompat.GetActionView(_disableAlarmMenuItem) as ToggleButton;
            _enableAlarmToggleButton.CheckedChange += AlarmEnabledChange;

            _alarmRadiusSpinner = MenuItemCompat.GetActionView(_alarmRadiusMenuItem) as Spinner;
            var adapter = new ArrayAdapter(this, Resource.Layout.support_simple_spinner_dropdown_item, 
                Android.Resource.Id.Text1, Constants.AlarmRadiusValues.Select(r => string.Format("{0} m", r)).ToList());
            adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _alarmRadiusSpinner.Adapter = adapter;

			ManageMenuItemsVisibilityForMode ();

            _addAlarmMenuButton.SetVisible(_isGooglePlayServicesAvailable == ConnectionResult.Success);

			return base.OnCreateOptionsMenu (menu);
		}

        void AlarmEnabledChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            EnableAlarm(_selectedAlarm, e.IsChecked);
            Mode = Mode.MarkerSelected;
            if (!e.IsChecked)
            {
                StopRinging();
            }

            GoogleAnalyticsManager.ReportEvent(GACategory.MainScreen, GAAction.Click, "alarm " + (e.IsChecked ? "enabled" : "disabled"));
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
			case Resource.Id.accept:
				if (Mode == Mode.Add) {
					if (AcceptAdd ()) {
                        Mode = Mode.None;
                        GoogleAnalyticsManager.ReportEvent(GACategory.MainScreen, GAAction.Click, "alarm added");
					}
				}
				return true;
			case Resource.Id.delete:
				DeleteSelectedMarker ();
				StopRinging ();
				Mode = Mode.None;
                GoogleAnalyticsManager.ReportEvent(GACategory.MainScreen, GAAction.Click, "alarm deleted");
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

				_acceptMenuButton.SetVisible (true);
				_alarmNameMenuItem.SetVisible (true);
                _alarmRadiusMenuItem.SetVisible(true);

                var radius = PreferenceManager.GetDefaultSharedPreferences(this).GetInt(SettingsScreen.DefaultRadiusKey, SettingsScreen.DefaultRadiusValue);
                _alarmRadiusSpinner.SetSelection(Constants.AlarmRadiusValues.IndexOf(radius));

				_alarmNameMenuItem.ExpandActionView ();
			    MenuItemCompat.SetOnActionExpandListener(_alarmNameMenuItem, this);
                    
				break;
			case Mode.MarkerSelected:
				HideAllActionbarButtons ();

				_deleteAlarmMenuItem.SetVisible (true);

				_disableAlarmMenuItem.SetVisible (true);
                _enableAlarmToggleButton.Checked = _selectedAlarm.Enabled;
                    
				SupportActionBar.SetDisplayHomeAsUpEnabled (true);

				break;
			}
		}

		void HideAllActionbarButtons ()
		{
			_addAlarmMenuButton.SetVisible (false);

			_acceptMenuButton.SetVisible (false);
			_alarmNameMenuItem.CollapseActionView ();
            MenuItemCompat.SetOnActionExpandListener(_alarmNameMenuItem, null);
			_alarmNameMenuItem.SetVisible (false);
			_deleteAlarmMenuItem.SetVisible (false);
			_disableAlarmMenuItem.SetVisible (false);
			_settingsMenuItem.SetVisible (false);
            _alarmRadiusMenuItem.SetVisible(false);

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
				ShowToast(Resource.String.click_on_map_to_set_alarm);
				return false;
			} else if (string.IsNullOrEmpty (_alarmNameEditText.Text)) {
				_alarmNameEditText.RequestFocus ();
                _alarmNameEditText.SetError(Html.FromHtml(string.Format("<font color='#9933cc'>{0}</font>", Resources.GetString(Resource.String.enter_alarm_name))),
                    null);
				return false;
			} else {
				var radius = Constants.AlarmRadiusValues[_alarmRadiusSpinner.SelectedItemPosition];
                
				var newAlarm = new AlarmData () {
					Latitude = _alarmToAdd.Position.Latitude,
					Longitude = _alarmToAdd.Position.Longitude,
					Radius = radius,
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

        public bool OnMenuItemActionCollapse(Android.Views.IMenuItem item)
		{
			if (Mode != Mode.None) {
				OnBackPressed ();
			}

			return true;
		}

        public bool OnMenuItemActionExpand(Android.Views.IMenuItem item)
		{
			return true;
		}
    }
}