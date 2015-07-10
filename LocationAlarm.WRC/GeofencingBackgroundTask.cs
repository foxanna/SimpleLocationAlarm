using System;
using System.Diagnostics;
using System.Linq;
using LocationAlarm.PCL;
using LocationAlarm.PCL.Services;
using LocationAlarm.WindowsPhoneLib;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Notifications;

namespace LocationAlarm.WRC
{
    public sealed class GeofencingBackgroundTask : IBackgroundTask
    {
        public async static void Register()
        {
            if (!IsTaskRegistered)
            {
                var result = await BackgroundExecutionManager.RequestAccessAsync();
                var builder = new BackgroundTaskBuilder();

                builder.Name = typeof(GeofencingBackgroundTask).Name;
                builder.TaskEntryPoint = typeof(GeofencingBackgroundTask).FullName;
                builder.SetTrigger(new LocationTrigger(LocationTriggerType.Geofence));

                builder.Register();
            }
        }

        public static void Unregister()
        {
            var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(kvp => kvp.Value.Name == typeof(GeofencingBackgroundTask).Name);

            if (entry.Value != null)
                entry.Value.Unregister(true);
        }

        public static bool IsTaskRegistered
        {
            get
            {
                var taskRegistered = false;
                var entry = BackgroundTaskRegistration.AllTasks.FirstOrDefault(kvp => kvp.Value.Name == typeof(GeofencingBackgroundTask).Name);

                if (entry.Value != null)
                    taskRegistered = true;

                return taskRegistered;
            }
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Run of {0}", typeof(GeofencingBackgroundTask).FullName);

            IoCHelper.Init();

            var alarmsManager = IoC.Get<IAlarmsManager>();

            var reports = GeofenceMonitor.Current.ReadReports()
                .Where(report => report.NewState == GeofenceState.Entered).Select(report => report.Geofence.Id);
            var triggeredAlarm = alarmsManager.Alarms.FirstOrDefault(alarm => reports.Contains(alarm.GeofenceId));

            if (triggeredAlarm == null)
            {
                Debug.WriteLine("{0} : no such geofence id", typeof(GeofencingBackgroundTask).FullName);
                return;
            }

			if (triggeredAlarm.Enabled)
			{
				Debug.WriteLine("Alarm is active");

				Debug.WriteLine("{0} : showing toast", typeof(GeofencingBackgroundTask).FullName);

				var toastContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
				var textNodes = toastContent.GetElementsByTagName("text");
				textNodes[0].AppendChild(toastContent.CreateTextNode("You are pretty close to "));
				textNodes[1].AppendChild(toastContent.CreateTextNode(triggeredAlarm.Title));

				ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastContent));
			}
			else
				Debug.WriteLine("Alarm is not active");
        }
    }
}
