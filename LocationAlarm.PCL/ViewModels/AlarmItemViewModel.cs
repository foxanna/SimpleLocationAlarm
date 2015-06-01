using System;
using System.Windows.Input;
using LocationAlarm.PCL.Models;
using LocationAlarm.PCL.Services;
using LocationAlarm.PCL.Utils;

namespace LocationAlarm.PCL.ViewModels
{
    public class AlarmItemViewModel : NotifiableViewModel
    {
        readonly IAlarmsManager AlarmsManager;

        public AlarmItemViewModel(IAlarmsManager alarmsManager)
        {
            AlarmsManager = alarmsManager;
        }

        public string Anchor { get { return "0.5,1"; } }

        AlarmItem alarm;
        public AlarmItem Alarm
        {
            get { return alarm; }
            set
            {
                alarm = value;
                OnPropertyChanged();

                Location = Tuple.Create<double, double>(alarm.Latitude, alarm.Longitude);
                RaisePropertyChanged(() => Location);
            }
        }

        public Tuple<double, double> Location { get; private set; }

        RelayCommand switchEnableCommand;
        public RelayCommand SwitchEnabledCommand
        {
            get
            {
                return switchEnableCommand ?? (switchEnableCommand = new RelayCommand(() => AlarmsManager.SwitchEnabled(Alarm)));
            }
        }

        RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand(() => AlarmsManager.Remove(Alarm)));
            }
        }
    }
}
