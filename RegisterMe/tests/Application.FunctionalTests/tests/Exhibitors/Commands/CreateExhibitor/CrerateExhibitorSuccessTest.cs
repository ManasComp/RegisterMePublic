#region

using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Domain.Common;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitors.Commands.CreateExhibitor;

public class CreateExhibitorSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldCreateExhibitor(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };

        // Act
        Result<int> data = await SendAsync(createExhibitorCommand);

        // Assert
        data.IsSuccess.Should().BeTrue();
        ExhibitorAndUserDto exhibitor = await SendAsync(new GetExhibitorByIdQuery { ExhibitorId = data.Value });

        exhibitor.Should().NotBeNull();
        exhibitor.Organization.Should().Be(createExhibitorCommand.Exhibitor.Organization);
        exhibitor.MemberNumber.Should().Be(createExhibitorCommand.Exhibitor.MemberNumber);
        exhibitor.Country.Should().Be(createExhibitorCommand.Exhibitor.Country);
        exhibitor.City.Should().Be(createExhibitorCommand.Exhibitor.City);
        exhibitor.Street.Should().Be(createExhibitorCommand.Exhibitor.Street);
        exhibitor.HouseNumber.Should().Be(createExhibitorCommand.Exhibitor.HouseNumber);
        exhibitor.ZipCode.Should().Be(createExhibitorCommand.Exhibitor.ZipCode);
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldFailCreateExhibitorForExistingExhibitor(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };

        // Act
        Result<int> data = await SendAsync(createExhibitorCommand);
        data.IsSuccess.Should().BeTrue();
        Result<int> data1 = await SendAsync(createExhibitorCommand);

        // Assert
        data1.IsSuccess.Should().BeFalse();
    }
}
