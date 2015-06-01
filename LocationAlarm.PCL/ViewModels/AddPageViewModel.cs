using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LocationAlarm.PCL.Models;
using LocationAlarm.PCL.Services;
using LocationAlarm.PCL.Utils;

namespace LocationAlarm.PCL.ViewModels
{
    public class AddPageViewModel : BaseViewModel
    {
        readonly IAlarmsManager AlarmsManager;

        public AddPageViewModel(IAlarmsManager alarmsManager)
        {
            AlarmsManager = alarmsManager;

            SelectedRadius = 200;
        }
        
        Tuple<double, double> location;
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

        public bool IsLocationSet { get { return Location != null; } }

        string title;
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

        public List<int> Radiuses { get { return Constants.Radiuses; } }

        public int SelectedRadius { get; set; }
                
        RelayCommand saveCommand;
        public RelayCommand SaveCommand 
        {
            get { return saveCommand ?? (saveCommand = new RelayCommand(Save, CanSave)); }
        }

        bool CanSave()
        {
            return Location != null && !string.IsNullOrEmpty(Title);
        }

        void Save()
        {
            AlarmsManager.AddAlarm(new AlarmItem()
            {
                Enabled = true,
                Latitude = Location.Item1,
                Longitude = Location.Item2,
                Radius = SelectedRadius,
                Title = Title,
            });

            OnSaved();
        }

        public EventHandler Saved;
        void OnSaved()
        {
            var handler = Saved;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
