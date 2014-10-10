using System;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Xaml;

namespace SimpleLocationAlarm.Phone.Services
{
    public class GeofenceManager
    {
        public void AddGeofence(string fenceKey, double lat, double lng, double radius)
        {           
            BasicGeoposition position;
            position.Latitude = lat;
            position.Longitude = lng;
            position.Altitude = 0.0;
                        
            var geocircle = new Geocircle(position, radius);


            MonitoredGeofenceStates mask = MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Removed;

            var geofence = new Geofence(fenceKey, geocircle, mask, false, new TimeSpan(0), default(DateTimeOffset), new TimeSpan(0));
            GeofenceMonitor.Current.Geofences.Add(geofence);
        }

           async private void RegisterBackgroundTask(object sender, RoutedEventArgs e)
{
    // Get permission for a background task from the user. If the user has already answered once,
    // this does nothing and the user must manually update their preference via PC Settings.
    BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

    // Regardless of the answer, register the background task. If the user later adds this application
    // to the lock screen, the background task will be ready to run.
    // Create a new background task builder
    BackgroundTaskBuilder geofenceTaskBuilder = new BackgroundTaskBuilder();

    geofenceTaskBuilder.Name = "asdsdf";
    geofenceTaskBuilder.TaskEntryPoint = SampleBackgroundTaskEntryPoint;

    // Create a new location trigger
    var trigger = new LocationTrigger(LocationTriggerType.Geofence);

    // Associate the locationi trigger with the background task builder
    geofenceTaskBuilder.SetTrigger(trigger);

    // If it is important that there is user presence and/or
    // internet connection when OnCompleted is called
    // the following could be called before calling Register()
    // SystemCondition condition = new SystemCondition(SystemConditionType.UserPresent | SystemConditionType.InternetAvailable);
    // geofenceTaskBuilder.AddCondition(condition);

    // Register the background task
    geofenceTask = geofenceTaskBuilder.Register();

    // Associate an event handler with the new background task
    geofenceTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);

    BackgroundTaskState.RegisterBackgroundTask(BackgroundTaskState.LocationTriggerBackgroundTaskName);

    switch (backgroundAccessStatus)
    {
    case BackgroundAccessStatus.Unspecified:
    case BackgroundAccessStatus.Denied:
        rootPage.NotifyUser("This application must be added to the lock screen before the background task will run.", NotifyType.ErrorMessage);
        break;

    
}
        }
    }
}
