#region

using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;
using RegisterMe.Application.Exhibitions.Commands.UpdateRentedCageGroupDto;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.UpdateRentedCageGroupDto;

#region

using static Testing;

#endregion

public class UpdateRentedCagesAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    public async Task ShouldUpdateRentedCages(RunAsSpecificUser runAsSpecificUser)
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

        Result<string> cages = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 60,
                Length = 120,
                Width = 60,
                RentedCageTypes =
                [
                    RentedType.Single,
                    RentedType.Double
                ],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });
        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(new UpdateRentedCageGroupCommand
        {
            CagesId = cages.Value,
            RentedCage = new CreateRentedRentedCageDto
            {
                Count = 5,
                Height = 63,
                Length = 63,
                Width = 63,
                RentedCageTypes =
                [
                    RentedType.Single
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
    public async Task ShouldFailUpdateRentedCages(RunAsSpecificUser runAsSpecificUser)
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

        Result<string> cages = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 60,
                Length = 120,
                Width = 60,
                RentedCageTypes =
                [
                    RentedType.Single,
                    RentedType.Double
                ],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> act = async () => await SendAsync(new UpdateRentedCageGroupCommand
        {
            CagesId = cages.Value,
            RentedCage = new CreateRentedRentedCageDto
            {
                Count = 5,
                Height = 63,
                Length = 63,
                Width = 63,
                RentedCageTypes =
                [
                    RentedType.Single
                ],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
