using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationAlarm.PCL.ViewModels
{
	public class ActiveAlarmViewModel : BaseViewModel
	{
		AlarmItemViewModel activeAlarm;
		public AlarmItemViewModel ActiveAlarm
		{
			get { return activeAlarm; }
			set
			{
				activeAlarm = value;
				OnPropertyChanged();
			}
		}
	}
}
