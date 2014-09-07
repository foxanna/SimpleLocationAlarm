using System;
using Android.Gms.Location;
using SQLite;

namespace SimpleLocationAlarm.Droid
{
	public class AlarmData
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public double Radius { get; set; }
	}
}