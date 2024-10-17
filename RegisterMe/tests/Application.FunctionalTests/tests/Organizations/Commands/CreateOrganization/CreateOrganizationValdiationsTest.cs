#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Dtos;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Organizations.Commands.CreateOrganization;

#region

using static Testing;

#endregion

public class CreateOrganizationValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(null, null, null, null, null, null, null)]
    public async Task ShouldCreateOrganization(string? name, string? email, string? ico, string? telephoneNumber,
        string? website, string? address, string? adminId)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationDto defaultOrg = OrganizationDataGenerator.GetOrganizationDto1(user);
        CreateOrganizationDto organizationDto = new()
        {
            Name = name ?? defaultOrg.Name,
            Email = email ?? defaultOrg.Email,
            Ico = ico ?? defaultOrg.Ico,
            TelNumber = telephoneNumber ?? defaultOrg.TelNumber,
            Website = website ?? defaultOrg.Website,
            Address = address ?? defaultOrg.Address,
            AdminId = adminId ?? user
        };
        CreateOrganizationCommand createOrganizationCommand = new() { CreateOrganizationDto = organizationDto };

        // Act
        Func<Task> act = async () => await SendAsync(createOrganizationCommand);

        // Assert
        await act.Should().NotThrowAsync();
    }


    [Test]
    [TestCase("", null, null, null, null, null, null)]
    [TestCase(null, "", null, null, null, null, null)]
    [TestCase(null, null, "", null, null, null, null)]
    [TestCase(null, null, null, "", null, null, null)]
    [TestCase(null, null, null, null, "", null, null)]
    [TestCase(null, null, null, null, null, "", null)]
    [TestCase(null, null, null, null, null, null, "")]
    public async Task ShouldFailCreateOrganization(string? name, string? email, string? ico, string? telephoneNumber,
        string? website, string? address, string? adminId)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationDto defaultOrg = OrganizationDataGenerator.GetOrganizationDto1(user);
        CreateOrganizationDto organizationDto = new()
        {
            Name = name ?? defaultOrg.Name,
            Email = email ?? defaultOrg.Email,
            Ico = ico ?? defaultOrg.Ico,
            TelNumber = telephoneNumber ?? defaultOrg.TelNumber,
            Website = website ?? defaultOrg.Website,
            Address = address ?? defaultOrg.Address,
            AdminId = adminId ?? user
        };
        CreateOrganizationCommand createOrganizationCommand = new() { CreateOrganizationDto = organizationDto };

        // Act
        Func<Task> act = async () => await SendAsync(createOrganizationCommand);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
