using System;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using LocationAlarm.PCL;
using LocationAlarm.PCL.Utils;
using LocationAlarm.PCL.ViewModels;
using LocationAlarm.WindowsPhone.Common;

namespace LocationAlarm.WindowsPhone.Views
{
    public sealed partial class MapPage : Page
    {
        private readonly Geolocator locator = new Geolocator();

        private GeoboundingBox boxToDisplay;

        private bool isSomeAlarmSelected;

        public MapPage()
        {
            ViewModel = IoC.Get<MapPageViewModel>();

            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

            NavigationHelper = new NavigationHelper(this);
            NavigationHelper.LoadState += NavigationHelper_LoadState;
            NavigationHelper.SaveState += NavigationHelper_SaveState;
        }

        public NavigationHelper NavigationHelper { get; }

        public MapPageViewModel ViewModel { get; }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            locator.MovementThreshold = 5;
            locator.PositionChanged += LocatorPositionChanged;

            ViewModel.MapZoomChanged += ViewModel_MapZoomChanged;
            ViewModel.AlarmsChanged += ViewModel_AlarmsChanged;
            ViewModel.AlarmScreenRequired += ViewModel_AlarmScreenRequired;
            ViewModel.AddScreenRequired += ViewModel_AddScreenRequired;

            GeofenceMonitor.Current.GeofenceStateChanged += OnGeofenceStateChanged;

            ViewModel.OnStart();
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            ViewModel.OnStop();

            GeofenceMonitor.Current.GeofenceStateChanged -= OnGeofenceStateChanged;

            locator.PositionChanged -= LocatorPositionChanged;

            ViewModel.AlarmScreenRequired -= ViewModel_AlarmScreenRequired;
            ViewModel.AddScreenRequired -= ViewModel_AddScreenRequired;
            ViewModel.AlarmsChanged -= ViewModel_AlarmsChanged;
            ViewModel.MapZoomChanged -= ViewModel_MapZoomChanged;
        }

        private void ViewModel_MapZoomChanged(object sender, MapZoomChangedEventArgs e)
        {
            boxToDisplay = GeoboundingBox.TryCompute(e.Data.Select(location =>
                new BasicGeoposition {Latitude = location.Item1, Longitude = location.Item2}));

            isSomeAlarmSelected = false;
            ZoomMap(boxToDisplay);
        }

        private async void ZoomMap(GeoboundingBox box)
        {
            if (box != null)
                await Map.TrySetViewBoundsAsync(box, null, MapAnimationKind.Default);
        }

        private void LocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Debug.WriteLine("LocatorPositionChanged to {0};{1}",
                args.Position.Coordinate.Point.Position.Latitude,
                args.Position.Coordinate.Point.Position.Longitude);
            ViewModel.MyCurrentLocation = Tuple.Create(args.Position.Coordinate.Point.Position.Latitude,
                args.Position.Coordinate.Point.Position.Longitude);
        }

        private async void ViewModel_AddScreenRequired(object sender, EventArgs e)
        {
            await
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { Frame.Navigate(typeof (AddPage), ViewModel.MyCurrentLocation); });
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            DrawAlarmsOnMap();
            ZoomMap(boxToDisplay);
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            isSomeAlarmSelected = true;
            e.Handled = true;
            ZoomMap(
                GeoboundingBox.TryCompute(
                    ((sender as FrameworkElement).DataContext as AlarmItemViewModel).Alarm.GetPointsForCirle()));
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement) sender);
        }

        private void ViewModel_AlarmsChanged(object sender, EventArgs e)
        {
            DrawAlarmsOnMap();
        }

        private void DrawAlarmsOnMap()
        {
            Map.MapElements.Clear();

            foreach (var alarm in ViewModel.Alarms.Select(alarmViewModel => alarmViewModel.Alarm))
            {
                var mapCircle = new MapPolygon
                {
                    StrokeThickness = 1,
                    StrokeColor = alarm.Enabled ? AlarmColors.ActiveAlarmDarkColor : AlarmColors.InactiveAlarmDarkColor,
                    FillColor = alarm.Enabled ? AlarmColors.ActiveAlarmLightColor : AlarmColors.InactiveAlarmLightColor,
                    StrokeDashed = false,
                    Path = new Geopath(alarm.GetPointsForCirle())
                };
                Map.MapElements.Add(mapCircle);
            }
        }

        private void Map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            if (isSomeAlarmSelected)
                ZoomMap(boxToDisplay);
        }

        private void OnGeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports =
                GeofenceMonitor.Current.ReadReports()
                    .Where(report => report.NewState == GeofenceState.Entered)
                    .Select(report => report.Geofence.Id);
            ViewModel.OnGeofenceInForeground(reports);
        }

        private async void ViewModel_AlarmScreenRequired(object sender, AlarmItemViewModel e)
        {
            await
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { Frame.Navigate(typeof (ActiveAlarmPage), e); });
        }

        #region NavigationHelper registration

        /// <summary>
        ///     The methods provided in this section are simply used to allow
        ///     NavigationHelper to respond to the page's navigation methods.
        ///     <para>
        ///         Page specific logic should be placed in event handlers for the
        ///         <see cref="NavigationHelper.LoadState" />
        ///         and <see cref="NavigationHelper.SaveState" />.
        ///         The navigation parameter is available in the LoadState method
        ///         in addition to page state preserved during an earlier session.
        ///     </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}