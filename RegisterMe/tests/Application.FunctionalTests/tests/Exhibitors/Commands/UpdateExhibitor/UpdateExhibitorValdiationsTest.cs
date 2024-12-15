#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.Exhibitors.Commands.UpdateExhibitor;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitors.Commands.UpdateExhibitor;

#region

using static Testing;

#endregion

public class UpdateExhibitorExhibitionValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(null, null, null, null, null, null, null, null, null, null, null)]
    public async Task ShouldUpdateExhibitor(string? userId, string? city, string? country,
        string? houseNumber, string? organization, string? memberNumber, string? street, string? zipcode,
        bool? isPartOfCsch, bool? isPartOfFife, string? emailToOrganization)
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };
        await SendAsync(createExhibitorCommand);

        UpdateExhibitorCommand updateExhibitorCommand = new()
        {
            Exhibitor = new UpsertExhibitorDto
            {
                City = city ?? "Ci1ty",
                Country = country ?? "CZ",
                HouseNumber = houseNumber ?? "Ho1useNumber",
                MemberNumber = memberNumber ?? "Memb1erNumber",
                Organization = organization ?? "Organi1zation",
                Street = street ?? "Stre1et",
                ZipCode = zipcode ?? "Zip1Code",
                IsPartOfCsch = isPartOfCsch ?? true,
                IsPartOfFife = isPartOfFife ?? true,
                EmailToOrganization = emailToOrganization ?? "Email@organization.com"
            },
            AspNetUserId = userId ?? id
        };


        // Act
        Func<Task> act = async () => await SendAsync(updateExhibitorCommand);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }


    [Test]
    [TestCase("", null, null, null, null, null, null, null, null, null, null)]
    [TestCase(null, "", null, null, null, null, null, null, null, null, null)]
    [TestCase(null, null, "", null, null, null, null, null, null, null, null)]
    [TestCase(null, null, null, "", null, null, null, null, null, null, null)]
    [TestCase(null, null, null, null, "", null, null, null, null, null, null)]
    [TestCase(null, null, null, null, null, "", null, null, null, null, null)]
    [TestCase(null, null, null, null, null, null, "", null, null, null, null)]
    [TestCase(null, null, null, null, null, null, null, "", null, null, null)]
    [TestCase(null, null, null, null, null, null, null, "", null, null, "")]
    [TestCase(null, null, "CZK", null, null, null, null, null, null, null, "")]
    [TestCase(null, null, "Česko", null, null, null, null, null, null, null, "")]
    public async Task ShouldFailUpdateExhibitor(string? userId, string? city, string? country,
        string? houseNumber, string? organization, string? memberNumber, string? street, string? zipcode,
        bool? isPartOfCsch, bool? isPartOfFife, string? emailToOrganization)
    {
        // Arrange
        string id = await RunAsOndrejAsync();
        CreateExhibitorCommand createExhibitorCommand = new()
        {
            UserId = id, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };
        await SendAsync(createExhibitorCommand);

        UpdateExhibitorCommand updateExhibitorCommand = new()
        {
            Exhibitor = new UpsertExhibitorDto
            {
                City = city ?? "Ci1ty",
                Country = country ?? "CZ",
                HouseNumber = houseNumber ?? "Ho1useNumber",
                MemberNumber = memberNumber ?? "Memb1erNumber",
                Organization = organization ?? "Organi1zation",
                Street = street ?? "Stre1et",
                ZipCode = zipcode ?? "Zip1Code",
                IsPartOfCsch = isPartOfCsch ?? true,
                IsPartOfFife = isPartOfFife ?? true,
                EmailToOrganization = emailToOrganization ?? "Email@organization.com"
            },
            AspNetUserId = userId ?? id
        };

        // Act
        Func<Task> act = async () => await SendAsync(updateExhibitorCommand);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
