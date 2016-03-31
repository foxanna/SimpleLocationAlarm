using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using LocationAlarm.PCL.Services;
using LocationAlarm.PCL.Services.Logs;

namespace LocationAlarm.WindowsPhoneLib.Services
{
    internal class UIThreadProvider : IUIThreadProvider
    {
        private const string Tag = "UIThreadProvider";

        private readonly ILogService _logService;

        public UIThreadProvider(ILogService logService)
        {
            _logService = logService;
        }

        public async Task RunInUIThread(Action action)
        {
            try
            {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                dispatcher = dispatcher ?? CoreWindow.GetForCurrentThread().Dispatcher;

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        _logService.Log(Tag, e);
                        throw;
                    }
                });
            }
            catch (Exception e)
            {
                _logService.Log(Tag, e);
                throw;
            }
        }
    }
}