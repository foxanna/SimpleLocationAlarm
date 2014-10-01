using SimpleLocationAlarm.Phone.Common;
using SimpleLocationAlarm.Phone.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace SimpleLocationAlarm.Phone.Views
{
    public sealed partial class MapPage : Page
    {
        private readonly NavigationHelper navigationHelper;

        public MapPageViewModel MapPageViewModel { get; private set; }

        public MapPage()
        {
            MapPageViewModel = new MapPageViewModel();

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

            MapPageViewModel.Alarms.CollectionChanged += Alarms_CollectionChanged;
            MapPageViewModel.Load();
        }

        void Alarms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ZoomToMyLocationAndAlarms();
        }

        async void ZoomToMyLocationAndAlarms()
        {
            var locations = new List<BasicGeoposition>();

            if (MapPageViewModel.Alarms != null && MapPageViewModel.Alarms.Count != 0)
            {
                locations = MapPageViewModel.Alarms.Cast<AlarmItemViewModel>()
                    .Select(alarm => alarm.Location.Position).ToList();
            }

            if (myCurrentLocation != null)
            {
                locations.Add(myCurrentLocation.Coordinate.Point.Position);
            }

            if (locations.Count > 0)
            {
                await Map.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(locations), null, MapAnimationKind.Default);
            }
        }

        Geoposition myCurrentLocation;

        async void LocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (myCurrentLocation == null && MapPageViewModel.Alarms.Count == 0)
            {
                await Map.TrySetViewAsync(args.Position.Coordinate.Point, 16D);
            }

            myCurrentLocation = args.Position;
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            locator.PositionChanged -= LocatorPositionChanged;
            MapPageViewModel.Alarms.CollectionChanged -= Alarms_CollectionChanged;
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
    }
}
