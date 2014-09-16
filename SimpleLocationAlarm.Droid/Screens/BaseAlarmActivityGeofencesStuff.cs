using System;
using System.Linq;
using Android.Widget;
using Android.App;
using Android.Gms.Location;
using System.Collections.Generic;
using Android.Util;
using SimpleLocationAlarm.Droid.Services;

namespace SimpleLocationAlarm.Droid.Screens
{
    public abstract partial class BaseAlarmActivity         
    {
        public enum ActionOnAlarm
        {
            Add, Disable, Delete
        }

        List<Tuple<ActionOnAlarm, AlarmData>> _changesToProceed = new List<Tuple<ActionOnAlarm, AlarmData>>();

        protected void AddGeofence(AlarmData alarm)
        {
            Log.Debug(TAG, "AddGeofence");

            lock (_changesToProceed)
            {
                _changesToProceed.Add(Tuple.Create(ActionOnAlarm.Add, alarm));
            }

            ProcessNextChange();
        }

        protected void RemoveGeofence(AlarmData alarm, ActionOnAlarm action)
        {
            Log.Debug(TAG, "RemoveGeofence");

            lock (_changesToProceed)
            {
                // if alarm in queue to be added should be removed
                var alarmFromQueueToAdd = _changesToProceed.FirstOrDefault(a => a.Item1 == ActionOnAlarm.Add && a.Item2.Latitude == alarm.Latitude && a.Item2.Longitude == alarm.Longitude);
                if (alarmFromQueueToAdd != null && (!_isProcessing || _changesToProceed.IndexOf(alarmFromQueueToAdd) > 0))
                {
                    _changesToProceed.Remove(alarmFromQueueToAdd);
                    return;
                }

                _changesToProceed.Add(Tuple.Create(action, alarm));
            }

            ProcessNextChange();
        }

        bool _isProcessing;

        void ProcessNextChange()
        {
            lock (_changesToProceed)
            {
                if (_isProcessing || _changesToProceed.Count == 0)
                {
                    return;
                }
            }

            _isProcessing = true;

            _geofenceManager.Start();
        }

        void OnGeofenceManagerError(object sender, StringEventArgs e)
        {
            ShowToast(e.Data);
        }

        protected void ShowToast(int resId)
        {
            ShowToast(Resources.GetString(resId));
        }

        protected void ShowToast(string data)
        {
            var toast = Toast.MakeText(this, data, ToastLength.Short);
            toast.View.SetBackgroundResource(Resource.Drawable.undo_bar_bg);
            toast.Show();
        }
        
        void OnGeofenceManagerStoped(object sender, EventArgs e)
        {
            _isProcessing = false;

            ProcessNextChange();
        }

        void OnGeofenceManagerStarted(object sender, EventArgs e)
        {
            Tuple<ActionOnAlarm, AlarmData> change = null;

            lock (_changesToProceed)
            {
                if (_changesToProceed.Count == 0)
                {
                    _geofenceManager.Stop();
                    return;
                }
                else
                {
                    change = _changesToProceed[0];
                }
            }

            switch (change.Item1)
            {
                case ActionOnAlarm.Add:
                    var alarm = _changesToProceed[0].Item2;
                    _dbManager.AddAlarm(alarm);
                    _geofenceManager.AddGeofence(alarm.RequestId, alarm.Latitude, alarm.Longitude, alarm.Radius);
                    break;
                case ActionOnAlarm.Delete:
                case ActionOnAlarm.Disable:
                    _geofenceManager.RemoveGeofence(_changesToProceed[0].Item2.RequestId);
                    break;
            }           
        }

        void OnGeofenceManagerFailedToStartWithResolution(object sender, ConnectionResultEventArgs e)
        {
            _isProcessing = false;

            try
            {
                e.ConnectionResult.StartResolutionForResult(this, GeofenceManager.ConnectionFailedRequestCode);
            }
            catch (Android.Content.IntentSender.SendIntentException ex)
            {
                Log.Debug(TAG, ex.Message);

                ShowToast(Resource.String.failed_to_connect);
            }
        }

        void OnGeofenceManagerFailedToStart(object sender, ConnectionResultEventArgs e)
        {
            _isProcessing = false;
        }
        
        void OnGeofenceManagerGeofenceAdded(object sender, GeofenceChangeEventArgs e)
        {
            if (LocationStatusCodes.Success != e.Status)
            {
                _dbManager.DisableAlarm(_changesToProceed[0].Item2.RequestId);
            }

            lock (_changesToProceed)
            {
                _changesToProceed.RemoveAt(0);
            }

            _isProcessing = false;

            ProcessNextChange();
        }

        void OnGeofenceManagerGeofenceRemoved(object sender, GeofenceChangeEventArgs e)
        {
            if (LocationStatusCodes.Success == e.Status)
            {
                if (_changesToProceed[0].Item1 == ActionOnAlarm.Disable)
                {
                    _dbManager.DisableAlarm(_changesToProceed[0].Item2.RequestId);
                }
                else
                {
                    _dbManager.DeleteAlarm(_changesToProceed[0].Item2);
                }
            }

            lock (_changesToProceed)
            {
                _changesToProceed.RemoveAt(0);
            }

            _isProcessing = false;

            ProcessNextChange();
        }                   

        protected void OnActivityResultForLM(Result resultCode)
        {
            if (resultCode == Result.Ok)
            {
                ProcessNextChange();
            }
            else
            {
                Log.Debug(TAG, "OnActivityResultForLM canceled");

                ShowToast(Resource.String.failed_to_connect);
            }
        }
    }
}