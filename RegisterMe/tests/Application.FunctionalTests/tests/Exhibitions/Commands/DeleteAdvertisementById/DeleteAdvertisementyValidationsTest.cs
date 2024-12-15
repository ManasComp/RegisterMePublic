#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.DeleteAdvertisementById;
using RegisterMe.Application.FunctionalTests.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.DeleteAdvertisementById;

#region

using static Testing;

#endregion

public class DeleteAdvertisementValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(-1)]
    [TestCase(0)]
    public async Task ShouldFailValidations(int advertisementId)
    {
        // Arrange

        // Act
        Func<Task> act = async () =>
            await SendAsync(new DeleteAdvertisementCommand { AdvertisementId = advertisementId });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    public async Task ShouldNotFailValidations(int advertisementId)
    {
        // Arrange

        // Act
        Func<Task> act = async () => await SendAsync(new DeleteAdvertisementCommand
        {
            AdvertisementId = advertisementId
        });

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}
