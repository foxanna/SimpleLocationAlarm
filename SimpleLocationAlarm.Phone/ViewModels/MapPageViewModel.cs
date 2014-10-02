using SimpleLocationAlarm.Phone.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Devices.Geolocation;
using System.Diagnostics;

namespace SimpleLocationAlarm.Phone.ViewModels
{
    public class GeoboundingBoxEventArgs : EventArgs
    {
        public GeoboundingBox Data { get; private set; }

        public GeoboundingBoxEventArgs(GeoboundingBox box)
        {
            Data = box;
        }
    }

    public class MapPageViewModel : BaseViewModel
    {
        public MapPageViewModel()
        {
            Alarms = new ObservableCollection<AlarmItemViewModel>();

            AlarmsSource.Instance.CollectionChanged += (s, e) => Load();
        }

        ObservableCollection<AlarmItemViewModel> alarms;
        public ObservableCollection<AlarmItemViewModel> Alarms
        {
            get { return alarms; }
            set
            {
                alarms = value;
                OnPropertyChanged();
            }
        }

        public async void Load()
        {
            var alarms = await AlarmsSource.Instance.GetItemsAsync();
            var alarmModels = alarms.Select(alarm => new AlarmItemViewModel(alarm)).ToList();
            Alarms = new ObservableCollection<AlarmItemViewModel>(alarmModels);

            OnMapZoomChanged();
        }

        public event EventHandler<GeoboundingBoxEventArgs> MapZoomChanged;

        void OnMapZoomChanged()
        {
            var handler = MapZoomChanged;
            if (handler != null)
            {
                var locations = new List<BasicGeoposition>();

                if (Alarms != null && Alarms.Count != 0)
                {
                    locations = Alarms.Select(alarm => alarm.Location.Position).ToList();
                }

                if (MyCurrentLocation != null)
                {
                    locations.Add(MyCurrentLocation.Coordinate.Point.Position);
                }

                if (locations.Count > 0)
                {
                    handler(this, new GeoboundingBoxEventArgs(GeoboundingBox.TryCompute(locations)));
                }
            }
        }

        Geoposition myCurrentLocation;
        public Geoposition MyCurrentLocation
        {
            get { return myCurrentLocation; }
            set
            {
                var oldValue = myCurrentLocation;

                myCurrentLocation = value;

                if (oldValue == null && myCurrentLocation != null && Alarms.Count == 0)
                {
                    OnMapZoomChanged();
                }
            }
        }
    }
}
