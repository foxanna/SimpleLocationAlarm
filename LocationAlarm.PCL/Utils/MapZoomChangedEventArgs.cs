using System;
using System.Collections.Generic;

namespace LocationAlarm.PCL.Utils
{
    public class MapZoomChangedEventArgs : EventArgs
    {
        public List<Tuple<double, double>> Data { get; private set; }

        public MapZoomChangedEventArgs(List<Tuple<double, double>> data)
        {
            Data = data;
        }
    }
}
