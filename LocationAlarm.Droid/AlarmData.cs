using System;
using Android.Gms.Location;
using SQLite;
using Android.OS;
using Java.Interop;

namespace SimpleLocationAlarm.Droid
{
	public class AlarmData
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public float Radius { get; set; }

		public string Name { get; set; }

        public string RequestId { get; set; }

        public bool Enabled { get; set; }
	}
}