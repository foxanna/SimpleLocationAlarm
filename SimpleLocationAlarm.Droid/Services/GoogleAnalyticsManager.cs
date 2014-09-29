using Android.App;
using Android.Gms.Analytics;

namespace SimpleLocationAlarm.Droid.Services
{
	public enum GACategory
	{
		MainScreen,
		SettingsScreen,
		AlarmsScreen,
		AboutScreen
	}

	public enum GAAction
	{
		Click
	}

	public class GoogleAnalyticsManager
	{
		readonly Tracker _tracker;

		public GoogleAnalyticsManager ()
		{
			var analytics = GoogleAnalytics.GetInstance (Application.Context);
			#if (DEBUG)
			analytics.SetDryRun(true);
			#endif
			_tracker = analytics.NewTracker ("UA-54114842-3");
			_tracker.EnableExceptionReporting (true);
		}

		public void ReportScreenEnter (string name)
		{
			_tracker.SetScreenName (name);
			_tracker.Send (new HitBuilders.AppViewBuilder ().Build ());
		}

		public void ReportEvent (GACategory category, GAAction action, string label)
		{            
			_tracker.Send (new HitBuilders.EventBuilder ().SetCategory (category.ToString ()).SetAction (action.ToString ()).SetLabel (label).Build ());
		}
	}
}