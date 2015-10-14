using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.OS;
using Android.Util;
using Object = Java.Lang.Object;

namespace SimpleLocationAlarm.Droid.Services
{
    public class StringEventArgs : EventArgs
    {
        public StringEventArgs(string data)
        {
            Data = data;
        }

        public string Data { get; private set; }
    }

    public class GeofenceChangeEventArgs : EventArgs
    {
        public GeofenceChangeEventArgs(int status, string[] requestId)
        {
            Status = status;
            RequestId = requestId;
        }

        public int Status { get; private set; }
        public string[] RequestId { get; private set; }
    }

    public class ConnectionResultEventArgs : EventArgs
    {
        public ConnectionResultEventArgs(ConnectionResult status)
        {
            ConnectionResult = status;
        }

        public ConnectionResult ConnectionResult { get; private set; }
    }

    public class GeofenceManager : Object,
        IGooglePlayServicesClientConnectionCallbacks, IGooglePlayServicesClientOnConnectionFailedListener,
        LocationClient.IOnRemoveGeofencesResultListener, LocationClient.IOnAddGeofencesResultListener
    {
        private const string TAG = "GeofenceManager";
        public const int ConnectionFailedRequestCode = 42;
        private readonly LocationClient _locationClient;

        public GeofenceManager()
        {
            _locationClient = new LocationClient(Application.Context, this, this);
        }

        public event EventHandler<StringEventArgs> Error;

        private void OnError(int errorResId)
        {
            var errorText = Application.Context.GetString(errorResId);
            Error?.Invoke(this, new StringEventArgs(errorText));
        }

        #region Add geofence

        public void AddGeofence(string requestId, double latitude, double longitude, float radius)
        {
            var geofence = PrepareGeofence(requestId, latitude, longitude, radius);
            AddGeofences(new List<IGeofence> {geofence});
        }

        public void AddGeofences(List<IGeofence> geofences)
        {
            var transitionIntent =
                PendingIntent.GetService(Application.Context, 0,
                    new Intent(Application.Context, typeof (ReceiveTransitionsIntentService)),
                    PendingIntentFlags.UpdateCurrent);

            _locationClient.AddGeofences(geofences, transitionIntent, this);
        }

        public IGeofence PrepareGeofence(string requestId, double latitude, double longitude, float radius)
        {
            return new GeofenceBuilder()
                .SetRequestId(requestId)
                .SetTransitionTypes(Geofence.GeofenceTransitionEnter)
                .SetCircularRegion(latitude, longitude, radius)
                .SetExpirationDuration(Geofence.NeverExpire)
                .Build();
        }

        public void OnAddGeofencesResult(int statusCode, string[] geofenceRequestIds)
        {
            Log.Debug(TAG, "OnAddGeofencesResult " + (statusCode == LocationStatusCodes.Success ? "success" : "failure"));

            OnGeofenceAdded(statusCode, geofenceRequestIds);

            if (statusCode != LocationStatusCodes.Success)
            {
                OnError(Resource.String.failed_to_add);
                OnError(Resource.String.probably_location_services_are_off);
            }
        }

        public event EventHandler<GeofenceChangeEventArgs> GeofenceAdded;

        private void OnGeofenceAdded(int status, string[] requestId)
        {
            GeofenceAdded?.Invoke(this, new GeofenceChangeEventArgs(status, requestId));
        }

        #endregion // Add geofence

        #region Remove geofence

        public void RemoveGeofence(string requestId)
        {
            _locationClient.RemoveGeofences(new List<string> {requestId}, this);
        }

        public event EventHandler<GeofenceChangeEventArgs> GeofenceRemoved;

        private void OnGeofenceRemoved(int status, string requestId)
        {
            GeofenceRemoved?.Invoke(this, new GeofenceChangeEventArgs(status, new[] {requestId}));
        }

        public void OnRemoveGeofencesByPendingIntentResult(int statusCode, PendingIntent pendingIntent)
        {
            throw new NotImplementedException();
        }

        public void OnRemoveGeofencesByRequestIdsResult(int statusCode, string[] geofenceRequestIds)
        {
            Log.Debug(TAG,
                "OnRemoveGeofencesByRequestIdsResult " +
                (statusCode == LocationStatusCodes.Success ? "success" : "failure"));

            OnGeofenceRemoved(statusCode, geofenceRequestIds[0]);

            if (statusCode != LocationStatusCodes.Success)
            {
                OnError(Resource.String.failed_to_remove);
                OnError(Resource.String.probably_location_services_are_off);
            }
        }

        #endregion // Remove geofence

        #region Location manager callbacks

        public event EventHandler Started;

        public void Start()
        {
            _locationClient.Connect();
        }

        public void OnConnected(Bundle connectionHint)
        {
            Log.Debug(TAG, "OnConnected");

            Started?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<ConnectionResultEventArgs> FailedToStart;

        public event EventHandler<ConnectionResultEventArgs> FailedToStartWithResolution;

        public void OnConnectionFailed(ConnectionResult result)
        {
            Log.Debug(TAG, "OnConnectionFailed " + (result.HasResolution ? "with" : "without") + " resolution");

            var handler = result.HasResolution ? FailedToStartWithResolution : FailedToStart;
            handler?.Invoke(this, new ConnectionResultEventArgs(result));

            if (!result.HasResolution)
            {
                OnError(Resource.String.failed_to_connect);
            }
        }

        public void Stop()
        {
            _locationClient.Disconnect();
        }

        public event EventHandler Stoped;

        public void OnDisconnected()
        {
            Log.Debug(TAG, "OnDisconnected");

            Stoped?.Invoke(this, EventArgs.Empty);
        }

        #endregion // Location manager callbacks
    }
}