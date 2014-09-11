using System;
using Android.Support.V7.App;
using Android.Widget;
using Android.Gms.Maps.Model;

namespace SimpleLocationAlarm.Droid.Screens
{
    public abstract partial class BaseAlarmActivity : ActionBarActivity
	{
        protected DBManager _dbManager = new DBManager();
        protected GeofenceManager _geofenceManager = new GeofenceManager();

        protected abstract string TAG { get; }

		protected void InitDBManager()
        {
            _dbManager.DataUpdated += OnDataUpdated;
        }

        protected void DeinitDBManager()
        {
            _dbManager.DataUpdated -= OnDataUpdated;
        }

        protected abstract void OnDataUpdated(object sender, AlarmsEventArgs e);

        protected void InitGeofenceManager()
        {
            _geofenceManager.Error += OnGeofenceManagerError;
            _geofenceManager.Started += OnGeofenceManagerStarted;
            _geofenceManager.FailedToStart += OnGeofenceManagerFailedToStart;
            _geofenceManager.FailedToStartWithResolution += OnGeofenceManagerFailedToStartWithResolution;
            _geofenceManager.Stoped += OnGeofenceManagerStoped;
            _geofenceManager.GeofenceAdded += OnGeofenceManagerGeofenceAdded;
            _geofenceManager.GeofenceRemoved += OnGeofenceManagerGeofenceRemoved;
        }

        protected void DeinitGeofenceManager()
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

        protected void ShowUndoBar(Action undoAction, Action acceptAction = null)
        {
            if (UndoBar != null)
            {
                UndoBar.Hide();
            }

            UndoBar = new UndoBar(this, Resources.GetString(Resource.String.alarm_deleted), FindViewById(Android.Resource.Id.Content));
            UndoBar.Undo += (sender, e) => undoAction();
            if (acceptAction != null)
            {
                UndoBar.Discard += (sender, e) => acceptAction();
            }

            UndoBar.Show();
        }

        protected override void OnStart()
        {
            base.OnStart();

            InitDBManager();
            InitGeofenceManager();
        }

        protected override void OnStop()
        {
            DeinitDBManager();
            DeinitGeofenceManager();

            base.OnStop();
        }

        protected void EnableAlarm(AlarmData alarm, bool enabled)
        {
            alarm.Enabled = enabled;

            if (enabled)
            {
                AddGeofence(alarm);
            }
            else
            {
                RemoveGeofence(alarm, ActionOnAlarm.Disable);
            }
        }
	}
}

