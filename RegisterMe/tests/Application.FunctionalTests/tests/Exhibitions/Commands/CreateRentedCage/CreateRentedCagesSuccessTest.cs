#region

using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Cages.Queries.GetRentedCageGroupById;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetRentedCagesByExhibitionId;
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

public class CreateRentedCagesPricesSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldCreateRentedCages()
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

        // Act
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

        // Assert
        ids.Value.Split(",").Length.Should().Be(10);
        BriefCageDto cages = await SendAsync(new GetRentedCageGroupByIdQuery { CagesId = ids.Value });
        cages.Ids.Should().Be(ids.Value);
        cages.Count.Should().Be(10);
        cages.ExhibitionDays.Should()
            .BeEquivalentTo(exhibitionDays.Select(x => new SmallExhibitionDayDto { Date = x.Date, Id = x.Id }));
        cages.Height.Should().Be(120);
        cages.Length.Should().Be(120);
        cages.Width.Should().Be(120);
        cages.RentedTypes.Should().BeEquivalentTo(new List<RentedType> { RentedType.Single, RentedType.Double });

        List<BriefCageDto> cageGroups =
            await SendAsync(new GetRentedCagesByExhibitionIdQuery { ExhibitionId = exhibitionId.Value });
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
    public async Task ShouldNotCreatePricesForDifferentExhibition()
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

        List<int> daysIds = exhibitionDays1.Select(x => x.Id).ToList();
        daysIds.AddRange(exhibitionDays2.Select(x => x.Id).ToList());

        // Act
        Result<string> ids = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
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
                ExhibitionDaysId = daysIds
            }
        });

        // Assert
        ids.IsSuccess.Should().BeFalse();
    }

    [Test]
    public async Task ShouldNotCreatePricesForRandomExhibitionDayIds()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;
        await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });

        // Act
        Func<Task> ids = async () => await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
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
                ExhibitionDaysId = [1, 5, 7]
            }
        });

        // Assert
        await ids.Should().ThrowAsync<NotFoundException>();
    }
}
