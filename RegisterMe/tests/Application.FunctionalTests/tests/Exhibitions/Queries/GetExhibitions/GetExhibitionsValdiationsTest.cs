#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Enums;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitions;
using RegisterMe.Application.FunctionalTests.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Queries.GetExhibitions;

#region

using static Testing;

#endregion

public class GetExhibitionsValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(2, 50, null, "", OrganizationPublishStatus.All, ExhibitionRegistrationStatus.All)]
    public async Task ShouldThrowExceptionWhenNotProvidingOrganizationId(int pageNumber, int pageSize,
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
        await act.Should().ThrowAsync<Exception>();
    }

    [Test]
    [TestCase(2, 0, null, "", OrganizationPublishStatus.All, ExhibitionRegistrationStatus.All)]
    [TestCase(0, 50, null, "", OrganizationPublishStatus.All, ExhibitionRegistrationStatus.All)]
    [TestCase(2, 50, 0, "", OrganizationPublishStatus.All, ExhibitionRegistrationStatus.All)]
    [TestCase(2, 50, null,
        "longlonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglong",
        OrganizationPublishStatus.All, ExhibitionRegistrationStatus.All)]
    [TestCase(200, 50, null, "", OrganizationPublishStatus.All, ExhibitionRegistrationStatus.All)]
    [TestCase(1, 454, null, "", OrganizationPublishStatus.All, ExhibitionRegistrationStatus.All)]
    public async Task ShouldFailValidations(int pageNumber, int pageSize, int? organizationId, string? searchString,
        OrganizationPublishStatus organizationPublishStatus, ExhibitionRegistrationStatus exhibitionStatus)
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
        await act.Should().ThrowAsync<ValidationException>();
    }
}
