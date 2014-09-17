using System;
using System.Collections.Generic;

namespace SimpleLocationAlarm.Droid
{
	public static class Constants
	{
        public const string DatabaseUpdated_Action = "DatabaseUpdated_Action";

		public const string DatabaseService_SendDatabaseState_Action = "DatabaseService_SendDatabaseState_Action";
		public const string DatabaseService_AddAlarm_Action = "DatabaseService_AddAlarm_Action";
        public const string DatabaseService_DeleteAlarm_Action = "DatabaseService_DeleteAlarm_Action";
        public const string DatabaseService_DeleteAll_Action = "DatabaseService_DeleteAll_Action";

		public const string AlarmsData_Extra = "AlarmsData_Extra";

        public const string DeveloperEmail = "simple.location.notifications@gmail.com";

        public static List<int> AlarmRadiusValues = new List<int>() { 50, 100, 150, 200, 300, 400, 500, 700, 1000 };
	}
}