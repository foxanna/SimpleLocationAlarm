using System;
using System.Threading.Tasks;

namespace LocationAlarm.PCL.Services
{
	public interface IUIThreadProvider
	{
		Task RunInUIThread(Action action);
	}
}
