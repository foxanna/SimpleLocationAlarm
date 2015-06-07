using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LocationAlarm.PCL.Services;
using LocationAlarm.PCL.Utils;

namespace LocationAlarm.PCL.ViewModels
{
    public class MapPageViewModel : BaseViewModel
    {
        readonly IAlarmsManager AlarmsManager;

        public MapPageViewModel(IAlarmsManager alarmsManager)
        {
            AlarmsManager = alarmsManager;
        }

        public event EventHandler AlarmsChanged;

        List<AlarmItemViewModel> alarms = new List<AlarmItemViewModel>();
        public List<AlarmItemViewModel> Alarms
        {
            get { return alarms; }
            set
            {
                alarms = value;
                OnPropertyChanged();

                OnAlarmsChanged();
            }
        }

        void OnAlarmsChanged()
        {
            var handler = AlarmsChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public override void OnStart()
        {
            base.OnStart();

            AlarmsManager.AlarmsSetChanged += AlarmsManager_AlarmsSetChanged;

            UpdateAlarms();
            OnMapZoomChanged();
        }

        public override void OnStop()
        {
            AlarmsManager.AlarmsSetChanged -= AlarmsManager_AlarmsSetChanged;

            base.OnStop();
        }

        void AlarmsManager_AlarmsSetChanged(object sender, EventArgs e)
        {
            UpdateAlarms();
            OnMapZoomChanged();
        }

        void UpdateAlarms()
        {
            Alarms = AlarmsManager.Alarms.Select(alarm => new AlarmItemViewModel(AlarmsManager) { Alarm = alarm }).ToList();
        }
        
        public event EventHandler<MapZoomChangedEventArgs> MapZoomChanged;

        void OnMapZoomChanged()
        {
            var handler = MapZoomChanged;
            if (handler != null)
            {
                var locations = Alarms.Select(alarm => alarm.Location).ToList();

                if (MyCurrentLocation != null)
                    locations.Add(MyCurrentLocation);

                if (locations.Any())
                    handler(this, new MapZoomChangedEventArgs(locations));
            }
        }

        Tuple<double, double> myCurrentLocation;
        public Tuple<double, double> MyCurrentLocation
        {
            get { return myCurrentLocation; }
            set
            {
                var oldValue = myCurrentLocation;
                myCurrentLocation = value;

                if (oldValue == null && myCurrentLocation != null)
                {
                    OnMapZoomChanged();
                }
            }
        }
    }
}
