using System;
using LocationAlarm.PCL.ViewModels;
using LocationAlarm.WindowsPhone.Common;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using LocationAlarm.PCL;
using LocationAlarm.PCL.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LocationAlarm.WindowsPhone.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {
        private readonly NavigationHelper navigationHelper;

        public MapPageViewModel ViewModel { get; private set; }

        public MapPage()
        {
            ViewModel = IoC.Get<MapPageViewModel>();

            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        Geolocator locator = new Geolocator();

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            locator.MovementThreshold = 5;
            locator.PositionChanged += LocatorPositionChanged;

            ViewModel.MapZoomChanged += MapPageViewModel_MapZoomChanged;
            ViewModel.OnStart();
        }

        GeoboundingBox boxToDisplay;

        void MapPageViewModel_MapZoomChanged(object sender, MapZoomChangedEventArgs e)
        {
            boxToDisplay = GeoboundingBox.TryCompute(e.Data.Select(location => 
                new BasicGeoposition() { Latitude = location.Item1, Longitude = location.Item2 }));

            ZoomMap(boxToDisplay);
        }

        async void ZoomMap(GeoboundingBox box)
        {
            if (box != null)
            {
                await Map.TrySetViewBoundsAsync(box, null, MapAnimationKind.Default);
            }
        }

        void LocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            ViewModel.MyCurrentLocation = Tuple.Create<double, double>(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            locator.PositionChanged -= LocatorPositionChanged;

            ViewModel.MapZoomChanged -= MapPageViewModel_MapZoomChanged;
            ViewModel.OnStop();
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
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddPage));
        }

        void Map_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ZoomMap(boxToDisplay);
        }
        
        private void StackPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
