#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Commands.UpdateOrganization;
using RegisterMe.Application.Organizations.Dtos;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations.Commands.UpdateOrganization;

#region

using static Testing;

#endregion

public class UpdateOrganizationValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(null, null, null, null, null)]
    public async Task ShouldCreateExhibition(string? name, string? email, string? telephoneNumber, string? website,
        string? address)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationDto defaultOrg = OrganizationDataGenerator.GetOrganizationDto1(user);

        CreateOrganizationCommand createOrganizationCommand = new() { CreateOrganizationDto = defaultOrg };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;
        CreateOrganizationDto defaultOrg2 = OrganizationDataGenerator.GetOrganizationDto2(user);
        UpdateOrganizationDto organizationDto = new()
        {
            Name = name ?? defaultOrg2.Name,
            Email = email ?? defaultOrg2.Email,
            Id = organizationId,
            TelNumber = telephoneNumber ?? defaultOrg2.TelNumber,
            Website = website ?? defaultOrg2.Website,
            Address = address ?? defaultOrg2.Address
        };
        UpdateOrganizationCommand updateOrganizationCommand = new() { OrganizationDto = organizationDto };

        // Act
        Func<Task> act = async () => await SendAsync(updateOrganizationCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }


    [Test]
    [TestCase("", null, null, null, null)]
    [TestCase(null, "", null, null, null)]
    [TestCase(null, null, "", null, null)]
    [TestCase(null, null, null, "", null)]
    [TestCase(null, null, null, null, "")]
    public async Task ShouldFailCreateExhibition(string? name, string? email, string? telephoneNumber, string? website,
        string? address)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationDto defaultOrg = OrganizationDataGenerator.GetOrganizationDto1(user);

        CreateOrganizationCommand createOrganizationCommand = new() { CreateOrganizationDto = defaultOrg };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;
        CreateOrganizationDto defaultOrg2 = OrganizationDataGenerator.GetOrganizationDto2(user);
        UpdateOrganizationDto organizationDto = new()
        {
            Name = name ?? defaultOrg2.Name,
            Email = email ?? defaultOrg2.Email,
            Id = organizationId,
            TelNumber = telephoneNumber ?? defaultOrg2.TelNumber,
            Website = website ?? defaultOrg2.Website,
            Address = address ?? defaultOrg2.Address
        };
        UpdateOrganizationCommand updateOrganizationCommand = new() { OrganizationDto = organizationDto };

        // Act
        Func<Task> act = async () => await SendAsync(updateOrganizationCommand);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
