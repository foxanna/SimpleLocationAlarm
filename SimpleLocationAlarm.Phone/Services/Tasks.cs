using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace SimpleLocationAlarm.Phone.Services
{
    public class GeofenceEnteredTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();
            _deferral.Complete();
        }

        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            //var settings = ApplicationData.Current.LocalSettings;
            //var key = task.TaskId.ToString();
            //var message = settings.Values[key].ToString();
            //UpdateUIExampleMethod(message);
        }
    }
}
