using System;
using System.Linq;
using Android.Gms.Common;
using Android.Widget;
using Android.App;
using Android.Gms.Location;
using System.Collections.Generic;
using Android.Content;
using Newtonsoft.Json;
using Android.Util;

namespace SimpleLocationAlarm.Droid.MainScreen
{
    public partial class HomeActivity :
        IGooglePlayServicesClientConnectionCallbacks, IGooglePlayServicesClientOnConnectionFailedListener,
        LocationClient.IOnRemoveGeofencesResultListener, LocationClient.IOnAddGeofencesResultListener
    {
        LocationClient _locationClient;

        List<Tuple<Mode, AlarmData>> _changesToProceed = new List<Tuple<Mode, AlarmData>>();

        void AddGeofence(AlarmData alarm)
        {
            Log.Debug(TAG, "AddGeofence");

            lock (_changesToProceed)
            {
                _changesToProceed.Add(Tuple.Create(Mode.Add, alarm));
            }

            ProcessNextChange();
        }

        void RemoveGeofence(AlarmData alarm)
        {
            Log.Debug(TAG, "RemoveGeofence");

            lock (_changesToProceed)
            {
                // if alarm in queue to be added should be removed
                var alarmFromQueueToAdd = _changesToProceed.FirstOrDefault(a => a.Item1 == Mode.Add && a.Item2.Latitude == alarm.Latitude && a.Item2.Longitude == alarm.Longitude);
                if (alarmFromQueueToAdd != null && (!_isProcessing || _changesToProceed.IndexOf(alarmFromQueueToAdd) > 0))
                {
                    _changesToProceed.Remove(alarmFromQueueToAdd);
                    return;
                }

                _changesToProceed.Add(Tuple.Create(Mode.MarkerSelected, alarm));
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

            _locationClient.Connect();
        }

        public void OnConnected(Android.OS.Bundle connectionHint)
        {
            Log.Debug(TAG, "OnConnected");

            Tuple<Mode, AlarmData> change = null;
            
            lock (_changesToProceed)
            {
                if (_changesToProceed.Count == 0)
                {
                    _locationClient.Disconnect();
                    return;
                }
                else
                {
                    change = _changesToProceed[0];
                }
            }

            var transitionIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(AlarmScreen)), PendingIntentFlags.UpdateCurrent);

            switch (change.Item1)
            {
                case Mode.Add:
                    _dbManager.AddAlarm(_changesToProceed[0].Item2);
                    _locationClient.AddGeofences(new List<IGeofence>() { AlarmToGeofence(change.Item2) }, transitionIntent, this);
                    break;
                case Mode.MarkerSelected:
                    _locationClient.RemoveGeofences(new List<string>() { _changesToProceed[0].Item2.RequestId }, this);
                    break;
            }           

        }

        IGeofence AlarmToGeofence(AlarmData alarm)
        {
             return new GeofenceBuilder()
                .SetRequestId(alarm.RequestId)
                .SetTransitionTypes(Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit)
                .SetCircularRegion(alarm.Latitude, alarm.Longitude, (float)alarm.Radius)
                .SetExpirationDuration(Geofence.NeverExpire)
                .Build();
        }

        public void OnDisconnected()
        {
            Log.Debug(TAG, "OnDisconnected");

            _isProcessing = false;

            ProcessNextChange();
        }


        public void OnAddGeofencesResult(int statusCode, string[] geofenceRequestIds)
        {
            if (LocationStatusCodes.Success == statusCode)
            {
                Log.Debug(TAG, "OnAddGeofencesResult Success");
            }
            else
            {
                Log.Debug(TAG, "OnAddGeofencesResult Failure");

                _dbManager.DisableAlarm(_changesToProceed[0].Item2.RequestId);

                Toast.MakeText(this, Resource.String.failed_to_add, ToastLength.Short).Show();
                Toast.MakeText(this, Resource.String.probably_location_services_are_off, ToastLength.Short).Show();
            }

            lock (_changesToProceed)
            {
                _changesToProceed.RemoveAt(0);
            }

            _isProcessing = false;

            ProcessNextChange();
        }

        public void OnRemoveGeofencesByPendingIntentResult(int statusCode, PendingIntent pendingIntent)
        {
            throw new NotImplementedException();
        }

        public void OnRemoveGeofencesByRequestIdsResult(int statusCode, string[] geofenceRequestIds)
        {
            if (LocationStatusCodes.Success == statusCode)
            {
                Log.Debug(TAG, "OnRemoveGeofencesByRequestIdsResult Success");

                _dbManager.DeleteAlarm(_changesToProceed[0].Item2);
            }
            else
            {
                Log.Debug(TAG, "OnRemoveGeofencesByRequestIdsResult Failure");

                Toast.MakeText(this, Resource.String.failed_to_remove, ToastLength.Short).Show();
                Toast.MakeText(this, Resource.String.probably_location_services_are_off, ToastLength.Short).Show();
            }

            lock (_changesToProceed)
            {
                _changesToProceed.RemoveAt(0);
            }
            
            _isProcessing = false;

            ProcessNextChange();
        }

        const int _locationManagerFailedRequestCode = 42;

        public void OnConnectionFailed(ConnectionResult result)
        {
            _isProcessing = false;

            if (result.HasResolution)
            {
                Log.Debug(TAG, "OnConnectionFailed with resolution");

                try
                {
                    result.StartResolutionForResult(this, _locationManagerFailedRequestCode);
                }
                catch (Android.Content.IntentSender.SendIntentException e)
                {
                    Log.Debug(TAG, e.Message);

                    Toast.MakeText(this, Resource.String.failed_to_connect, ToastLength.Short).Show();
                }
            }
            else
            {
                Log.Debug(TAG, "OnConnectionFailed without resolution");

                Toast.MakeText(this, Resource.String.failed_to_connect, ToastLength.Short).Show();
            }
        }

        void OnActivityResultForLM(Result resultCode)
        {
            if (resultCode == Result.Ok)
            {
                ProcessNextChange();
            }
            else
            {
                Log.Debug(TAG, "OnActivityResultForLM canceled");

                Toast.MakeText(this, Resource.String.failed_to_connect, ToastLength.Short).Show();
            }
        }
    }
}