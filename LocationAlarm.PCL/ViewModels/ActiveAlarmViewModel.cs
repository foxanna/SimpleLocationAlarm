using System.Windows.Input;
using LocationAlarm.PCL.Utils;

namespace LocationAlarm.PCL.ViewModels
{
    public class ActiveAlarmViewModel : BaseViewModel
    {
        private AlarmItemViewModel _activeAlarm;
        private ICommand _deleteAlarmCommand;

        public AlarmItemViewModel ActiveAlarm
        {
            get { return _activeAlarm; }
            set
            {
                _activeAlarm = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteAlarmCommand
            => _deleteAlarmCommand ?? (_deleteAlarmCommand = new RelayCommand(DeleteAlarm));

        private void DeleteAlarm()
        {
        }
    }
}