#region

using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RegisterMe.Infrastructure.Data;
using Respawn;

#endregion

namespace RegisterMe.Application.FunctionalTests.TestDatabases;

public class SqlServerTestDatabase : ITestDatabase
{
    private readonly string _connectionString;
    private SqlConnection _connection = null!;
    private Respawner _respawner = null!;

    public SqlServerTestDatabase()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        string? connectionString = configuration.GetConnectionString("LocalSqlServerConnection");

        Guard.Against.Null(connectionString);

        _connectionString = connectionString;
    }

    public async Task InitialiseAsync()
    {
        _connection = new SqlConnection(_connectionString);

        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_connectionString, x => x.MigrationsAssembly("DAL.Migrations.SqlServer"))
            .Options;

        ApplicationDbContext context = new(options);

        await context.Database.MigrateAsync();

        _respawner = await Respawner.CreateAsync(_connectionString,
            new RespawnerOptions { TablesToIgnore = ["__EFMigrationsHistory"] });
    }

    public DbConnection GetConnection()
    {
        return _connection;
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(_connectionString);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
