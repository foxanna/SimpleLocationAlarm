using System;
using System.Collections.Generic;
using LocationAlarm.PCL.Models;
using LocationAlarm.PCL.Services.Alarms;
using LocationAlarm.PCL.Utils;

namespace LocationAlarm.PCL.ViewModels
{
    public class AddPageViewModel : BaseViewModel
    {
        private readonly IAlarmsManager _alarmsManager;

        private Tuple<double, double> _location;

        private RelayCommand _saveCommand;

        private int _selectedRadius;

        private string _title;

        public EventHandler Saved;

        public AddPageViewModel(IAlarmsManager alarmsManager)
        {
            _alarmsManager = alarmsManager;

            SelectedRadius = 200;
        }

        public Tuple<double, double> Location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnPropertyChanged();

                RaisePropertyChanged(() => IsLocationSet);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLocationSet => Location != null;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public List<int> Radiuses => Constants.Radiuses;

        public int SelectedRadius
        {
            get { return _selectedRadius; }
            set
            {
                _selectedRadius = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save, CanSave));

        public AlarmItem Alarm => new AlarmItem
        {
            Enabled = true,
            Latitude = Location.Item1,
            Longitude = Location.Item2,
            Radius = SelectedRadius,
            Title = Title
        };

        private bool CanSave()
        {
            return Location != null && !string.IsNullOrEmpty(Title);
        }

        private async void Save()
        {
            try
            {
                await _alarmsManager.AddAlarm(Alarm);

                Saved?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
            }
        }
    }
}