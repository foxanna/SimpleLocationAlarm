using System.Collections.Generic;
using SQLite.Net;

namespace LocationAlarm.PCL.Services
{
    class DatabaseManager : IDatabaseManager
    {
        readonly SQLiteConnection DatabaseConnection;

        public DatabaseManager(SQLiteConnection sqLiteConnection)
        {
            DatabaseConnection = sqLiteConnection;
        }
        
        public void CreateTable<T>() where T : class, new()
        {
            DatabaseConnection.CreateTable<T>();
        }

        public IEnumerable<T> GetAll<T>() where T : class, new()
        {
            return DatabaseConnection.Table<T>();
        }

        public T GetById<T>(int id) where T : class, new()
        {
            return DatabaseConnection.Get<T>(id);
        }

        public void Add<T>(T data) where T : class, new()
        {
            DatabaseConnection.Insert(data);
        }

        public void Update<T>(T data) where T : class, new()
        {
            DatabaseConnection.Update(data);
        }

        public void Delete<T>(T data) where T : class, new()
        {
            DatabaseConnection.Delete(data);
        }

        public void Delete<T>(int id) where T : class, new()
        {
            DatabaseConnection.Delete<T>(id);
        }
    }
}
