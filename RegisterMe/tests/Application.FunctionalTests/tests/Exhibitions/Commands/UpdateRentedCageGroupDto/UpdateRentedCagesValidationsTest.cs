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

public class UpdateRentedCagesPricesValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private List<RentedType> GetRentedCages(RentedCageEnum rentedCageEnum)
    {
        return rentedCageEnum switch
        {
            RentedCageEnum.Empty => [],
            RentedCageEnum.Single => [RentedType.Single],
            RentedCageEnum.Double => [RentedType.Double],
            RentedCageEnum.SingleAndDouble => [RentedType.Single, RentedType.Double],
            _ => throw new ArgumentOutOfRangeException(nameof(rentedCageEnum), rentedCageEnum, null)
        };
    }

    private List<int> GetExhibitionDays(ExhibitionDaysEnum exhibitionDaysEnum)
    {
        return exhibitionDaysEnum switch
        {
            ExhibitionDaysEnum.Empty => [],
            ExhibitionDaysEnum.Negative => [-1],
            ExhibitionDaysEnum.Zero => [0],
            ExhibitionDaysEnum.RandomDays => [1, 2, 3, 4, 5],
            _ => throw new ArgumentOutOfRangeException(nameof(exhibitionDaysEnum), exhibitionDaysEnum, null)
        };
    }


    [Test]
    [TestCase(0, null, null, null, null, null)]
    [TestCase(-1, null, null, null, null, null)]
    [TestCase(null, RentedCageEnum.Empty, null, null, null, null)]
    [TestCase(null, null, -1, null, null, null)]
    [TestCase(null, null, 0, null, null, null)]
    [TestCase(null, null, 5, null, null, null)]
    [TestCase(null, null, null, -1, null, null)]
    [TestCase(null, null, null, 0, null, null)]
    [TestCase(null, null, null, 5, null, null)]
    [TestCase(null, null, null, null, -1, null)]
    [TestCase(null, null, null, null, 0, null)]
    [TestCase(null, null, null, null, 5, null)]
    [TestCase(null, null, null, null, null, ExhibitionDaysEnum.Empty)]
    [TestCase(null, null, null, null, null, ExhibitionDaysEnum.Negative)]
    [TestCase(null, null, null, null, null, ExhibitionDaysEnum.Zero)]
    public async Task ShouldFailValidations(int? count, RentedCageEnum? rentedCage, int? length, int? width,
        int? height, ExhibitionDaysEnum? exhibitionDaysE)
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

        List<RentedType> cages = GetRentedCages(rentedCage ?? RentedCageEnum.Single);
        List<int> exhibitionDaysIds = exhibitionDaysE != null
            ? GetExhibitionDays(exhibitionDaysE.Value)
            : exhibitionDays.Select(x => x.Id).ToList();

        Result<string> id = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 60,
                Length = 60,
                Width = 60,
                RentedCageTypes = [RentedType.Single],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        // Act
        Func<Task> act = async () => await SendAsync(new UpdateRentedCageGroupCommand
        {
            CagesId = id.Value,
            RentedCage = new CreateRentedRentedCageDto
            {
                Count = count ?? 10,
                Height = height ?? 60,
                Length = length ?? 60,
                Width = width ?? 60,
                RentedCageTypes = cages,
                ExhibitionDaysId = exhibitionDaysIds
            }
        });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }


    [Test]
    [TestCase(null, null, null, null, null, ExhibitionDaysEnum.RandomDays)]
    public async Task ShouldThrowValidations(int? count, RentedCageEnum? rentedCage, int? length, int? width,
        int? height, ExhibitionDaysEnum? exhibitionDaysE)
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

        List<RentedType> cages = GetRentedCages(rentedCage ?? RentedCageEnum.Single);
        List<int> exhibitionDaysIds = exhibitionDaysE != null
            ? GetExhibitionDays(exhibitionDaysE.Value)
            : exhibitionDays.Select(x => x.Id).ToList();

        Result<string> id = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 60,
                Length = 60,
                Width = 60,
                RentedCageTypes = [RentedType.Single],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        // Act
        Func<Task> act = async () => await SendAsync(new UpdateRentedCageGroupCommand
        {
            CagesId = id.Value,
            RentedCage = new CreateRentedRentedCageDto
            {
                Count = count ?? 10,
                Height = height ?? 60,
                Length = length ?? 60,
                Width = width ?? 60,
                RentedCageTypes = cages,
                ExhibitionDaysId = exhibitionDaysIds
            }
        });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    [TestCase(null, RentedCageEnum.SingleAndDouble, 70, 70, 70, null)]
    [TestCase(null, RentedCageEnum.Double, 70, 70, 70, null)]
    public async Task ShouldResultFailValidations(int? count, RentedCageEnum? rentedCage, int? length,
        int? width,
        int? height, ExhibitionDaysEnum? exhibitionDaysE)
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

        List<RentedType> cages = GetRentedCages(rentedCage ?? RentedCageEnum.Single);
        List<int> exhibitionDaysIds = exhibitionDaysE != null
            ? GetExhibitionDays(exhibitionDaysE.Value)
            : exhibitionDays.Select(x => x.Id).ToList();

        Result<string> id = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 60,
                Length = 60,
                Width = 60,
                RentedCageTypes = [RentedType.Single],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        // Act
        Result<string> result = await SendAsync(new UpdateRentedCageGroupCommand
        {
            CagesId = id.Value,
            RentedCage = new CreateRentedRentedCageDto
            {
                Count = count ?? 10,
                Height = height ?? 60,
                Length = length ?? 60,
                Width = width ?? 60,
                RentedCageTypes = cages,
                ExhibitionDaysId = exhibitionDaysIds
            }
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Test]
    [TestCase(null, null, null, null, null, null)]
    [TestCase(null, RentedCageEnum.SingleAndDouble, 120, 120, 120, null)]
    [TestCase(null, RentedCageEnum.Double, 120, 120, 120, null)]
    public async Task ShouldResultTrueValidations(int? count, RentedCageEnum? rentedCage, int? length, int? width,
        int? height, ExhibitionDaysEnum? exhibitionDaysE)
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

        List<RentedType> cages = GetRentedCages(rentedCage ?? RentedCageEnum.Single);
        List<int> exhibitionDaysIds = exhibitionDaysE != null
            ? GetExhibitionDays(exhibitionDaysE.Value)
            : exhibitionDays.Select(x => x.Id).ToList();

        Result<string> id = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 60,
                Length = 60,
                Width = 60,
                RentedCageTypes = [RentedType.Single],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        // Act
        Result<string> result = await SendAsync(new UpdateRentedCageGroupCommand
        {
            CagesId = id.Value,
            RentedCage = new CreateRentedRentedCageDto
            {
                Count = count ?? 10,
                Height = height ?? 60,
                Length = length ?? 60,
                Width = width ?? 60,
                RentedCageTypes = cages,
                ExhibitionDaysId = exhibitionDaysIds
            }
        });

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
