#region

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RegisterMe.Infrastructure.Data;
using Respawn;
using Testcontainers.PostgreSql;

#endregion

// Use Npgsql for PostgreSQL

namespace RegisterMe.Application.FunctionalTests.TestDatabases;

public class DockerPostgresTestDatabase : ITestDatabase
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    private NpgsqlConnection _connection = null!; // Use NpgsqlConnection

    private Respawner _respawner = null!;

    public async Task InitialiseAsync()
    {
        await _postgres.StartAsync();
        string? connectionstring = _postgres.GetConnectionString();

        // ...Context setup
        _connection = new NpgsqlConnection(connectionstring); // Use NpgsqlConnection
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionstring, x => x.MigrationsAssembly("DAL.Migrations.Postgres"))
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
        await _postgres.DisposeAsync().AsTask();
    }
}
