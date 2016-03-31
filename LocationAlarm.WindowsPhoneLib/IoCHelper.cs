using System.IO;
using Windows.Storage;
using LocationAlarm.PCL;
using LocationAlarm.PCL.Services;
using LocationAlarm.WindowsPhoneLib.Services;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;

namespace LocationAlarm.WindowsPhoneLib
{
    public static class IoCHelper
    {
        public static void Init()
        {
            IoC.NinjectKernel.Bind<SQLiteConnection>().ToMethod(ctx => new SQLiteConnection(new SQLitePlatformWinRT(),
                Path.Combine(ApplicationData.Current.LocalFolder.Path, "alarms.db")));
            IoC.NinjectKernel.Bind<IGeofenceManager>().To<GeofenceManager>().InSingletonScope();
            IoC.NinjectKernel.Bind<IUIThreadProvider>().To<UIThreadProvider>().InSingletonScope();
        }
    }
}