#region

using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Cages.Dtos.Combination;
using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Cages.Queries.GetAvailableRentedCageTypes;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;
using RegisterMe.Application.Exhibitions.Commands.PublishExhibition;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionCagesInfo;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.RegistrationToExhibition.Commands.CreateRegistrationToExhibition;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Cages.Queries.GetAvailableRentedCageTypes;

#region

using static Testing;

#endregion

public class GetAvailableCages(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldReturnAvailableCagesForNoRegisteredCats()
    {
        // Arrange
        string userId = await RunAsAdministratorAsync();
        Tuple<BriefExhibitionDto, int> tuple = await CreateRegistrationToExhibition(userId);

        GetExhibitionCagesInfoQuery command = new() { ExhibitionId = tuple.Item1.Id };

        // Act
        CagesPerDayDto availableCages = await SendAsync(new GetAvailableRentedCageTypesQuery
        {
            ExhibitionDayId = tuple.Item1.Id,
            RegistrationToExhibitionId = tuple.Item2,
            IsLitter = false,
            CatRegistrationId = null
        });
        List<ExhibitionCagesInfo> exhibitionData = await SendAsync(command);

        // Assert
        availableCages.Should().NotBeNull();
        availableCages.ExhibitorsCages.Should().BeEmpty();
        availableCages.AvailableAlreadyRentedCagesByExhibitor.Should().BeEmpty();
        availableCages.AvailableCagesForRent.Should().NotBeEmpty();
        availableCages.AvailableCagesForRent.Should().HaveCount(2);
        availableCages.AvailableCagesForRent[0].RentedTypeId.Should()
            .Be(new RentedCageGroup(60, 120, 60, RentedType.Single, RentingType.RentedWithZeroOtherCats));
        availableCages.AvailableCagesForRent[1].RentedTypeId.Should()
            .Be(new RentedCageGroup(60, 120, 60, RentedType.Double, RentingType.RentedWithZeroOtherCats));

        exhibitionData.Should().NotBeNull();
        exhibitionData.Count.Should().Be(2);

        exhibitionData[0].Date.Should().Be(DateOnly.FromDateTime(DateTime.Now.AddDays(2)).ToString("dd/MM/yyyy"));
        exhibitionData[0].NumberOfPersonCages.Should().Be(0);
        exhibitionData[0].CageGroupInfos.Count.Should().Be(1);
        exhibitionData[0].CageGroupInfos[0].NumberOfUsedCages.Should().Be(new CagesRec
        {
            NumberOfDoubleCages = 0, NumberOfSingleCages = 0
        });
        exhibitionData[0].CageGroupInfos[0].TotalNumberOfRentedCages.Should().Be(1);

        exhibitionData[1].Date.Should().Be(DateOnly.FromDateTime(DateTime.Now.AddDays(3)).ToString("dd/MM/yyyy"));
        exhibitionData[1].NumberOfPersonCages.Should().Be(0);
        exhibitionData[1].CageGroupInfos.Count.Should().Be(1);
        exhibitionData[1].CageGroupInfos[0].NumberOfUsedCages.Should().Be(new CagesRec
        {
            NumberOfDoubleCages = 0, NumberOfSingleCages = 0
        });
        exhibitionData[1].CageGroupInfos[0].TotalNumberOfRentedCages.Should().Be(1);
    }

    [Test]
    public async Task ShouldReturnAvailableCagesForOneRegisteredRentedSingleCageCat()
    {
        // Arrange
        string userId = await RunAsAdministratorAsync();
        Tuple<BriefExhibitionDto, int> tuple = await CreateRegistrationToExhibition(userId);
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = tuple.Item1.Id });

        Result<int> result = await SendAsync(new CreateCatRegistrationCommand
        {
            CatRegistration = CatRegistrationDataGenerator.WithDays(tuple.Item2, exhibitionDays,
                new RentedCageGroup(60, 120, 60, RentedType.Single, RentingType.RentedWithZeroOtherCats), null)
        });

        result.IsSuccess.Should().Be(true);


        // Act
        GetExhibitionCagesInfoQuery command = new() { ExhibitionId = tuple.Item1.Id };
        CagesPerDayDto availableCages = await SendAsync(new GetAvailableRentedCageTypesQuery
        {
            ExhibitionDayId = exhibitionDays.First().Id,
            RegistrationToExhibitionId = tuple.Item2,
            IsLitter = false,
            CatRegistrationId = null
        });
        List<ExhibitionCagesInfo> exhibitionData = await SendAsync(command);

        // Assert
        availableCages.Should().NotBeNull();
        availableCages.ExhibitorsCages.Should().BeEmpty();
        availableCages.AvailableAlreadyRentedCagesByExhibitor.Should().BeEmpty();
        availableCages.AvailableCagesForRent.Should().NotBeEmpty();
        availableCages.AvailableCagesForRent.Should().HaveCount(1);
        availableCages.AvailableCagesForRent[0].RentedTypeId.Should()
            .Be(new RentedCageGroup(60, 120, 60, RentedType.Single, RentingType.RentedWithZeroOtherCats));

        exhibitionData.Should().NotBeNull();
        exhibitionData.Count.Should().Be(2);
        exhibitionData[0].NumberOfPersonCages.Should().Be(0);
        exhibitionData[0].CageGroupInfos.Count.Should().Be(1);
        exhibitionData[0].CageGroupInfos[0].NumberOfUsedCages.Should().Be(new CagesRec
        {
            NumberOfDoubleCages = 0, NumberOfSingleCages = 1
        });
        exhibitionData[0].CageGroupInfos[0].TotalNumberOfRentedCages.Should().Be(1);

        exhibitionData[1].NumberOfPersonCages.Should().Be(0);
        exhibitionData[1].CageGroupInfos.Count.Should().Be(1);
        exhibitionData[1].CageGroupInfos[0].NumberOfUsedCages.Should().Be(new CagesRec
        {
            NumberOfDoubleCages = 0, NumberOfSingleCages = 1
        });
        exhibitionData[1].CageGroupInfos[0].TotalNumberOfRentedCages.Should().Be(1);
    }

    [Test]
    public async Task ShouldReturnAvailableCagesForOneRegisteredRentedDoubleCageCat()
    {
        // Arrange
        string userId = await RunAsAdministratorAsync();
        Tuple<BriefExhibitionDto, int> tuple = await CreateRegistrationToExhibition(userId);
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = tuple.Item1.Id });

        Result<int> result = await SendAsync(new CreateCatRegistrationCommand
        {
            CatRegistration = CatRegistrationDataGenerator.WithDays(tuple.Item2, exhibitionDays,
                new RentedCageGroup(60, 120, 60, RentedType.Double, RentingType.RentedWithZeroOtherCats), null)
        });

        result.IsSuccess.Should().Be(true);


        // Act
        GetExhibitionCagesInfoQuery command = new() { ExhibitionId = tuple.Item1.Id };
        CagesPerDayDto availableCages = await SendAsync(new GetAvailableRentedCageTypesQuery
        {
            ExhibitionDayId = exhibitionDays.First().Id,
            RegistrationToExhibitionId = tuple.Item2,
            IsLitter = false,
            CatRegistrationId = null
        });
        List<ExhibitionCagesInfo> exhibitionData = await SendAsync(command);

        // Assert
        availableCages.Should().NotBeNull();
        availableCages.ExhibitorsCages.Should().BeEmpty();
        availableCages.AvailableAlreadyRentedCagesByExhibitor.Should().HaveCount(1);
        availableCages.AvailableCagesForRent.Should().BeEmpty();
        availableCages.AvailableCagesForRent.Should().HaveCount(0);
        availableCages.AvailableAlreadyRentedCagesByExhibitor[0].RentedTypeId.Should()
            .Be(new RentedCageGroup(60, 120, 60, RentedType.Double, RentingType.RentedWithOneOtherCat));

        exhibitionData.Should().NotBeNull();
        exhibitionData.Count.Should().Be(2);
        exhibitionData[0].NumberOfPersonCages.Should().Be(0);
        exhibitionData[0].CageGroupInfos.Count.Should().Be(1);
        exhibitionData[0].CageGroupInfos[0].NumberOfUsedCages.Should().Be(new CagesRec
        {
            NumberOfDoubleCages = 1, NumberOfSingleCages = 0
        });
        exhibitionData[0].CageGroupInfos[0].TotalNumberOfRentedCages.Should().Be(1);

        exhibitionData[1].NumberOfPersonCages.Should().Be(0);
        exhibitionData[1].CageGroupInfos.Count.Should().Be(1);
        exhibitionData[1].CageGroupInfos[0].NumberOfUsedCages.Should().Be(new CagesRec
        {
            NumberOfDoubleCages = 1, NumberOfSingleCages = 0
        });
        exhibitionData[1].CageGroupInfos[0].TotalNumberOfRentedCages.Should().Be(1);
    }


    private async Task<Tuple<BriefExhibitionDto, int>> CreateRegistrationToExhibition(string userId)
    {
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(userId)
        })).Value;
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        int exhibition1Id = (await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        })).Value;
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition1Id });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds =
            [
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13a", "13b", "13c", "14", "15", "16",
                "17"
            ],
            ExhibitionId = exhibition1Id,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = exhibitionDays.Select(x => x.Id).ToList(),
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds =
            [
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13a", "13b", "13c", "14", "15", "16",
                "17"
            ],
            ExhibitionId = exhibition1Id,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = [exhibitionDays.Select(x => x.Id).ToList()[0]],
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds =
            [
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13a", "13b", "13c", "14", "15", "16",
                "17"
            ],
            ExhibitionId = exhibition1Id,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = [exhibitionDays.Select(x => x.Id).ToList()[1]],
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id
        });

        CreateRentedRentedCageDto rentedRentedRentedRentedCageDto =
            new()
            {
                Count = 1,
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList(),
                RentedCageTypes = [RentedType.Double, RentedType.Single],
                Height = 60,
                Length = 120,
                Width = 60
            };
        await SendAsync(
            new AddNewRentedCageGroupToExhibitionCommand
            {
                CreateRentedRentedCageDto = rentedRentedRentedRentedCageDto
            });

        await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id });
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = userId, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);


        BriefExhibitionDto exhibition = await SendAsync(new GetExhibitionByIdQuery { ExhibitionId = exhibition1Id });
        return new Tuple<BriefExhibitionDto, int>(exhibition, registrationToExhibitionId.Value);
    }

    [Test]
    public async Task ShouldReturnAvailableCagesForOwnCageSingleCageCat()
    {
        // Arrange
        string userId = await RunAsAdministratorAsync();
        Tuple<BriefExhibitionDto, int> tuple = await CreateRegistrationToExhibition(userId);
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = tuple.Item1.Id });

        Result<int> result = await SendAsync(new CreateCatRegistrationCommand
        {
            CatRegistration = CatRegistrationDataGenerator.WithDays(tuple.Item2, exhibitionDays,
                null, new CreateCageDto { Height = 60, Length = 60, Width = 60 })
        });

        result.IsSuccess.Should().Be(true);

        // Act
        GetExhibitionCagesInfoQuery command = new() { ExhibitionId = tuple.Item1.Id };
        CagesPerDayDto availableCages = await SendAsync(new GetAvailableRentedCageTypesQuery
        {
            ExhibitionDayId = exhibitionDays.First().Id,
            RegistrationToExhibitionId = tuple.Item2,
            IsLitter = false,
            CatRegistrationId = null
        });
        List<ExhibitionCagesInfo> exhibitionData = await SendAsync(command);


        // Assert
        availableCages.Should().NotBeNull();
        availableCages.AvailableCagesForRent.Should().NotBeNull();
        availableCages.AvailableCagesForRent.Should().HaveCount(2);
        availableCages.AvailableCagesForRent[0].RentedTypeId.Should()
            .Be(new RentedCageGroup(60, 120, 60, RentedType.Double, RentingType.RentedWithZeroOtherCats));
        availableCages.AvailableCagesForRent[1].RentedTypeId.Should()
            .Be(new RentedCageGroup(60, 120, 60, RentedType.Single, RentingType.RentedWithZeroOtherCats));

        availableCages.AvailableAlreadyRentedCagesByExhibitor.Should().BeEmpty();

        availableCages.ExhibitorsCages.Should().BeEmpty();

        exhibitionData.Should().NotBeNull();
        exhibitionData.Count.Should().Be(2);
        exhibitionData[0].NumberOfPersonCages.Should().Be(1);
        exhibitionData[0].CageGroupInfos.Count.Should().Be(1);
        exhibitionData[0].CageGroupInfos[0].NumberOfUsedCages.Should().Be(new CagesRec
        {
            NumberOfDoubleCages = 0, NumberOfSingleCages = 0
        });
        exhibitionData[0].CageGroupInfos[0].TotalNumberOfRentedCages.Should().Be(1);

        exhibitionData[1].NumberOfPersonCages.Should().Be(1);
        exhibitionData[1].CageGroupInfos.Count.Should().Be(1);
        exhibitionData[1].CageGroupInfos[0].NumberOfUsedCages.Should().Be(new CagesRec
        {
            NumberOfDoubleCages = 0, NumberOfSingleCages = 0
        });
        exhibitionData[1].CageGroupInfos[0].TotalNumberOfRentedCages.Should().Be(1);
    }

    [Test]
    public async Task ShouldReturnAvailableCagesForOwnCageDoubleCageCat()
    {
        // Arrange
        string userId = await RunAsAdministratorAsync();
        Tuple<BriefExhibitionDto, int> tuple = await CreateRegistrationToExhibition(userId);
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = tuple.Item1.Id });

        Result<int> result = await SendAsync(new CreateCatRegistrationCommand
        {
            CatRegistration = CatRegistrationDataGenerator.WithDays(tuple.Item2, exhibitionDays,
                null, new CreateCageDto { Height = 60, Length = 120, Width = 60 })
        });

        result.IsSuccess.Should().Be(true);

        // Act
        GetExhibitionCagesInfoQuery command = new() { ExhibitionId = tuple.Item1.Id };
        CagesPerDayDto availableCages = await SendAsync(new GetAvailableRentedCageTypesQuery
        {
            ExhibitionDayId = exhibitionDays.First().Id,
            RegistrationToExhibitionId = tuple.Item2,
            IsLitter = false,
            CatRegistrationId = null
        });
        List<ExhibitionCagesInfo> exhibitionData = await SendAsync(command);

        // Assert
        availableCages.Should().NotBeNull();
        availableCages.AvailableCagesForRent.Should().NotBeNull();
        availableCages.AvailableCagesForRent.Should().HaveCount(2);
        availableCages.AvailableCagesForRent[0].RentedTypeId.Should()
            .Be(new RentedCageGroup(60, 120, 60, RentedType.Double, RentingType.RentedWithZeroOtherCats));
        availableCages.AvailableCagesForRent[1].RentedTypeId.Should()
            .Be(new RentedCageGroup(60, 120, 60, RentedType.Single, RentingType.RentedWithZeroOtherCats));

        availableCages.AvailableAlreadyRentedCagesByExhibitor.Should().BeEmpty();

        availableCages.ExhibitorsCages.Should().NotBeEmpty();
        availableCages.ExhibitorsCages.Should().HaveCount(1);
        availableCages.ExhibitorsCages[0].Description.Should().Be("Moje klec 120x60");

        exhibitionData.Should().NotBeNull();
        exhibitionData.Count.Should().Be(2);
        exhibitionData[0].NumberOfPersonCages.Should().Be(1);
        exhibitionData[0].CageGroupInfos.Count.Should().Be(1);
        exhibitionData[0].CageGroupInfos[0].NumberOfUsedCages.Should().Be(new CagesRec
        {
            NumberOfDoubleCages = 0, NumberOfSingleCages = 0
        });
        exhibitionData[0].CageGroupInfos[0].TotalNumberOfRentedCages.Should().Be(1);

        exhibitionData[1].NumberOfPersonCages.Should().Be(1);
        exhibitionData[1].CageGroupInfos.Count.Should().Be(1);
        exhibitionData[1].CageGroupInfos[0].NumberOfUsedCages.Should().Be(new CagesRec
        {
            NumberOfDoubleCages = 0, NumberOfSingleCages = 0
        });
        exhibitionData[1].CageGroupInfos[0].TotalNumberOfRentedCages.Should().Be(1);
    }
}
