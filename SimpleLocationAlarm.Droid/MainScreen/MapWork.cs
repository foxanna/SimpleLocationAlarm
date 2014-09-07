using System;
using Android.Gms.Maps;
using Android.Widget;
using Android.Gms.Maps.Model;
using Android.Content;
using System.Collections.Generic;
using Android.Locations;
using Android.Gms.Location;
using System.Threading.Tasks;
using Android.Util;

namespace SimpleLocationAlarm.Droid.MainScreen
{
	public partial class HomeActivity : GoogleMap.IOnMapLoadedCallback
	{
		GoogleMap _map;

		void AnimateTo (Location location)
		{
			if (location != null) {
				_map.AnimateCamera (CameraUpdateFactory.NewLatLngZoom (
					new LatLng (location.Latitude, location.Longitude), _map.MaxZoomLevel - 6));
			}
		}

		void FindMap ()
		{
			_map = (SupportFragmentManager.FindFragmentById (Resource.Id.map) as SupportMapFragment).Map;
			if (_map != null) {
				_map.MyLocationEnabled = true;

				_map.UiSettings.TiltGesturesEnabled = false;
				_map.UiSettings.RotateGesturesEnabled = false;

				RefreshData ();
				if (Mode == Mode.None) {
					RedrawMapData ();
				}

				ZoomToMyLocationAndAlarms ();

				_map.SetOnMapLoadedCallback (this);

				_map.MyLocationChange += HandleMyLocationChange;				
				_map.MapClick += OnMapClick;
			}
		}

		public void OnMapLoaded ()
		{
			Log.Debug (TAG, "OnMapLoaded");
			_map.SetOnMapLoadedCallback (null);

			ZoomToMyLocationAndAlarms ();
		}

		Location GetLastKnownLocation ()
		{
			var locationManager = LocationManager.FromContext (this);

			return locationManager.GetLastKnownLocation (LocationManager.GpsProvider) ??
			locationManager.GetLastKnownLocation (LocationManager.NetworkProvider);
		}

		void HandleMyLocationChange (object sender, GoogleMap.MyLocationChangeEventArgs e)
		{
			Log.Debug (TAG, "New location detected");
			_map.MyLocationChange -= HandleMyLocationChange;

			ZoomToMyLocationAndAlarms ();
		}

		void LooseMap ()
		{
			if (_map != null) {
				_map.MapClick -= OnMapClick;
				_map.Clear ();

				_map = null;
			}
		}

		void OnMapClick (object sender, GoogleMap.MapClickEventArgs e)
		{
			//Toast.MakeText (this, string.Format ("{0} ; {1}", e.Point.Latitude, e.Point.Longitude), 
			//	ToastLength.Short).Show ();

			SendBroadcast (new Intent (Constants.DataBaseUpdatesBroadcastReceiverAction));
		}

		List<AlarmData> _mapData = new List<AlarmData> ();
		List<Marker> _currentMarkers = new List<Marker> ();
		List<Circle> _currentCircles = new List<Circle> ();

		void RefreshData ()
		{
			try {
				_mapData = GetAlarms ();					
			} catch {
				_mapData = new List<AlarmData> ();
			}

			Log.Debug (TAG, "data refreshed");
		}

		void RedrawMapData ()
		{
			_currentCircles.Clear ();
			_currentMarkers.Clear ();

			if (_map == null) {
				return;
			}

			_map.Clear ();

			foreach (var alarm in _mapData) {
				_currentCircles.Add (_map.AddCircle (new CircleOptions ()
					.InvokeCenter (new LatLng (alarm.Latitude, alarm.Longitude))
					.InvokeRadius (alarm.Radius)
					//	.InvokeStrokeColor (Resources.GetColor (Android.Resource.Color.HoloBlueLight))
				));
				_currentMarkers.Add (_map.AddMarker (new MarkerOptions ()
					.SetPosition (new LatLng (alarm.Latitude, alarm.Longitude))));
			}

			Log.Debug (TAG, "data redrawn");
		}

		void ZoomToMyLocationAndAlarms ()
		{
			var location = GetLastKnownLocation ();

			if (_mapData.Count > 0 || location != null) {
				var boundsBuilder = new LatLngBounds.Builder ();

				foreach (var alarm in _mapData) {
					boundsBuilder.Include (new LatLng (alarm.Latitude, alarm.Longitude));
				}

				if (location != null) {
					boundsBuilder.Include (new LatLng (location.Latitude, location.Longitude));
				}

				try {
					_map.AnimateCamera (CameraUpdateFactory.NewLatLngBounds (boundsBuilder.Build (), 100));
					Log.Debug (TAG, "map zoomed with NewLatLngBounds");
				} catch {
					Log.Debug (TAG, "exception while zooming with NewLatLngBounds");
				}
			}
		}
	}
}