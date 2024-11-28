#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.DeletePriceGroup;
using RegisterMe.Application.FunctionalTests.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.DeletePriceGroup;

#region

using static Testing;

#endregion

public class DeletePricesValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase("")]
    [TestCase("-1")]
    [TestCase("1,-1")]
    [TestCase("0")]
    [TestCase("1,1")]
    [TestCase("invalid")]
    public async Task ShouldFailValidations(string priceGroupIds)
    {
        // Arrange
        DeletePriceGroupCommand createPricesCommand = new() { PriceIds = priceGroupIds };

        // Act
        Func<Task> act = async () => await SendAsync(createPricesCommand);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    [TestCase("1")]
    [TestCase("1,2")]
    public async Task ShouldNotFailValidations(string priceGroupIds)
    {
        // Arrange
        DeletePriceGroupCommand createPricesCommand = new() { PriceIds = priceGroupIds };

        // Act
        Func<Task> act = async () => await SendAsync(createPricesCommand);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}
