#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.ChangeAdvertisement;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.ChangeAdvertisement;

public class ChangeAdvertisementValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(0, null)]
    [TestCase(-1, null)]
    [TestCase(null, 0)]
    [TestCase(null, -1)]
    public async Task ShouldCreateRegistrationToExhibition(int? advertisementId, int? registrationToExhibitionId)
    {
        // Arrange
        await RunAsAdministratorAsync();

        // Act
        Func<Task> act = async () => await SendAsync(new ChangeAdvertisementsCommand
        {
            AdvertisementId = advertisementId ?? 1, RegistrationToExhibitionId = registrationToExhibitionId ?? 1
        });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
