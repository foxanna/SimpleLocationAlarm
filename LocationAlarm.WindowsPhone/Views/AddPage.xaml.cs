using LocationAlarm.WindowsPhone.Common;
using LocationAlarm.PCL.ViewModels;
using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LocationAlarm.PCL;
using Windows.UI.Xaml.Controls.Maps;
using LocationAlarm.PCL.Models;

namespace LocationAlarm.WindowsPhone.Views
{
    public sealed partial class AddPage : Page
	{
		public NavigationHelper NavigationHelper { get; private set; }
        
        public AddPageViewModel ViewModel { get; private set; }

        public AddPage()
        {
            ViewModel = IoC.Get<AddPageViewModel>();

            this.InitializeComponent();

            NavigationHelper = new NavigationHelper(this);
            NavigationHelper.LoadState += this.NavigationHelper_LoadState;
            NavigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            ViewModel.Saved += OnSaved;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            if (e.NavigationParameter != null)
            {
                var currentPosition = e.NavigationParameter as Tuple<double, double>;
                await Map.TrySetViewAsync(new Geopoint(new BasicGeoposition { Latitude = currentPosition.Item1, Longitude = currentPosition.Item2 }), 16D);
            }
            else
            {
                var locator = new Geolocator();
                locator.DesiredAccuracyInMeters = 50;

                var position = await locator.GetGeopositionAsync();
                await Map.TrySetViewAsync(position.Coordinate.Point, 16D);
            }
        }
                
        void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            ViewModel.Saved -= OnSaved;
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
			NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        void Map_MapTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            ViewModel.Location = Tuple.Create<double, double>(args.Location.Position.Latitude, args.Location.Position.Longitude);
        }

        void OnSaved(object sender, EventArgs e)
        {
            Frame.GoBack();      
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ("Location".Equals (e.PropertyName) || "SelectedRadius".Equals(e.PropertyName))
            {
                Map.MapElements.Clear();

                var mapCircle = new MapPolygon
                {
                    StrokeThickness = 1,
                    StrokeColor = AlarmColors.ActiveAlarmDarkColor,
                    FillColor = AlarmColors.ActiveAlarmLightColor,
                    StrokeDashed = false,
                    Path = new Geopath(ViewModel.Alarm.GetPointsForCirle()),
                };
                Map.MapElements.Add(mapCircle);
            }
        }
    }
}
