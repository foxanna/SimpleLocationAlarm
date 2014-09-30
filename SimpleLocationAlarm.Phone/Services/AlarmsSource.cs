using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using SimpleLocationAlarm.Phone.Models;

namespace SimpleLocationAlarm.Phone.Services
{
    public class AlarmsSource : INotifyCollectionChanged
    {
        const string fileName = "alarms.json";

        ObservableCollection<AlarmItem> _alarms = new ObservableCollection<AlarmItem>();
        ObservableCollection<AlarmItem> Alarms
        {
            get
            {
                return _alarms;
            }
            set
            {
                if (_alarms != null)
                {
                    _alarms.CollectionChanged -= OnCollectionChanged;
                }

                _alarms = value;

                if (_alarms != null)
                {
                    _alarms.CollectionChanged += OnCollectionChanged;
                }
            }
        }
      
        public async Task<IList<AlarmItem>> GetItemsAsync()
        {
            await EnsureDataLoadedAsync();

            return Alarms;
        }

        public async Task<AlarmItem> GetItemAsync(string uniqueId)
        {
            await EnsureDataLoadedAsync();

            return Alarms.FirstOrDefault(alarm => alarm.UniqueId.Equals(uniqueId));
        }

        public async Task AddAlarm(AlarmItem  alarm)
        {
            Alarms.Add(alarm);

            await SaveMapNoteDataAsync();
        }

        public async Task Remove(AlarmItem alarm)
        {
            Alarms.Remove(alarm);

            await SaveMapNoteDataAsync();
        }

        public async Task Enable(AlarmItem alarm, bool enabled)
        {
            Alarms.FirstOrDefault(a => a.UniqueId.Equals(alarm.UniqueId)).Enabled = enabled;

            await SaveMapNoteDataAsync();
        }

        async Task EnsureDataLoadedAsync()
        {
            if (Alarms.Count == 0)
                await LoadDataAsync();
        }

        async Task LoadDataAsync()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<AlarmItem>));

            try
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName))
                {
                    Alarms = (ObservableCollection<AlarmItem>)jsonSerializer.ReadObject(stream);
                }
            }
            catch
            {
                Alarms = new ObservableCollection<AlarmItem>();
            }
        }

        async Task SaveMapNoteDataAsync()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<AlarmItem>));

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName,
                CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, Alarms);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}