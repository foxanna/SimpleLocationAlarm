using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace LocationAlarm.WindowsPhone.Common
{
    public static class AlarmColors
    {
        public static Color ActiveAlarmDarkColor { get { return Color.FromArgb(255, 153, 51, 204); } }
        public static Color InactiveAlarmDarkColor { get { return Color.FromArgb(255, 139, 139, 139); } }
        public static Color ActiveAlarmLightColor { get { return Color.FromArgb(102, 229, 202, 242); } }
        public static Color InactiveAlarmLightColor { get { return Color.FromArgb(102, 207, 207, 207); } }
    }

    public static class AlarmPinUris
    {
        public static Uri ActiveAlarmPin { get { return new Uri("ms-appx:///Assets/marker_violet.png"); } }
        public static Uri InactiveAlarmPin { get { return new Uri("ms-appx:///Assets/marker_grey.png"); } }
    }
}
