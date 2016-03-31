using System.Collections.Generic;
using SQLite.Net;

namespace LocationAlarm.PCL.Services.Database
{
    internal class DatabaseManager : IDatabaseManager
    {
        private readonly SQLiteConnection _databaseConnection;

        public DatabaseManager(SQLiteConnection sqLiteConnection)
        {
            _databaseConnection = sqLiteConnection;
        }

        public void CreateTable<T>() where T : class, new()
        {
            _databaseConnection.CreateTable<T>();
        }

        public IEnumerable<T> GetAll<T>() where T : class, new()
        {
            return _databaseConnection.Table<T>();
        }

        public T GetById<T>(int id) where T : class, new()
        {
            return _databaseConnection.Get<T>(id);
        }

        public void Add<T>(T data) where T : class, new()
        {
            _databaseConnection.Insert(data);
        }

        public void Update<T>(T data) where T : class, new()
        {
            _databaseConnection.Update(data);
        }

        public void Delete<T>(T data) where T : class, new()
        {
            _databaseConnection.Delete(data);
        }

        public void Delete<T>(int id) where T : class, new()
        {
            _databaseConnection.Delete<T>(id);
        }
    }
}