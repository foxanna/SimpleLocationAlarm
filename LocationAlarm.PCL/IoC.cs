using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using LocationAlarm.PCL.Services;
using SQLite.Net;

namespace LocationAlarm.PCL
{
    public static class IoC
    {						
		public static IKernel NinjectKernel { get; set; }

		static IoC()
		{
			NinjectKernel = new StandardKernel(new ServicesNinjectModule());
		}

		public static T Get<T>()
		{
			IParameter[] parameters = new IParameter[0];
			return NinjectKernel.Get<T>(parameters);
		}

		public static object Get(Type service)
		{
			IParameter[] parameters = new IParameter[0];
			return NinjectKernel.Get(service, parameters);
		}

        public static void Bind<I, K> () where K : I
        {
            NinjectKernel.Bind<I>().To<K>().InSingletonScope();
        }
    }

    public class ServicesNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDatabaseManager>().To<DatabaseManager>().InSingletonScope();
            Bind<IAlarmsManager>().To<AlarmsManager>().InSingletonScope();
        }
    }
}
