#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CancelExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CancelExhibition;

#region

using static Testing;

#endregion

public class CancelExhibitionValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(-1)]
    [TestCase(0)]
    public async Task ShouldNotAllowToCancelRandomExhibitions(int id)
    {
        // Arrange
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        });
        await RunAsOndrejAsync();

        // Act
        Func<Task> act = async () => await SendAsync(new CancelExhibitionCommand { ExhibitionId = id });
        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
