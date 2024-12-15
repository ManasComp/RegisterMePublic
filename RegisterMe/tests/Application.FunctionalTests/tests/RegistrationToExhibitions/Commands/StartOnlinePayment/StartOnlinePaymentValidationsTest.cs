#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.StartOnlinePayment;
using RegisterMe.Domain.Enums;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    StartOnlinePayment;

public class StartOnlinePaymentCommandValidationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(0, null, null)]
    [TestCase(-1, null, null)]
    [TestCase(null, "", null)]
    [TestCase(null, " ", null)]
    [TestCase(null, null, "")]
    [TestCase(null, null, " ")]
    public async Task ShouldFailStartOnlinePayment(int? registrationToExhibitionId, string? successUrl,
        string? cancelUrl)
    {
        // Arrange
        await RunAsAdministratorAsync();

        // Act
        Func<Task> act = async () => await SendAsync(new StartOnlinePaymentCommand
        {
            RegistrationToExhibitionId = registrationToExhibitionId ?? 1,
            SuccessUrl = successUrl ?? "https://www.kocky.cz/success",
            Currency = Currency.Czk,
            CancelUrl = cancelUrl ?? "https://www.kocky.cz/cancel"
        });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
