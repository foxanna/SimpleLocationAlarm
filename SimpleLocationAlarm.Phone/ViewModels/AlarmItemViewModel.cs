using SimpleLocationAlarm.Phone.Models;
using SimpleLocationAlarm.Phone.Services;
using System;
using System.Windows.Input;
using Windows.Devices.Geolocation;

namespace SimpleLocationAlarm.Phone.ViewModels
{
    public class AlarmItemViewModel
    {
        public AlarmItemViewModel(AlarmItem item)
        {
            UniqueID = item.UniqueId;
            Title = item.Title;
            Enabled = item.Enabled;
            Radius = item.Radius;
            Location = new Geopoint(new BasicGeoposition() { Latitude = item.Latitude, Longitude = item.Longitude });
            Anchor = ".5,1";

            EnableCommand = new EnableLocationMarkCommand();
        }

        public string UniqueID { get; set; }
        public string Title { get; set; }
        public bool Enabled { get; set; }
        public double Radius { get; set; }        
        public Geopoint Location { get; private set; }
        public string Anchor { get; private set; }

        public EnableLocationMarkCommand EnableCommand { get; set; }

        public class EnableLocationMarkCommand : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public async void Execute(object parameter)
            {
                if (parameter == null || !(parameter is AlarmItemViewModel))
                    return;

                await AlarmsSource.Instance.SwitchEnable((parameter as AlarmItemViewModel).UniqueID);
            }
        }
    }
}
