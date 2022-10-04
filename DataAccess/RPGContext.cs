using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace DataAccess;

public class RPGContext
{
    private readonly IConfiguration _configuration;
    private readonly IMigrationRunner _migrationRunner;
    private readonly string _databaseConnection;

    public RPGContext(
        IConfiguration configuration,
        IMigrationRunner migrationRunner)
    {
        _configuration = configuration;
        _migrationRunner = migrationRunner;
        _databaseConnection = _configuration.GetConnectionString("DatabaseConnection");

        EnsureDatabase();
    }

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_databaseConnection);
        return connection;
    }

    private void EnsureDatabase()
    {
        using var connection = new NpgsqlConnection(_configuration.GetConnectionString("GlobalPostgresConnection"));
        string query = @"SELECT datname FROM pg_catalog.pg_database 
                         WHERE datname = @DBName";
        var queryParams = new { DBName = "rpg_api_db" };
        string queryResult = connection.QueryFirstOrDefault<string>(query, queryParams);

        if (string.IsNullOrEmpty(queryResult))
        {
            InitializeDatabase();
            _migrationRunner.MigrateUp();
        }
    }

    private void InitializeDatabase()
    {
        using var connection = new NpgsqlConnection(_configuration.GetConnectionString("GlobalPostgresConnection"));
        string query = "CREATE DATABASE rpg_api_db";

        connection.Execute(query);
    }
}
