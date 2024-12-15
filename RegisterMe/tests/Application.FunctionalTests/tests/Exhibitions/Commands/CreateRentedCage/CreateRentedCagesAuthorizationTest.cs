#region

using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreateRentedCage;

#region

using static Testing;

#endregion

public class CreateRentedCagesAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    public async Task ShouldCreateRentedCages(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;
        Result<int> exhibitionId = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId.Value });

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 60,
                Length = 60,
                Width = 60,
                RentedCageTypes =
                [
                    RentedType.Single,
                    RentedType.Double
                ],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        // Assert
        await act.Should().NotThrowAsync();
    }


    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldFailCreateRentedCages(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;
        Result<int> exhibitionId = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId.Value });
        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 60,
                Length = 60,
                Width = 60,
                RentedCageTypes =
                [
                    RentedType.Single,
                    RentedType.Double
                ],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
