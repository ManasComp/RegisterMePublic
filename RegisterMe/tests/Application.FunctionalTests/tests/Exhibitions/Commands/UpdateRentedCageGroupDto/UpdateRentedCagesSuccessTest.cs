#region

using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Cages.Queries.GetRentedCageGroupById;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;
using RegisterMe.Application.Exhibitions.Commands.UpdateRentedCageGroupDto;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetRentedCagesByExhibitionId;
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

public class UpdateRentedCagesPricesSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldUpdateRentedCages()
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

        Result<string> ids = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 120,
                Length = 120,
                Width = 120,
                RentedCageTypes =
                [
                    RentedType.Single,
                    RentedType.Double
                ],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        // Act
        Result<string> newIds = await SendAsync(new UpdateRentedCageGroupCommand
        {
            CagesId = ids.Value,
            RentedCage = new CreateRentedRentedCageDto
            {
                Count = 5,
                Height = 60,
                Length = 60,
                Width = 60,
                RentedCageTypes =
                [
                    RentedType.Single
                ],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        // Assert
        newIds.Value.Split(",").Length.Should().Be(5);

        Func<Task> act = async () => await SendAsync(new GetRentedCageGroupByIdQuery { CagesId = ids.Value });
        await act.Should().ThrowAsync<NotFoundException>();
        BriefCageDto cages = await SendAsync(new GetRentedCageGroupByIdQuery { CagesId = newIds.Value });
        cages.Ids.Should().Be(newIds.Value);
        cages.Count.Should().Be(5);
        cages.ExhibitionDays.Should()
            .BeEquivalentTo(exhibitionDays.Select(x => new SmallExhibitionDayDto { Date = x.Date, Id = x.Id }));
        cages.Height.Should().Be(60);
        cages.Length.Should().Be(60);
        cages.Width.Should().Be(60);
        cages.RentedTypes.Should().BeEquivalentTo(new List<RentedType> { RentedType.Single });

        List<BriefCageDto> cageGroups =
            await SendAsync(new GetRentedCagesByExhibitionIdQuery { ExhibitionId = exhibitionId.Value });
        cageGroups.Count.Should().Be(1);
        bool wasFound = cageGroups.Exists(group => group.Ids == cages.Ids &&
                                                   group.ExhibitionDays.OrderBy(x => x.Id)
                                                       .SequenceEqual(cages.ExhibitionDays.OrderBy(x => x.Id)) &&
                                                   group.Height == cages.Height && group.Length == cages.Length &&
                                                   group.Width == cages.Width &&
                                                   group.RentedTypes.OrderBy(x => x)
                                                       .SequenceEqual(cages.RentedTypes.OrderBy(x => x)));
        wasFound.Should().BeTrue();
    }

    [Test]
    public async Task ShouldFailUpdateRentedCagesForDifferentExhibition()
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
        Result<int> exhibitionId2 = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });
        List<ExhibitionDayDto> exhibitionDays1 =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId.Value });
        List<ExhibitionDayDto> exhibitionDays2 =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId2.Value });

        List<int> exhibitionDaysIds = exhibitionDays1.Select(x => x.Id).ToList();
        exhibitionDaysIds.AddRange(exhibitionDays2.Select(x => x.Id).ToList());
        Result<string> ids = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 120,
                Length = 120,
                Width = 120,
                RentedCageTypes =
                [
                    RentedType.Single,
                    RentedType.Double
                ],
                ExhibitionDaysId = exhibitionDays1.Select(x => x.Id).ToList()
            }
        });

        ids.IsSuccess.Should().BeTrue();

        // Act
        Result<string> newIds = await SendAsync(new UpdateRentedCageGroupCommand
        {
            CagesId = ids.Value,
            RentedCage = new CreateRentedRentedCageDto
            {
                Count = 5,
                Height = 60,
                Length = 60,
                Width = 60,
                RentedCageTypes =
                [
                    RentedType.Single
                ],
                ExhibitionDaysId = exhibitionDaysIds
            }
        });

        // Assert
        newIds.IsSuccess.Should().BeFalse();
    }

    [Test]
    public async Task ShouldFailUpdateCagesForRandomExhibitionDayIds()
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

        List<ExhibitionDayDto> exhibitionDays1 =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId.Value });
        Result<string> ids = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 120,
                Length = 120,
                Width = 120,
                RentedCageTypes =
                [
                    RentedType.Single,
                    RentedType.Double
                ],
                ExhibitionDaysId = exhibitionDays1.Select(x => x.Id).ToList()
            }
        });

        ids.IsSuccess.Should().BeTrue();

        // Act
        Func<Task> newIds = async () => await SendAsync(new UpdateRentedCageGroupCommand
        {
            CagesId = ids.Value,
            RentedCage = new CreateRentedRentedCageDto
            {
                Count = 5,
                Height = 60,
                Length = 60,
                Width = 60,
                RentedCageTypes =
                [
                    RentedType.Single
                ],
                ExhibitionDaysId = [1, 3, 5]
            }
        });

        // Assert
        await newIds.Should().ThrowAsync<NotFoundException>();
    }
}
