#region

using RegisterMe.Application.FunctionalTests.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests;

#region

using static Testing;

#endregion

// [TestFixture(DatabaseTypes.DockerSqlServer)]
// [TestFixture(DatabaseTypes.Postgres)]
[TestFixture(DatabaseTypes.DockerPostgres)]
public abstract class BaseTestFixture
{
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
    }

    protected BaseTestFixture(DatabaseTypes id)
    {
        SetDatabaseType(id);
        if (!WasRun)
        {
            RunBeforeAnyTests().Wait();
        }
    }
}
