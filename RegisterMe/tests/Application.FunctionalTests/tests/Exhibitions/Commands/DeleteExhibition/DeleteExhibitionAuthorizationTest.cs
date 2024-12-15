#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.DeleteExhibition;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.DeleteExhibition;

#region

using static Testing;

#endregion

public class DeleteExhibitionsAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldDeleteExhibition(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });

        Result<int> exhibition1Id = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        });

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () =>
            await SendAsync(new DeleteUnpublishedExhibitionCommand { ExhibitionId = exhibition1Id.Value });

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    public async Task ShouldFailDeleteExhibition(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });

        Result<int> exhibition1Id = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        });

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () =>
            await SendAsync(new DeleteUnpublishedExhibitionCommand { ExhibitionId = exhibition1Id.Value });

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
