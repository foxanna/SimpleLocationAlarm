using SimpleLocationAlarm.Phone.Models;
using SimpleLocationAlarm.Phone.Services;
using System.Collections.ObjectModel;

namespace SimpleLocationAlarm.Phone.ViewModels
{
    public class MapPageViewModel
    {
        public MapPageViewModel()
        {
            Alarms = new ObservableCollection<AlarmItemViewModel>();

            AlarmsSource.Instance.CollectionChanged += (s, e) => Load();
        }

        public ObservableCollection<AlarmItemViewModel> Alarms { get; set; }

        public async void Load()
        {
            var alarms = await AlarmsSource.Instance.GetItemsAsync();

            Alarms.Clear();
            foreach (var alarm in alarms) 
            {
                Alarms.Add(new AlarmItemViewModel(alarm));
            }
        }
    }
}
