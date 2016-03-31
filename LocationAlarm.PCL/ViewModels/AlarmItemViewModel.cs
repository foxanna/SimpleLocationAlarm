using System;
using LocationAlarm.PCL.Models;
using LocationAlarm.PCL.Services.Alarms;
using LocationAlarm.PCL.Utils;

namespace LocationAlarm.PCL.ViewModels
{
    public class AlarmItemViewModel : NotifiableViewModel
    {
        private readonly IAlarmsManager _alarmsManager;

        private AlarmItem _alarm;

        private RelayCommand _deleteCommand, _switchEnableCommand;

        public AlarmItemViewModel(IAlarmsManager alarmsManager)
        {
            _alarmsManager = alarmsManager;
        }

        public string Anchor => "0.5,1";

        public AlarmItem Alarm
        {
            get { return _alarm; }
            set
            {
                _alarm = value;
                OnPropertyChanged();

                Location = Tuple.Create(_alarm.Latitude, _alarm.Longitude);
                RaisePropertyChanged(() => Location);
            }
        }

        public Tuple<double, double> Location { get; private set; }

        public RelayCommand SwitchEnabledCommand => _switchEnableCommand ??
                                                    (_switchEnableCommand = new RelayCommand(OnSwitchEnabled));

        public RelayCommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete));

        private async void Delete()
        {
            try
            {
                await _alarmsManager.Remove(Alarm);
            }
            catch
            {
            }
        }

        private async void OnSwitchEnabled()
        {
            try
            {
                await _alarmsManager.SwitchEnabled(Alarm);
            }
            catch
            {
            }
        }
    }
}