using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using System.Data.SqlClient;
using DotNet.Testcontainers.Containers;
using System.Data.Common;

namespace GatewayRequestApi.FunctionalTests;


public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();
    private readonly DbConnectionFactory _dbConnectionFactory;

    public string DbConnectionString
    {
        get
        {
            return _dbContainer.GetConnectionString();
        }
    }

    private const string Database = "Gateway";

    public FunctionalTestWebAppFactory()
    {
        _dbConnectionFactory = new DbConnectionFactory(_dbContainer, Database);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var connection = _dbConnectionFactory.MasterDbConnection;

        // TODO: Add your database migration here.
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE DATABASE " + Database;

        await connection.OpenAsync()
            .ConfigureAwait(false);

        await command.ExecuteNonQueryAsync()
            .ConfigureAwait(false);
    }

    public Task DisposeAsync()
    {
        return _dbContainer.DisposeAsync().AsTask();
    }

    private sealed class DbConnectionFactory
    {
        private readonly IDatabaseContainer _databaseContainer;

        private readonly string _database;

        public DbConnectionFactory(IDatabaseContainer databaseContainer, string database)
        {
            _databaseContainer = databaseContainer;
            _database = database;
        }

        public DbConnection MasterDbConnection
        {
            get
            {
                return new SqlConnection(_databaseContainer.GetConnectionString());
            }
        }

        public DbConnection CustomDbConnection
        {
            get
            {
                var connectionString = new SqlConnectionStringBuilder(_databaseContainer.GetConnectionString());
                connectionString.InitialCatalog = _database;
                return new SqlConnection(connectionString.ToString());
            }
        }
    }
}
