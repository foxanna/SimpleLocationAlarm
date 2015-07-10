using System;
using System.Threading.Tasks;
using LocationAlarm.PCL.Services;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace LocationAlarm.WindowsPhoneLib
{
	class UIThreadProvider : IUIThreadProvider
	{
		const string Tag = "UIThreadProvider";

		readonly ILogService LogService;

		public UIThreadProvider(ILogService logService)
		{
			LogService = logService;
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
						LogService.Log(Tag, e);
						throw;
					}
				});
			}
			catch (Exception e)
			{
				LogService.Log(Tag, e);
				throw;
			}
		}
	}
}
