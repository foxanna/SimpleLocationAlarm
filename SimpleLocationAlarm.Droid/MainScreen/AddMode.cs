using Android.Views;
using Android.Widget;
using Android.Support.V4.View;

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

		void CancelAnything ()
		{
			ManageMenuItemsVisibilityForMode ();

			_map.Clear ();
			RedrawMapData ();
		}

		void PrepareToAdd ()
		{
			ManageMenuItemsVisibilityForMode ();

			_map.Clear ();
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