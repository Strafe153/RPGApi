using Npgsql;
using System.Data;

namespace DataAccess.Database;

public class DatabaseConnectionProvider(string connectionString)
{
    public IDbConnection CreateConnection() => new NpgsqlConnection(connectionString);
}
