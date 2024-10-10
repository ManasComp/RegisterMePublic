#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitors.Commands.CreateExhibitor;

#region

using static Testing;

#endregion

public class CreateExhibitorValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(null, null, null, null, null, null, null, null)]
    public async Task ShouldCreateExhibitor(string? userId, string? city, string? country,
        string? houseNumber, string? organization, string? memberNumber, string? street, string? zipcode)
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = userId ?? id,
            Exhibitor = new UpsertExhibitorDto
            {
                City = city ?? "City",
                Country = country ?? "CZ",
                HouseNumber = houseNumber ?? "HouseNumber",
                MemberNumber = memberNumber ?? "MemberNumber",
                Organization = organization ?? "Organization",
                Street = street ?? "Street",
                ZipCode = zipcode ?? "ZipCode",
                IsPartOfCsch = true,
                EmailToOrganization = "emailToOrganization@example.com",
                IsPartOfFife = true
            }
        };

        // Act
        Result<int> data = await SendAsync(createExhibitorCommand);

        // Assert
        data.IsSuccess.Should().BeTrue();
    }


    [Test]
    [TestCase("", null, null, null, null, null, null, null)]
    public async Task ShouldFailCreateExhibitor(string? userId, string? city, string? country,
        string? houseNumber, string? organization, string? memberNumber, string? street, string? zipcode)
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = userId ?? id,
            Exhibitor = new UpsertExhibitorDto
            {
                City = city ?? "City",
                Country = country ?? "CZ",
                HouseNumber = houseNumber ?? "HouseNumber",
                MemberNumber = memberNumber ?? "MemberNumber",
                Organization = organization ?? "Organization",
                Street = street ?? "Street",
                ZipCode = zipcode ?? "ZipCode",
                IsPartOfCsch = true,
                EmailToOrganization = "emailToOrganization@example.com",
                IsPartOfFife = true
            }
        };

        // Act
        Func<Task> act = async () => await SendAsync(createExhibitorCommand);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
