#region

using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorById;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Users.Command;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Users.Commands.DeletePersonalData;

public class DeletePersonalDataSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldDeletePersonalData()
    {
        string id = await RunAsOndrejAsync();
        // Arrange
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };

        // Act

        Result<int> data = await SendAsync(createExhibitorCommand);
        ApplicationUser? originalUser = await FindAsync<ApplicationUser>(id);
        string? originalUserPasswordHash = originalUser?.PasswordHash;

        Result result = await SendAsync(new DeletePersonalDataCommand { UserId = id });
        result.IsSuccess.Should().BeTrue();
        // Assert

        data.IsSuccess.Should().BeTrue();
        ExhibitorAndUserDto exhibitor = await SendAsync(new GetExhibitorByIdQuery { ExhibitorId = data.Value });

        exhibitor.Should().NotBeNull();
        exhibitor.Organization.Should().Be("Deleted");
        exhibitor.MemberNumber.Should().Be("Deleted");
        exhibitor.Country.Should().Be("Deleted");
        exhibitor.City.Should().Be("Deleted");
        exhibitor.Street.Should().Be("Deleted");
        exhibitor.HouseNumber.Should().Be("Deleted");
        exhibitor.ZipCode.Should().Be("Deleted");
        exhibitor.Email.Should().Be("Deleted");
        exhibitor.FirstName.Should().Be("Deleted");
        exhibitor.LastName.Should().Be("Deleted");
        exhibitor.PhoneNumber.Should().Be("Deleted");
        exhibitor.FirstName.Should().Be("Deleted");
        exhibitor.LastName.Should().Be("Deleted");
        exhibitor.Email.Should().Be("Deleted");
        ApplicationUser? updatedUser = await FindAsync<ApplicationUser>(id);
        updatedUser?.UserName.Should().Contain("Deleted");
        updatedUser?.PasswordHash.Should().NotBe(originalUserPasswordHash);
    }
}
