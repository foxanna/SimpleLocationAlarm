using System.ComponentModel;
using System.Threading.Tasks;
using SimpleLocationAlarm.Phone.Services;
using Windows.Devices.Geolocation;
using SimpleLocationAlarm.Phone.Models;
using System;
using System.Windows.Input;

namespace SimpleLocationAlarm.Phone.ViewModels
{
    public class AddPageViewModel : BaseViewModel
    {
        public AddPageViewModel()
        {
            Anchor = ".5,1";
            SaveCommand = new SaveLocationMarkCommand(this);
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
            }
        }

        public SaveLocationMarkCommand SaveCommand { get; private set; }

        public bool IsLocationSet
        {
            get { return Location != null; }
        }
        
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

        public class SaveLocationMarkCommand : ICommand
        {
            AddPageViewModel viewModel;

            public SaveLocationMarkCommand(AddPageViewModel viewModel)
            {
                this.viewModel = viewModel;

                viewModel.PropertyChanged += viewModel_PropertyChanged;
            }

            void viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName.Equals("Title") || e.PropertyName.Equals("Location"))
                {
                    OnCanExecuteChanged();
                }
            }

            public bool CanExecute(object parameter)
            {
                return viewModel.IsLocationSet && !string.IsNullOrEmpty(viewModel.Title);
            }

            public event EventHandler CanExecuteChanged;

            void OnCanExecuteChanged()
            {
                var handler = CanExecuteChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }

            public async void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    await viewModel.Save();
                }
            }
        }
    }
}
