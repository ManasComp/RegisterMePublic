#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.UpdatePriceGroup;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.ValueTypes;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdatePriceGroup;

#region

using static Testing;

#endregion

public class UpdatePricesValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase("-1")]
    [TestCase("0")]
    [TestCase("1,1,1,1")]
    [TestCase("l;l")]
    [TestCase("")]
    public async Task ShouldFailValidations(string pricgroupIds)
    {
        // Arrange
        await RunAsAdministratorAsync();
        UpdatePriceGroupCommand command = new()
        {
            GroupsIds = ["1", "2"],
            PriceDays = [new PriceDays { ExhibitionDayIds = [1, 5], Price = new MultiCurrencyPrice(100, 3) }],
            OriginalPricesId = pricgroupIds
        };

        // Act
        Func<Task> act = async () => await SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
