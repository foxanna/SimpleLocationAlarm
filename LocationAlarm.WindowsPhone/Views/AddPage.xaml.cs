using LocationAlarm.WindowsPhone.Common;
using LocationAlarm.PCL.ViewModels;
using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LocationAlarm.PCL;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LocationAlarm.WindowsPhone.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPage : Page
    {
        private NavigationHelper navigationHelper;
        
        public AddPageViewModel AddPageViewModel {get; private set; }

        public AddPage()
        {
            AddPageViewModel = IoC.Get<AddPageViewModel>();

            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;

            var position = await locator.GetGeopositionAsync();
            var myPoint = position.Coordinate.Point;
            
            await Map.TrySetViewAsync(myPoint, 16D);

            AddPageViewModel.Saved += OnSaved;
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            AddPageViewModel.Saved -= OnSaved;
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
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Map_MapTapped(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            AddPageViewModel.Location = Tuple.Create<double, double>(args.Location.Position.Latitude, args.Location.Position.Longitude);
        }

        void OnSaved(object sender, EventArgs e)
        {
            Frame.GoBack();      
        }
    }
}
