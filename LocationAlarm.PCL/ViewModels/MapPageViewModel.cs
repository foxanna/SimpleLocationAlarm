using System;
using System.Collections.Generic;
using System.Linq;
using LocationAlarm.PCL.Services.Alarms;
using LocationAlarm.PCL.Utils;

namespace LocationAlarm.PCL.ViewModels
{
    public class MapPageViewModel : BaseViewModel
    {
        private readonly IAlarmsManager _alarmsManager;

        private RelayCommand _addCommand;

        private List<AlarmItemViewModel> _alarms = new List<AlarmItemViewModel>();

        private Tuple<double, double> _myCurrentLocation;

        public MapPageViewModel(IAlarmsManager alarmsManager)
        {
            _alarmsManager = alarmsManager;
        }

        public List<AlarmItemViewModel> Alarms
        {
            get { return _alarms; }
            set
            {
                _alarms = value;
                OnPropertyChanged();

                AlarmsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Tuple<double, double> MyCurrentLocation
        {
            get { return _myCurrentLocation; }
            set
            {
                var oldValue = _myCurrentLocation;
                _myCurrentLocation = value;

                if (oldValue == null && _myCurrentLocation != null)
                {
                    OnMapZoomChanged();
                }
            }
        }

        public RelayCommand AddCommand => _addCommand ?? (_addCommand = new RelayCommand(OnAddScreenRequired));

        public event EventHandler AlarmsChanged;

        public override void OnStart()
        {
            base.OnStart();

            _alarmsManager.AlarmsSetChanged += AlarmsManager_AlarmsSetChanged;

            UpdateAlarms();
            OnMapZoomChanged();
        }

        public override void OnStop()
        {
            _alarmsManager.AlarmsSetChanged -= AlarmsManager_AlarmsSetChanged;

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
                _alarmsManager.Alarms.Select(alarm => new AlarmItemViewModel(_alarmsManager) {Alarm = alarm}).ToList();
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
            var triggeredAlarm = _alarmsManager.Alarms.FirstOrDefault(alarm => ids.Contains(alarm.GeofenceId));

            if (triggeredAlarm == null)
                return;

            if (triggeredAlarm.Enabled)
                AlarmScreenRequired?.Invoke(this, new AlarmItemViewModel(_alarmsManager) {Alarm = triggeredAlarm});
        }

        public event EventHandler<AlarmItemViewModel> AlarmScreenRequired;

        public event EventHandler AddScreenRequired;

        private void OnAddScreenRequired()
        {
            AddScreenRequired?.Invoke(this, EventArgs.Empty);
        }
    }
}