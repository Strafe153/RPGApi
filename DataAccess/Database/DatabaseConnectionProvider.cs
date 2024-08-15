using Domain.Constants;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace DataAccess.Database;

public class DatabaseConnectionProvider
{
    private readonly string _databaseConnectionString;

    public DatabaseConnectionProvider(IConfiguration configuration)
    {
        _databaseConnectionString = configuration.GetConnectionString(ConnectionStringConstants.DatabaseConnection);
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_databaseConnectionString);
}
