#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Enums;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitions;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Queries.GetExhibitions;

#region

using static Testing;

#endregion

public class GetExhibitionsAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(1, 10, 1, "", OrganizationPublishStatus.NotPublished, ExhibitionRegistrationStatus.All)]
    [TestCase(1, 10, 1, "", OrganizationPublishStatus.All, ExhibitionRegistrationStatus.All)]
    public async Task ShouldFailAuthorizationForUserAndNotPublishedExhibitions(int pageNumber, int pageSize,
        int? organizationId, string? searchString, OrganizationPublishStatus organizationPublishStatus,
        ExhibitionRegistrationStatus exhibitionStatus)
    {
        // Arrange
        string user = await RunAsOndrejAsync();

        // Act
        GetExhibitionsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organizationId,
            UserId = user,
            SearchString = searchString,
            OrganizationPublishStatus = organizationPublishStatus,
            ExhibitionStatus = exhibitionStatus
        };
        Func<Task> act = async () => await SendAsync(query);

        // Assert

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    [TestCase(1, 10, 1, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    public async Task ShouldPassAuthorizationForUserAndHisData(int pageNumber, int pageSize, int? organizationId,
        string? searchString, OrganizationPublishStatus organizationPublishStatus,
        ExhibitionRegistrationStatus exhibitionStatus)
    {
        // Arrange
        string user = await RunAsOndrejAsync();

        // Act
        GetExhibitionsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organizationId,
            UserId = user,
            SearchString = searchString,
            OrganizationPublishStatus = organizationPublishStatus,
            ExhibitionStatus = exhibitionStatus
        };
        Func<Task> act = async () => await SendAsync(query);

        // Assert

        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(1, 10, 1, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    public async Task ShouldFailAuthorizationForUserAndOtherUserData(int pageNumber, int pageSize, int? organizationId,
        string? searchString, OrganizationPublishStatus organizationPublishStatus,
        ExhibitionRegistrationStatus exhibitionStatus)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        await RunAsSabrinaAsync();

        // Act
        GetExhibitionsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organizationId,
            UserId = user,
            SearchString = searchString,
            OrganizationPublishStatus = organizationPublishStatus,
            ExhibitionStatus = exhibitionStatus
        };
        Func<Task> act = async () => await SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    [TestCase(1, 10, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    [TestCase(1, 10, "", OrganizationPublishStatus.All, ExhibitionRegistrationStatus.All)]
    [TestCase(1, 10, "", OrganizationPublishStatus.NotPublished, ExhibitionRegistrationStatus.All)]
    public async Task ShouldPassAuthorizationForOrganizationAdmin(int pageNumber, int pageSize, string? searchString,
        OrganizationPublishStatus organizationPublishStatus, ExhibitionRegistrationStatus exhibitionStatus)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        int organizationId = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        })).Value;

        // Act
        GetExhibitionsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organizationId,
            UserId = user,
            SearchString = searchString,
            OrganizationPublishStatus = organizationPublishStatus,
            ExhibitionStatus = exhibitionStatus
        };
        Func<Task> act = async () => await SendAsync(query);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(1, 10, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    [TestCase(1, 10, "", OrganizationPublishStatus.All, ExhibitionRegistrationStatus.All)]
    [TestCase(1, 10, "", OrganizationPublishStatus.NotPublished, ExhibitionRegistrationStatus.All)]
    public async Task ShouldPassAuthorizationForSuperAdmin(int pageNumber, int pageSize, string? searchString,
        OrganizationPublishStatus organizationPublishStatus, ExhibitionRegistrationStatus exhibitionStatus)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        int organizationId = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        })).Value;
        await RunAsAdministratorAsync();


        // Act
        GetExhibitionsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organizationId,
            UserId = user,
            SearchString = searchString,
            OrganizationPublishStatus = organizationPublishStatus,
            ExhibitionStatus = exhibitionStatus
        };
        Func<Task> act = async () => await SendAsync(query);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(1, 5, "", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.Future)]
    [TestCase(4, 10, "test", OrganizationPublishStatus.Published, ExhibitionRegistrationStatus.All)]
    public async Task ShouldPassAuthorizationForAnonymous(int pageNumber, int pageSize, string? searchString,
        OrganizationPublishStatus organizationPublishStatus, ExhibitionRegistrationStatus exhibitionStatus)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        int organizationId = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        })).Value;
        RunAsAnonymousUser();

        // Act
        GetExhibitionsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organizationId,
            UserId = null,
            SearchString = searchString,
            OrganizationPublishStatus = organizationPublishStatus,
            ExhibitionStatus = exhibitionStatus
        };
        Func<Task> act = async () => await SendAsync(query);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
