using System.Collections.Generic;

namespace LocationAlarm.PCL.Services.Database
{
    public interface IDatabaseManager
    {
        void CreateTable<T>() where T : class, new();
        IEnumerable<T> GetAll<T>() where T : class, new();
        T GetById<T>(int id) where T : class, new();
        void Add<T>(T data) where T : class, new();
        void Update<T>(T data) where T : class, new();
        void Delete<T>(T data) where T : class, new();
        void Delete<T>(int id) where T : class, new();
    }
}