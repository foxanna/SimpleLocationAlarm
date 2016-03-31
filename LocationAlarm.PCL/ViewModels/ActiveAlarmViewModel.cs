namespace LocationAlarm.PCL.ViewModels
{
    public class ActiveAlarmViewModel : BaseViewModel
    {
        private AlarmItemViewModel _activeAlarm;

        public AlarmItemViewModel ActiveAlarm
        {
            get { return _activeAlarm; }
            set
            {
                _activeAlarm = value;
                OnPropertyChanged();
            }
        }
    }
}