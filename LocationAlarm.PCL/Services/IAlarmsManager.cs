using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationAlarm.PCL.Models;

namespace LocationAlarm.PCL.Services
{
    public interface IAlarmsManager
    {
        IReadOnlyCollection<AlarmItem> Alarms { get; }

        void Remove(AlarmItem alarm);
        void AddAlarm(AlarmItem alarm);
        void SwitchEnabled(AlarmItem alarm);
    }
}
