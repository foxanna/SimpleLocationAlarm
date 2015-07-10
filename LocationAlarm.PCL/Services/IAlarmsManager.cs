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

        event EventHandler AlarmsSetChanged;

        Task Remove(AlarmItem alarm);
        Task AddAlarm(AlarmItem alarm);
        Task SwitchEnabled(AlarmItem alarm);
    }
}
