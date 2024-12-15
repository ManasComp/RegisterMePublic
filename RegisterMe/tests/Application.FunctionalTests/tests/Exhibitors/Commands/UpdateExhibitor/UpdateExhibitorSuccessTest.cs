#region

using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.Exhibitors.Commands.UpdateExhibitor;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Domain.Common;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitors.Commands.UpdateExhibitor;

public class UpdateExhibitorSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldUpdateExhibitor()
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };

        Result<int> data = await SendAsync(createExhibitorCommand);

        UpdateExhibitorCommand updateExhibitorCommands = new()
        {
            Exhibitor = ExhibitorDataGenerator.GetExhibitorDto2(), AspNetUserId = id
        };

        // Act
        await SendAsync(updateExhibitorCommands);

        // Assert
        data.IsSuccess.Should().BeTrue();
        ExhibitorAndUserDto exhibitor = await SendAsync(new GetExhibitorByIdQuery { ExhibitorId = data.Value });

        exhibitor.Should().NotBeNull();
        exhibitor.Organization.Should().Be(updateExhibitorCommands.Exhibitor.Organization);
        exhibitor.MemberNumber.Should().Be(updateExhibitorCommands.Exhibitor.MemberNumber);
        exhibitor.Country.Should().Be(updateExhibitorCommands.Exhibitor.Country);
        exhibitor.City.Should().Be(updateExhibitorCommands.Exhibitor.City);
        exhibitor.Street.Should().Be(updateExhibitorCommands.Exhibitor.Street);
        exhibitor.HouseNumber.Should().Be(updateExhibitorCommands.Exhibitor.HouseNumber);
        exhibitor.ZipCode.Should().Be(updateExhibitorCommands.Exhibitor.ZipCode);
    }
}
