using System;
using System.Collections.Generic;
using System.Linq;
using LocationAlarm.PCL.Models;
using LocationAlarm.PCL.Services.Alarms;
using LocationAlarm.PCL.Utils;

namespace LocationAlarm.PCL.ViewModels
{
    public class MapPageViewModel : BaseViewModel
    {
        private readonly IAlarmsManager AlarmsManager;

        private RelayCommand addCommand;

        private List<AlarmItemViewModel> alarms = new List<AlarmItemViewModel>();

        private Tuple<double, double> myCurrentLocation;

        public MapPageViewModel(IAlarmsManager alarmsManager)
        {
            AlarmsManager = alarmsManager;
        }

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

        public RelayCommand AddCommand
        {
            get { return addCommand ?? (addCommand = new RelayCommand(OnAddScreenRequired)); }
        }

        public event EventHandler AlarmsChanged;

        private void OnAlarmsChanged()
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

        private void AlarmsManager_AlarmsSetChanged(object sender, EventArgs e)
        {
            UpdateAlarms();
            OnMapZoomChanged();
        }

        private void UpdateAlarms()
        {
            Alarms =
                AlarmsManager.Alarms.Select(alarm => new AlarmItemViewModel(AlarmsManager) {Alarm = alarm}).ToList();
        }

        public event EventHandler<MapZoomChangedEventArgs> MapZoomChanged;

        private void OnMapZoomChanged()
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

        public void OnGeofenceInForeground(IEnumerable<string> ids)
        {
            var triggeredAlarm = AlarmsManager.Alarms.FirstOrDefault(alarm => ids.Contains(alarm.GeofenceId));

            if (triggeredAlarm == null)
                return;

            if (triggeredAlarm.Enabled)
                OnAlarmScreenRequired(triggeredAlarm);
        }

        public event EventHandler<AlarmItemViewModel> AlarmScreenRequired;

        private void OnAlarmScreenRequired(AlarmItem alarm)
        {
            var handler = AlarmScreenRequired;
            if (handler != null)
                handler(this, new AlarmItemViewModel(AlarmsManager) {Alarm = alarm});
        }

        public event EventHandler AddScreenRequired;

        private void OnAddScreenRequired()
        {
            var handler = AddScreenRequired;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}