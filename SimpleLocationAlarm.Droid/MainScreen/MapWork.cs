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
    public partial class HomeActivity
    {
        GoogleMap _map;

        BitmapDescriptor _alarm_marker_normal, _alarm_normal_selected;

        Marker _alarmToAdd;

        List<AlarmData> _mapData = new List<AlarmData>();
        List<Marker> _currentMarkers = new List<Marker>();
        List<Circle> _currentCircles = new List<Circle>();

        void AnimateTo(Location location)
        {
            if (location != null)
            {
                _map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(
                    new LatLng(location.Latitude, location.Longitude), _map.MaxZoomLevel - 6));
            }
        }

        void FindMap()
        {
            _map = (SupportFragmentManager.FindFragmentById(Resource.Id.map) as SupportMapFragment).Map;
            if (_map != null)
            {
                _map.MyLocationEnabled = true;

                _map.UiSettings.TiltGesturesEnabled = false;
                _map.UiSettings.RotateGesturesEnabled = false;

                _map.MapClick += OnMapClick;
                _map.MyLocationChange += HandleMyLocationChange;
                _map.MarkerClick += OnMarkerClick;

                AskToRefreshData();
            }
        }

        Location GetLastKnownLocation()
        {
            var locationManager = LocationManager.FromContext(this);

            return locationManager.GetLastKnownLocation(LocationManager.GpsProvider) ??
            locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
        }

        Location _myCurrentLocation;

        Location MyCurrentLocation
        {
            get
            {
                return _myCurrentLocation ?? GetLastKnownLocation();
            }
        }

        bool _wasZoomedToCurrentLocation;

        void HandleMyLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            Log.Debug(TAG, "New location detected");

            _myCurrentLocation = e.Location;

            if (!_wasZoomedToCurrentLocation)
            {
                ZoomToMyLocationAndAlarms();
            }

            _wasZoomedToCurrentLocation = true;
        }

        void LooseMap()
        {
            if (_map != null)
            {
                _map.MapClick -= OnMapClick;
                _map.MyLocationChange -= HandleMyLocationChange;
                _map.MarkerClick -= OnMarkerClick;

                _map.Clear();

                _map = null;
            }
        }

        void OnDataUpdated(object sender, AlarmsEventArgs e)
        {
            Log.Debug(TAG, "OnDataUpdated, count = " + e.Data.Count);

            _mapData = e.Data;

            if (Mode == Mode.None)
            {
                RedrawMapData();
                ZoomToMyLocationAndAlarms();
            }
        }

        void AskToRefreshData()
        {
            StartService(new Intent(Constants.DatabaseService_SendDatabaseState_Action));
        }

        void RedrawMapData()
        {
            _currentCircles.Clear();
            _currentMarkers.Clear();

            if (_map == null)
            {
                return;
            }

            _map.Clear();

            foreach (var alarm in _mapData)
            {
                _currentCircles.Add(_map.AddCircle(new CircleOptions()
                    .InvokeCenter(new LatLng(alarm.Latitude, alarm.Longitude))
                    .InvokeRadius(alarm.Radius)
                    //	.InvokeStrokeColor (Resources.GetColor (Android.Resource.Color.HoloBlueLight))
                ));

                _currentMarkers.Add( _map.AddMarker(new MarkerOptions()
                    .SetPosition(new LatLng(alarm.Latitude, alarm.Longitude))
                    .SetTitle(alarm.Name)
                    .InvokeIcon(_alarm_marker_normal)));
            }

            Log.Debug(TAG, "data redrawn");
        }

        void ZoomToMyLocationAndAlarms()
        {
            var location = MyCurrentLocation;

            if (_mapData.Count > 0)
            {
                var boundsBuilder = new LatLngBounds.Builder();

                foreach (var alarm in _mapData)
                {
                    boundsBuilder.Include(new LatLng(alarm.Latitude, alarm.Longitude));
                }

                if (location != null)
                {
                    boundsBuilder.Include(new LatLng(location.Latitude, location.Longitude));
                }

                try
                {
                    _map.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(boundsBuilder.Build(), 200));
                    Log.Debug(TAG, "map zoomed with NewLatLngBounds");
                }
                catch
                {
                    Log.Debug(TAG, "exception while zooming with NewLatLngBounds");
                }
            }
            else
            {
                AnimateTo(location);
            }
        }

        void OnMapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            switch (Mode)
            {
                case Mode.Add:
                    if (_alarmToAdd == null)
                    {
                        _alarmToAdd = _map.AddMarker(new MarkerOptions().SetPosition(e.Point));
                        _alarmToAdd.SetIcon(_alarm_marker_normal);
                        _alarmToAdd.Draggable = true;
                    }
                    else
                    {
                        _alarmToAdd.Position = e.Point;
                    }

                    break;

                case Mode.MarkerSelected:
                    if (_selectedMarker != null)
                    {
                        _selectedMarker.SetIcon(_alarm_marker_normal);
                        _selectedMarker = null;
                    }
                    Mode = Mode.None;

                    break;
            }
        }

        Marker _selectedMarker;

        void OnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            switch (Mode)
            {
                case Mode.None:
                case Mode.MarkerSelected:
                    if (_selectedMarker != null)
                    {
                        _selectedMarker.SetIcon(_alarm_marker_normal);
                    }
                    _selectedMarker = e.Marker;
                    _selectedMarker.SetIcon(_alarm_normal_selected);
                    Mode = Mode.MarkerSelected;
                break;
            }

            e.Handled = false;
        }
    }
}