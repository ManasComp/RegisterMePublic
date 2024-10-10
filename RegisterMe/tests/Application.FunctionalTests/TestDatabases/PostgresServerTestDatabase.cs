#region

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using RegisterMe.Infrastructure.Data;
using Respawn;

#endregion

// Use Npgsql for PostgreSQL

namespace RegisterMe.Application.FunctionalTests.TestDatabases;

public class PostgresTestDatabase : ITestDatabase
{
    private readonly string _connectionString;
    private NpgsqlConnection _connection = null!; // Use NpgsqlConnection
    private Respawner _respawner = null!;

    public PostgresTestDatabase()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        string? connectionString = configuration.GetConnectionString("LocalPostgresServerConnection");
        // Make sure your appsettings.json has the correct connection string for PostgreSQL

        Guard.Against.Null(connectionString, nameof(connectionString));

        _connectionString = connectionString;
    }

    public async Task InitialiseAsync()
    {
        // ...Context setup
        _connection = new NpgsqlConnection(_connectionString); // Use NpgsqlConnection
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_connectionString, x => x.MigrationsAssembly("DAL.Migrations.Postgres"))
            .Options;
        ApplicationDbContext context = new(options);
        await context.Database.MigrateAsync();

        RespawnerOptions respawnerOptions = new()
        {
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory"],
            DbAdapter = DbAdapter.Postgres
        };
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, respawnerOptions);
    }


    public DbConnection GetConnection()
    {
        return _connection;
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(_connection);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _connection.CloseAsync();
    }
}
