using LocationAlarm.PCL;
using LocationAlarm.PCL.Services;
using SQLite.Net;

namespace LocationAlarm.WindowsPhoneLib
{
    public static class IoCHelper
    {
        public static void Init()
        {
            IoC.NinjectKernel.Bind<SQLiteConnection>().ToMethod(ctx => new SQLiteConnection(
                new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(),
                System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "alarms.db")));

            IoC.NinjectKernel.Bind<IGeofenceManager>().To<GeofenceManager>().InSingletonScope();
        }
    }
}
