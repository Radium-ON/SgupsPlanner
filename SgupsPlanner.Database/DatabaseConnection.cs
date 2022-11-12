using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace SgupsPlanner.Database
{
    public sealed class DatabaseConnection
    {
        public string ConnectionString => _connectionString;

        public SqliteConnection Connection => _connection;

        public Exception RootException { get; set; }

        public DatabaseConnection()
        {
            SetConnectionString();
            Source = this;
        }

        private static readonly Lazy<DatabaseConnection> _lazy =
            new Lazy<DatabaseConnection>(() => new DatabaseConnection());

        private string _connectionString = string.Empty;
        private SqliteConnection _connection;

        public static DatabaseConnection Source { get; private set; }

        public SqliteConnection OpenConnection()
        {
            Connection.Open();
            return Connection;
        }

        public void CloseConnection()
        {
            Connection.Close();
        }

        public void SetConnectionString()
        {
            var docFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dbPath = Path.Combine(docFolderPath, "SgupsPlanner\\sgups-planner.db");
            _connectionString = $"Data Source={dbPath};Mode=ReadWrite;Foreign Keys=True;";
            _connection = new SqliteConnection(ConnectionString);
        }
    }
}