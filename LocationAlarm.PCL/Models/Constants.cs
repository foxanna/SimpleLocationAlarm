using System.Collections.Generic;

namespace LocationAlarm.PCL.Models
{
    public static class Constants
    {
        public const string DeveloperEmail = "simple.location.notifications@gmail.com";

        public static List<int> Radiuses = new List<int>()
        {
            50, 100, 150, 200, 300, 400, 500, 700, 1000
        };
    }
}