#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.RequestDelayedPayment;
using RegisterMe.Domain.Enums;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    RequestDelayedPayment;

public class RequestDelayedPaymentCommandValidationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(0, null, null)]
    [TestCase(-1, null, null)]
    [TestCase(null, "", null)]
    [TestCase(null, " ", null)]
    [TestCase(null, null, "")]
    [TestCase(null, null, " ")]
    public async Task ShouldFailRequestDelayedPayment(int? registrationToExhibitionId, string? webAddress,
        string? rootPath)
    {
        // Arrange
        await RunAsAdministratorAsync();

        // Act
        Func<Task> act = async () => await SendAsync(new RequestDelayedPaymentCommand
        {
            RegistrationToExhibitionId = registrationToExhibitionId ?? 1,
            PaymentType = PaymentType.PayInPlaceByCache,
            Currency = Currency.Czk,
            WebAddress = webAddress ?? "www.kocky.cz",
            RootPath = rootPath ?? "/var/www"
        });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
