using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Content;
using Newtonsoft.Json;

namespace SimpleLocationAlarm.Droid.MainScreen
{
	public enum Mode
	{
		Edit,
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
				var oldMode = _mode;
				_mode = value;

				if (value == Mode.Add) {
					PrepareToAdd ();
				} else if (_mode == Mode.None) {
					CancelAnything ();
				}
			}
		}

		IMenuItem _addAlarmMenuButton, _cancelMenuButton, _acceptMenuButton, _alarmNameMenuItem;
		EditText _alarmNameEditText;

		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main_screen, menu);

			_addAlarmMenuButton = menu.FindItem (Resource.Id.add_alarm);
			_cancelMenuButton = menu.FindItem (Resource.Id.accept);
			_acceptMenuButton = menu.FindItem (Resource.Id.cancel);
			_alarmNameMenuItem = menu.FindItem (Resource.Id.alarm_name);

			_alarmNameEditText = MenuItemCompat.GetActionView (_alarmNameMenuItem) as EditText;
			_alarmNameEditText.Hint = Resources.GetString (Resource.String.alarm_name);

			ManageMenuItemsVisibilityForMode ();

			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
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
			default:
				return base.OnOptionsItemSelected (item);
			}
		}

		void ManageMenuItemsVisibilityForMode ()
		{
			switch (Mode) {
			case Mode.None:
				_addAlarmMenuButton.SetVisible (true);

				_cancelMenuButton.SetVisible (false);
				_acceptMenuButton.SetVisible (false);
				_alarmNameMenuItem.CollapseActionView ();
				_alarmNameMenuItem.SetOnActionExpandListener (null);
				_alarmNameMenuItem.SetVisible (false);

				_alarmNameEditText.Text = string.Empty;

				break;
			case Mode.Add:
				_addAlarmMenuButton.SetVisible (false);

				_cancelMenuButton.SetVisible (true);
				_acceptMenuButton.SetVisible (true);
				_alarmNameMenuItem.SetVisible (true);

				_alarmNameMenuItem.ExpandActionView ();
				_alarmNameMenuItem.SetOnActionExpandListener (this);

				break;
			}
		}

		void OnMapClick (object sender, GoogleMap.MapClickEventArgs e)
		{
			if (_alarmToAdd == null) {
				_alarmToAdd = _map.AddMarker (new MarkerOptions ().SetPosition (e.Point));
				_alarmToAdd.Draggable = true;
			} else {
				_alarmToAdd.Position = e.Point;
			}
		}

		Marker _alarmToAdd;

		void CancelAnything ()
		{
			ManageMenuItemsVisibilityForMode ();

			_map.MapClick -= OnMapClick;

			if (_alarmToAdd != null) {
				_alarmToAdd.Remove ();
				_alarmToAdd = null;
			}

			_map.Clear ();

			RedrawMapData ();
			ZoomToMyLocationAndAlarms ();
		}

		void PrepareToAdd ()
		{
			ManageMenuItemsVisibilityForMode ();

			_map.Clear ();
			_map.MapClick += OnMapClick;
		}

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
				StartService (new Intent (Constants.DatabaseService_AddAlarm_Action)
					.PutExtra (Constants.AlarmsData_Extra, JsonConvert.SerializeObject (
					new AlarmData () {
						Latitude = _alarmToAdd.Position.Latitude,
						Longitude = _alarmToAdd.Position.Longitude,
						Radius = 200,
						Name = _alarmNameEditText.Text,
					})));
				return true;
			}
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