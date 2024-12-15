#region

using RegisterMe.Application.FunctionalTests.TestDatabases;

#endregion

namespace RegisterMe.Application.FunctionalTests;

public static class TestDatabaseFactory
{
    public static async Task<ITestDatabase> CreatePostgresAsync()
    {
        PostgresTestDatabase database = new();

        await database.InitialiseAsync();

        return database;
    }

    public static async Task<ITestDatabase> CreateSqlServerAsync()
    {
        SqlServerTestDatabase database = new();

        await database.InitialiseAsync();

        return database;
    }

    public static async Task<ITestDatabase> CreateDockerPostgresServerAsync()
    {
        DockerPostgresTestDatabase database = new();

        await database.InitialiseAsync();

        return database;
    }

    public static async Task<ITestDatabase> CreateDockerSqlServerAsync()
    {
        DockerSqlServerTestDatabase database = new();

        await database.InitialiseAsync();

        return database;
    }
}
