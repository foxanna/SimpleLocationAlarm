using System;
using System.Diagnostics;
using System.Linq;
using LocationAlarm.PCL;
using LocationAlarm.PCL.Utils;
using LocationAlarm.PCL.ViewModels;
using LocationAlarm.WindowsPhone.Common;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace LocationAlarm.WindowsPhone.Views
{
	public sealed partial class MapPage : Page
	{
		public NavigationHelper NavigationHelper { get; private set; }

		readonly Geolocator locator = new Geolocator();

		public MapPageViewModel ViewModel { get; private set; }

		public MapPage()
		{
			ViewModel = IoC.Get<MapPageViewModel>();

			this.InitializeComponent();

			this.NavigationCacheMode = NavigationCacheMode.Required;

			NavigationHelper = new NavigationHelper(this);
			NavigationHelper.LoadState += this.NavigationHelper_LoadState;
			NavigationHelper.SaveState += this.NavigationHelper_SaveState;
		}

		void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
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

		void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
			ViewModel.OnStop();

			GeofenceMonitor.Current.GeofenceStateChanged -= OnGeofenceStateChanged;

			locator.PositionChanged -= LocatorPositionChanged;

			ViewModel.AlarmScreenRequired -= ViewModel_AlarmScreenRequired;
			ViewModel.AddScreenRequired -= ViewModel_AddScreenRequired;
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
			Debug.WriteLine("LocatorPositionChanged to {0};{1}",
				args.Position.Coordinate.Point.Position.Latitude,
				args.Position.Coordinate.Point.Position.Longitude);
			ViewModel.MyCurrentLocation = Tuple.Create<double, double>(args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude);
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
			NavigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			NavigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		async void ViewModel_AddScreenRequired(object sender, EventArgs e)
		{
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				Frame.Navigate(typeof(AddPage), ViewModel.MyCurrentLocation);
			});
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

			foreach (var alarm in ViewModel.Alarms.Select(alarmViewModel => alarmViewModel.Alarm))
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

		void OnGeofenceStateChanged(GeofenceMonitor sender, object args)
		{
			var reports = GeofenceMonitor.Current.ReadReports().Where(report => report.NewState == GeofenceState.Entered).Select(report => report.Geofence.Id);
			ViewModel.OnGeofenceInForeground(reports);
		}

		async void ViewModel_AlarmScreenRequired(object sender, AlarmItemViewModel e)
		{
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				Frame.Navigate(typeof(ActiveAlarmPage), e);
			});
		}
	}
}
