using System;
using Android.Support.V7.App;
using Android.Gms.Maps.Model;
using SimpleLocationAlarm.Droid.Services;
using Android.Content;
using Android.Gms.Ads;
using Android.Widget;
using Android.Views;

namespace SimpleLocationAlarm.Droid.Screens
{
    public abstract partial class BaseAlarmActivity : ActionBarActivity
	{
		protected DBManager _dbManager = new DBManager ();
		protected GeofenceManager _geofenceManager = new GeofenceManager ();

		protected abstract string TAG { get; }

		protected void InitDBManager ()
		{
			_dbManager.DataUpdated += OnDataUpdated;
		}

		protected void DeinitDBManager ()
		{
			_dbManager.DataUpdated -= OnDataUpdated;
		}

		protected abstract void OnDataUpdated (object sender, AlarmsEventArgs e);

		protected void InitGeofenceManager ()
		{
			_geofenceManager.Error += OnGeofenceManagerError;
			_geofenceManager.Started += OnGeofenceManagerStarted;
			_geofenceManager.FailedToStart += OnGeofenceManagerFailedToStart;
			_geofenceManager.FailedToStartWithResolution += OnGeofenceManagerFailedToStartWithResolution;
			_geofenceManager.Stoped += OnGeofenceManagerStoped;
			_geofenceManager.GeofenceAdded += OnGeofenceManagerGeofenceAdded;
			_geofenceManager.GeofenceRemoved += OnGeofenceManagerGeofenceRemoved;
		}

		protected void DeinitGeofenceManager ()
		{
			_geofenceManager.GeofenceRemoved -= OnGeofenceManagerGeofenceRemoved;
			_geofenceManager.GeofenceAdded -= OnGeofenceManagerGeofenceAdded;
			_geofenceManager.Stoped -= OnGeofenceManagerStoped;
			_geofenceManager.FailedToStart -= OnGeofenceManagerFailedToStart;
			_geofenceManager.FailedToStartWithResolution -= OnGeofenceManagerFailedToStartWithResolution;
			_geofenceManager.Started -= OnGeofenceManagerStarted;
			_geofenceManager.Error -= OnGeofenceManagerError;
		}

		protected Marker _selectedMarker;
		protected AlarmData _selectedAlarm;

		protected UndoBar UndoBar;

		protected void ShowUndoBar (Action undoAction, Action acceptAction = null)
		{
			if (UndoBar != null) {
				UndoBar.Hide ();
			}

			UndoBar = new UndoBar (this, Resources.GetString (Resource.String.alarm_deleted), FindViewById (Android.Resource.Id.Content));
			UndoBar.Undo += (sender, e) => undoAction ();
			if (acceptAction != null) {
				UndoBar.Discard += (sender, e) => acceptAction ();
			}

			UndoBar.Show ();
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			InitDBManager ();
			InitGeofenceManager ();
		}

		protected override void OnStop ()
		{
			DeinitDBManager ();
			DeinitGeofenceManager ();

			base.OnStop ();
		}

		protected void EnableAlarm (AlarmData alarm, bool enabled)
		{
			alarm.Enabled = enabled;

			if (enabled) {
				AddGeofence (alarm);
			} else {
				RemoveGeofence (alarm, ActionOnAlarm.Disable);
			}
		}

		protected void StopRinging ()
		{
			StopService (new Intent (this, typeof(UIWhileRingingIntentService)));
		}

        protected GoogleAnalyticsManager GoogleAnalyticsManager = new GoogleAnalyticsManager();

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GoogleAnalyticsManager.ReportScreenEnter(this.GetType().FullName);
        }
        
        protected abstract string AdId { get; }

        AdView _adView;
        LinearLayout _adViewContainer;

        public override void SetContentView(int layoutResID)
        {
            base.SetContentView(layoutResID);

            _adViewContainer = FindViewById<LinearLayout>(Resource.Id.adViewContainer);
            _adView = AddAd(_adViewContainer, AdId);
        }
        
        protected override void OnPause()
        {
            if (_adView != null)
            {
                _adView.Pause();
            }

            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (_adView != null)
            {
                _adView.Resume();
            }           
        }

        protected override void OnDestroy()
        {
            if (_adView != null)
            {
                _adView.Destroy();
            }

            base.OnDestroy();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            if (_adView != null)
            {
                _adView.Destroy();
                _adView = AddAd(_adViewContainer, AdId);
            }
        }

        AdView AddAd(LinearLayout adViewContainer, string adid)
        {
            AdView adView = null;

            if (adViewContainer != null && !string.IsNullOrEmpty(adid))
            {
                adViewContainer.RemoveAllViews();

                adView = new AdView(this)
                {
                    AdSize = AdSize.SmartBanner,
                    AdUnitId = adid,
                };

                adViewContainer.AddView(adView);
                adView.LoadAd(new AdRequest.Builder().Build());
            }

            return adView;
        }
    }
}