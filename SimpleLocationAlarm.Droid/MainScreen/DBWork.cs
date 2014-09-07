using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SimpleLocationAlarm.Droid.MainScreen
{
	public partial class HomeActivity
	{
		List<AlarmData> GetAlarms ()
		{
			return new List<AlarmData> () {
				new AlarmData () { Latitude = 48, Longitude = 35, Radius = new Random ().Next (50, 100) * 200 },
			};
		}
	}
}

