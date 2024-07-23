using System.Data.SqlClient;

namespace OT.Assessment.App.SQLConnection
{
    public class SqlDatabaseConnection : ISqlDatabaseConnection
    {
        private readonly string _connectionString;

        public SqlDatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
