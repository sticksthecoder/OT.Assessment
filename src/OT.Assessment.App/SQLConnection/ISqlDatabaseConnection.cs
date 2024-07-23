using System.Data.SqlClient;

namespace OT.Assessment.App.SQLConnection
{
    public interface ISqlDatabaseConnection
    {
        SqlConnection GetConnection();
    }
}
