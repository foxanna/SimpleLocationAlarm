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
using Windows.UI;
using Windows.Storage.Streams;
using Windows.Foundation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LocationAlarm.WindowsPhone.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {
        readonly NavigationHelper navigationHelper;
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        readonly Geolocator locator = new Geolocator();
        
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

        void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            locator.MovementThreshold = 5;
            locator.PositionChanged += LocatorPositionChanged;

            ViewModel.MapZoomChanged += ViewModel_MapZoomChanged;
            ViewModel.AlarmsChanged += ViewModel_AlarmsChanged;

            ViewModel.OnStart();
        }

        void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            ViewModel.OnStop();

            locator.PositionChanged -= LocatorPositionChanged;

            ViewModel.AlarmsChanged -= ViewModel_AlarmsChanged;
            ViewModel.MapZoomChanged -= ViewModel_MapZoomChanged;
        }

        GeoboundingBox boxToDisplay;

        void ViewModel_MapZoomChanged(object sender, MapZoomChangedEventArgs e)
        {
            boxToDisplay = GeoboundingBox.TryCompute(e.Data.Select(location => 
                new BasicGeoposition() { Latitude = location.Item1, Longitude = location.Item2 }));
            
            isSomeAlarmSelected = false;
            ZoomMap(boxToDisplay);
        }

        async void ZoomMap(GeoboundingBox box)
        {
            if (box != null)
                await Map.TrySetViewBoundsAsync(box, null, MapAnimationKind.Default);
        }

        void LocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            ViewModel.MyCurrentLocation = Tuple.Create<double, double>(args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Longitude);
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
            Frame.Navigate(typeof(AddPage), ViewModel.MyCurrentLocation);
        }

        void Map_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DrawAlarmsOnMap();
            ZoomMap(boxToDisplay);
        }

        bool isSomeAlarmSelected;
        void StackPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            isSomeAlarmSelected = true;
            e.Handled = true;
            ZoomMap(GeoboundingBox.TryCompute(((sender as FrameworkElement).DataContext as AlarmItemViewModel).Alarm.GetPointsForCirle()));
        }

        void StackPanel_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        void ViewModel_AlarmsChanged(object sender, EventArgs e)
        {
            DrawAlarmsOnMap();
        }

        void DrawAlarmsOnMap()
        {
            Map.MapElements.Clear();

            foreach(var alarm in ViewModel.Alarms.Select(alarmViewModel => alarmViewModel.Alarm))
            {
                var mapCircle = new MapPolygon
                {
                    StrokeThickness = 1,
                    StrokeColor = alarm.Enabled ? AlarmColors.ActiveAlarmDarkColor : AlarmColors.InactiveAlarmDarkColor,
                    FillColor = alarm.Enabled ? AlarmColors.ActiveAlarmLightColor : AlarmColors.InactiveAlarmLightColor,
                    StrokeDashed = false,
                    Path = new Geopath(alarm.GetPointsForCirle()),
                };
                Map.MapElements.Add(mapCircle);
            }
        }
        
        void Map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            if (isSomeAlarmSelected)
                ZoomMap(boxToDisplay);
        }
    }
}
