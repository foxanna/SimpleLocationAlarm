using System;
using LocationAlarm.PCL.Services.Alarms;
using LocationAlarm.PCL.Services.Database;
using LocationAlarm.PCL.Services.Logs;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;

namespace LocationAlarm.PCL
{
    public static class IoC
    {
        static IoC()
        {
            NinjectKernel = new StandardKernel(new ServicesNinjectModule());
        }

        public static IKernel NinjectKernel { get; set; }

        public static T Get<T>()
        {
            var parameters = new IParameter[0];
            return NinjectKernel.Get<T>(parameters);
        }

        public static object Get(Type service)
        {
            var parameters = new IParameter[0];
            return NinjectKernel.Get(service, parameters);
        }

        public static void Bind<I, K>() where K : I
        {
            NinjectKernel.Bind<I>().To<K>().InSingletonScope();
        }
    }

    public class ServicesNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogService>().To<LogService>().InSingletonScope();
            Bind<IDatabaseManager>().To<DatabaseManager>().InSingletonScope();
            Bind<IAlarmsManager>().To<AlarmsManager>().InSingletonScope();
        }
    }
}