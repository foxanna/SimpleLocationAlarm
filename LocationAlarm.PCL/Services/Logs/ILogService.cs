using System;

namespace LocationAlarm.PCL.Services.Logs
{
    public interface ILogService
    {
        void Log(string tag, string format, params object[] args);
        void Log(string tag, Exception e);
    }
}