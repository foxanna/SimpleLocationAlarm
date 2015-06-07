using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationAlarm.PCL.Models;
using Windows.Devices.Geolocation;

namespace LocationAlarm.WindowsPhone.Common
{
    public static class MapUtils
    {
        //http://dotnetbyexample.blogspot.com/2014/02/drawing-circle-shapes-on-windows-phone.html

        public static IEnumerable<BasicGeoposition> GetPointsForCirle(this AlarmItem source)
        {
            var center = new BasicGeoposition { Latitude = source.Latitude, Longitude = source.Longitude };

            var locations = new List<BasicGeoposition>();
            for (var i = 0; i <= 180; i++)
                locations.Add(center.GetAtDistanceBearing(source.Radius, 2 * i));

            return locations;
        }

        public static BasicGeoposition GetAtDistanceBearing(this BasicGeoposition point, double distance, double bearing)
        {
            const double degreesToRadian = Math.PI / 180.0;
            const double radianToDegrees = 180.0 / Math.PI;
            const double earthRadius = 6378137.0;

            var latA = point.Latitude * degreesToRadian;
            var lonA = point.Longitude * degreesToRadian;
            var angularDistance = distance / earthRadius;
            var trueCourse = bearing * degreesToRadian;

            var lat = Math.Asin(
                Math.Sin(latA) * Math.Cos(angularDistance) +
                Math.Cos(latA) * Math.Sin(angularDistance) * Math.Cos(trueCourse));

            var dlon = Math.Atan2(
                Math.Sin(trueCourse) * Math.Sin(angularDistance) * Math.Cos(latA),
                Math.Cos(angularDistance) - Math.Sin(latA) * Math.Sin(lat));

            var lon = ((lonA + dlon + Math.PI) % (Math.PI * 2)) - Math.PI;

            var result = new BasicGeoposition { Latitude = lat * radianToDegrees, Longitude = lon * radianToDegrees };

            return result;
        }
    }
}
