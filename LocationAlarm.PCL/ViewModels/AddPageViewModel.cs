using System;
using System.Collections.Generic;
using LocationAlarm.PCL.Models;
using LocationAlarm.PCL.Services.Alarms;
using LocationAlarm.PCL.Utils;

namespace LocationAlarm.PCL.ViewModels
{
    public class AddPageViewModel : BaseViewModel
    {
        private readonly IAlarmsManager AlarmsManager;

        private Tuple<double, double> location;

        private RelayCommand saveCommand;

        public EventHandler Saved;

        private int selectedRadius;

        private string title;

        public AddPageViewModel(IAlarmsManager alarmsManager)
        {
            AlarmsManager = alarmsManager;

            SelectedRadius = 200;
        }

        public Tuple<double, double> Location
        {
            get { return location; }
            set
            {
                location = value;
                OnPropertyChanged();

                RaisePropertyChanged(() => IsLocationSet);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLocationSet
        {
            get { return Location != null; }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged();

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public List<int> Radiuses
        {
            get { return Constants.Radiuses; }
        }

        public int SelectedRadius
        {
            get { return selectedRadius; }
            set
            {
                selectedRadius = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveCommand
        {
            get { return saveCommand ?? (saveCommand = new RelayCommand(Save, CanSave)); }
        }

        public AlarmItem Alarm
        {
            get
            {
                return new AlarmItem
                {
                    Enabled = true,
                    Latitude = Location.Item1,
                    Longitude = Location.Item2,
                    Radius = SelectedRadius,
                    Title = Title
                };
            }
        }

        private bool CanSave()
        {
            return Location != null && !string.IsNullOrEmpty(Title);
        }

        private async void Save()
        {
            try
            {
                await AlarmsManager.AddAlarm(Alarm);

                OnSaved();
            }
            catch
            {
            }
        }

        private void OnSaved()
        {
            var handler = Saved;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}