#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.DeleteRentedCages;
using RegisterMe.Application.FunctionalTests.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.DeleteRentedCages;

#region

using static Testing;

#endregion

public class DeleteRentedCagesPricesValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase("1")]
    public async Task ShouldPassValidation(string id)
    {
        // Arrange

        // Act
        Func<Task> validations = async () => await SendAsync(new DeleteRentedCagesCommand { CagesId = id });

        // Assert
        await validations.Should().NotThrowAsync<ValidationException>();
    }


    [Test]
    [TestCase("")]
    [TestCase("d")]
    [TestCase("1;2")]
    public async Task ShouldFailValidation(string id)
    {
        // Arrange

        // Act
        Func<Task> validations = async () => await SendAsync(new DeleteRentedCagesCommand { CagesId = id });

        // Assert
        await validations.Should().ThrowAsync<ValidationException>();
    }
}
