#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.FinishOnlinePayment;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    FinishOnlinePayment;

public class FinishOnlinePaymentCommandValidationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(0, null, null)]
    [TestCase(-1, null, null)]
    [TestCase(null, "", null)]
    [TestCase(null, " ", null)]
    [TestCase(null, null, "")]
    [TestCase(null, null, " ")]
    public async Task ShouldFailFinishOnlinePayment(int? registrationToExhibitionId, string? webAddress,
        string? rootPath)
    {
        // Arrange
        await RunAsAdministratorAsync();

        // Act
        Func<Task> act = async () => await SendAsync(new FinishOnlinePaymentCommand
        {
            RegistrationToExhibitionId = registrationToExhibitionId ?? 1,
            WebAddress = webAddress ?? "https://www.kocky.cz/success",
            RootPath = rootPath ?? "https://www.kocky.cz/cancel"
        });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
