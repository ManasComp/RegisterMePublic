#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.BalancePayment;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.BalancePayment;

public class BalancePaymentValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase("", null, null)]
    [TestCase(" ", null, null)]
    [TestCase(null, 0, null)]
    [TestCase(null, -1, null)]
    [TestCase(null, null, "")]
    [TestCase(null, null, " ")]
    public async Task ShouldFailBalancePayment(string? webAddress, int? registrationToExhibitionId,
        string? rootPath)
    {
        await RunAsAdministratorAsync();
        Func<Task> act = async () =>
            await SendAsync(new BalancePaymentCommand
            {
                WebAddress = webAddress ?? "www.kocky.cz",
                RootPath = rootPath ?? "/var/www",
                RegistrationToExhibitionId = registrationToExhibitionId ?? 1
            });

        await act.Should().ThrowAsync<ValidationException>();
    }
}
