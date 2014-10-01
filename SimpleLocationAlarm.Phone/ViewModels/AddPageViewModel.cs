using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SimpleLocationAlarm.Phone.Services;
using Windows.Devices.Geolocation;
using SimpleLocationAlarm.Phone.Models;
using System;
using Windows.ApplicationModel.Resources;

namespace SimpleLocationAlarm.Phone.ViewModels
{
    public class AddPageViewModel : INotifyPropertyChanged
    {
        public AddPageViewModel()
        {
            Anchor = ".5,1";
        }

        public string Anchor { get; private set; }

        Geopoint location;
        public Geopoint Location
        {
            get { return location; }
            set
            {
                location = value;

                OnPropertyChanged();
                OnPropertyChanged("IsLocationSet");
                OnPropertyChanged("CanBeSaved");
            }
        }

        string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;

                OnPropertyChanged();
                OnPropertyChanged("CanBeSaved");
            }
        }

        public bool CanBeSaved
        {
            get
            {
                return !string.IsNullOrEmpty(title) && Location != null;
            }
        }

        public bool IsLocationSet
        {
            get { return Location != null; }
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task Save()
        {
            await AlarmsSource.Instance.AddAlarm(new AlarmItem()
            {
                Enabled = true,
                Latitude = Location.Position.Latitude,
                Longitude = Location.Position.Longitude,
                Radius = 200,
                Title = Title,
                UniqueId = string.Format("{0}_{1}", Title, Location.Position, ToString()),
            });
        }

        //public class SaveLocationMarkCommand : ICommand
        //{
        //    public bool CanExecute(object parameter)
        //    {
        //        if (parameter == null)
        //            return false;

        //        var alarmData = (AddPageViewModel)parameter;
        //        return alarmData.IsLocationSet && !string.IsNullOrEmpty(alarmData.Title);
        //    }

        //    public event EventHandler CanExecuteChanged;

        //    public void OnCanExecuteChanged()
        //    {
        //        CanExecuteChanged(this, EventArgs.Empty);
        //    }

        //    public async void Execute(object parameter)
        //    {
        //        if (CanExecute(parameter))
        //        {
        //            var alarmData = (AddPageViewModel)parameter;
        //            await alarmData.Save();
        //        }
        //    }
        //}
    }
}
