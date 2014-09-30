using SimpleLocationAlarm.Phone.Models;
using SimpleLocationAlarm.Phone.Services;
using System.Collections.ObjectModel;

namespace SimpleLocationAlarm.Phone.ViewModels
{
    public class MapPageViewModel
    {
        private readonly AlarmsSource _alarmsSource = new AlarmsSource();

        public MapPageViewModel()
        {
            Alarms = new ObservableCollection<AlarmItemViewModel>();

            _alarmsSource.CollectionChanged += (s, e) => Load();
        }

        public ObservableCollection<AlarmItemViewModel> Alarms { get; set; }

        public async void Load()
        {
            var alarms = await _alarmsSource.GetItemsAsync();

            Alarms.Clear();
            foreach (var alarm in alarms) 
            {
                Alarms.Add(new AlarmItemViewModel(alarm));
            }
        }

        public async void AddAlarm()
        {
            await _alarmsSource.AddAlarm(new AlarmItem()
            {
                UniqueId = "asdasfrtrtytfygh",
                Title = "adksfmdskfm111122222",
                Enabled = false,
                Latitude = 52,
                Longitude = 52,
                Radius = 200,
            });
        }
    }
}
