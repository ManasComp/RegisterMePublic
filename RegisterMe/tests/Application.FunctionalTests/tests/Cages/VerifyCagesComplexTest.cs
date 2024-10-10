#region

using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.Combination;
using RegisterMe.Application.Cages.Queries.GetAvailableRentedCageTypes;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Commands.UpdateCatRegistrationCommand;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.CatRegistrations.Queries.GetCatRegistrationById;
using RegisterMe.Application.Common.Utils;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionCagesInfo;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Domain.Common;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.
    Cages;

public class VerifyCagesComplexTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();

    [Test]
    public async Task ShouldBeAbleToDoComplexActionsWithCages()
    {
        ListComparer<RentedCageGroup?> comparer = new();
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();

        CreateCatRegistrationCommand cat1 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                    TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = null,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage,
                    exhibitionDays.Select(x => x.Id).ToList())
            }
        };
        Result<int> catId1 = await SendAsync(cat1);
        catId1.IsSuccess.Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 1, 0);

        CreateCatRegistrationCommand cat2 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                    TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = null,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCageShared1,
                    exhibitionDays.Select(x => x.Id).ToList())
            }
        };
        Result<int> catId2 = await SendAsync(cat2);
        catId2.IsSuccess.Should().BeTrue();
        CatRegistrationDto catId2Cat = await SendAsync(new GetCatRegistrationByIdQuery { Id = catId2.Value });
        CatRegistrationDto catId1Cat = await SendAsync(new GetCatRegistrationByIdQuery { Id = catId1.Value });
        comparer.Equals(catId2Cat.CatDays.Select(x => x.RentedCageTypeId).ToList(),
            catId1Cat.CatDays.Select(x => x.RentedCageTypeId).ToList()).Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 1, 0);

        CreateCatRegistrationCommand cat3 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                    TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = null,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCageShared2,
                    exhibitionDays.Select(x => x.Id).ToList())
            }
        };
        Result<int> catId3 = await SendAsync(cat3);
        catId3.IsSuccess.Should().BeTrue();
        CatRegistrationDto catId31Cat = await SendAsync(new GetCatRegistrationByIdQuery { Id = catId2.Value });
        CatRegistrationDto catId21Cat = await SendAsync(new GetCatRegistrationByIdQuery { Id = catId2.Value });
        CatRegistrationDto catId11Cat = await SendAsync(new GetCatRegistrationByIdQuery { Id = catId1.Value });
        comparer.Equals(catId21Cat.CatDays.Select(x => x.RentedCageTypeId).ToList(),
            catId11Cat.CatDays.Select(x => x.RentedCageTypeId).ToList()).Should().BeTrue();
        comparer.Equals(catId31Cat.CatDays.Select(x => x.RentedCageTypeId).ToList(),
            catId21Cat.CatDays.Select(x => x.RentedCageTypeId).ToList()).Should().BeTrue();
        comparer.Equals(catId31Cat.CatDays.Select(x => x.RentedCageTypeId).ToList(),
            catId11Cat.CatDays.Select(x => x.RentedCageTypeId).ToList()).Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 1, 0);

        CreateCatRegistrationCommand cat4 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                    TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = null,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage,
                    exhibitionDays.Select(x => x.Id).ToList())
            }
        };
        Result<int> catId4 = await SendAsync(cat4);
        catId4.IsSuccess.Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 2, 0);

        CreateCatRegistrationCommand cat5 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                    TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = null,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCageShared1,
                    exhibitionDays.Select(x => x.Id).ToList())
            }
        };
        Result<int> catId5 = await SendAsync(cat5);
        catId5.IsSuccess.Should().BeTrue();
        CatRegistrationDto catId4Cat = await SendAsync(new GetCatRegistrationByIdQuery { Id = catId4.Value });
        CatRegistrationDto catId5Cat = await SendAsync(new GetCatRegistrationByIdQuery { Id = catId5.Value });
        comparer.Equals(catId4Cat.CatDays.Select(x => x.RentedCageTypeId).ToList(),
            catId5Cat.CatDays.Select(x => x.RentedCageTypeId).ToList()).Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 2, 0);

        CreateCatRegistrationCommand cat6 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                    TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = null,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCageShared2,
                    exhibitionDays.Select(x => x.Id).ToList())
            }
        };
        Result<int> catId6 = await SendAsync(cat6);
        catId6.IsSuccess.Should().BeTrue();
        CatRegistrationDto catId41Cat = await SendAsync(new GetCatRegistrationByIdQuery { Id = catId4.Value });
        CatRegistrationDto catId51Cat = await SendAsync(new GetCatRegistrationByIdQuery { Id = catId5.Value });
        CatRegistrationDto catId61Cat = await SendAsync(new GetCatRegistrationByIdQuery { Id = catId6.Value });
        comparer.Equals(catId41Cat.CatDays.Select(x => x.RentedCageTypeId).ToList(),
            catId51Cat.CatDays.Select(x => x.RentedCageTypeId).ToList()).Should().BeTrue();
        comparer.Equals(catId51Cat.CatDays.Select(x => x.RentedCageTypeId).ToList(),
            catId61Cat.CatDays.Select(x => x.RentedCageTypeId).ToList()).Should().BeTrue();
        comparer.Equals(catId41Cat.CatDays.Select(x => x.RentedCageTypeId).ToList(),
            catId61Cat.CatDays.Select(x => x.RentedCageTypeId).ToList()).Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 2, 0);

        CreateCatRegistrationCommand cat8 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                    TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = null,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage,
                    exhibitionDays.Select(x => x.Id).ToList())
            }
        };
        Result<int> catId8 = await SendAsync(cat8);
        catId8.IsSuccess.Should().BeTrue();
        await SendAsync(new GetCatRegistrationByIdQuery { Id = catId8.Value });
        await VerifyCages(registrationToExhibitionId, 3, 0);

        CreateCatRegistrationCommand cat9 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                    TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = null,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCageShared1,
                    exhibitionDays.Select(x => x.Id).ToList())
            }
        };
        Result<int> result = await SendAsync(cat9);
        result.IsSuccess.Should().BeTrue();


        CreateCatRegistrationCommand cat10 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
                    TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1),
                Litter = null,
                Note = null,
                CatDays = _testData.GetCatDayDto(TestData.CatDays.Single, exhibitionDays.Select(x => x.Id).ToList())
            }
        };
        Result<int> catId10 = await SendAsync(cat10);
        catId10.IsSuccess.Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 3, 1);

        UpdateCatRegistrationCommand
            updateCat6 = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = catId6.Value,
                    ExhibitedCat = _testData.GetExhibitedCatDto(
                        TestData.ExhibitedCats.ExhibitedCat1, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
                        TestData.Mothers.Mother1),
                    CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage,
                        exhibitionDays.Select(x => x.Id).ToList()),
                    Note = "Updated",
                    Litter = null
                }
            };

        (await SendAsync(updateCat6)).IsSuccess.Should().BeTrue();

        await VerifyCages(registrationToExhibitionId, 4, 1);

        UpdateCatRegistrationCommand
            updateCa10 = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = catId10.Value,
                    ExhibitedCat = _testData.GetExhibitedCatDto(
                        TestData.ExhibitedCats.ExhibitedCat1, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
                        TestData.Mothers.Mother1),
                    CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCageShared2,
                        exhibitionDays.Select(x => x.Id).ToList()),
                    Note = "Updated",
                    Litter = null
                }
            };
        Result<int> updateCat10 = await SendAsync(updateCa10);
        updateCat10.IsSuccess.Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 4, 0);

        UpdateCatRegistrationCommand
            updateCat110 = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = updateCat10.Value,
                    ExhibitedCat = _testData.GetExhibitedCatDto(
                        TestData.ExhibitedCats.ExhibitedCat1, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
                        TestData.Mothers.Mother1),
                    CatDays = _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage,
                        exhibitionDays.Select(x => x.Id).ToList()),
                    Note = "Updated",
                    Litter = null
                }
            };
        (await SendAsync(updateCat110)).IsSuccess.Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 5, 0);


        UpdateCatRegistrationCommand
            updateCat1110 = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = updateCat10.Value,
                    ExhibitedCat = _testData.GetExhibitedCatDto(
                        TestData.ExhibitedCats.ExhibitedCat1, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
                        TestData.Mothers.Mother1),
                    CatDays = _testData.GetCatDayDto(TestData.CatDays.Personal,
                        exhibitionDays.Select(x => x.Id).ToList()),
                    Note = "Updated",
                    Litter = null
                }
            };
        (await SendAsync(updateCat1110)).IsSuccess.Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 4, 0);

        CagesPerDayDto getAvailablePersonalCagesDay1 = await SendAsync(new GetAvailableRentedCageTypesQuery
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value,
            ExhibitionDayId = exhibitionDays.First().Id,
            IsLitter = false,
            CatRegistrationId = null
        });
        getAvailablePersonalCagesDay1.ExhibitorsCages.Count.Should().Be(1);


        UpdateCatRegistrationCommand
            updateCat11110 = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Id = updateCat10.Value,
                    ExhibitedCat = _testData.GetExhibitedCatDto(
                        TestData.ExhibitedCats.ExhibitedCat1, TestData.Breeders.Breeder1, TestData.Fathers.Father1,
                        TestData.Mothers.Mother1),
                    CatDays = _testData.GetCatDayDto(TestData.CatDays.Personal,
                        exhibitionDays.Select(x => x.Id).ToList(),
                        getAvailablePersonalCagesDay1.ExhibitorsCages.First().CageId),
                    Note = "Updated",
                    Litter = null
                }
            };
        (await SendAsync(updateCat11110)).IsSuccess.Should().BeTrue();
        await VerifyCages(registrationToExhibitionId, 4, 0);
    }

    private static async Task VerifyCages(Result<int> registrationToExhibitionId, int countOfDouble, int countOfSingle)
    {
        RegistrationToExhibitionDto registrationToExhibition = await SendAsync(
            new GetRegistrationToExhibitionByIdQuery { RegistrationToExhibitionId = registrationToExhibitionId.Value });
        await RunAsOndrejAsync();
        List<ExhibitionCagesInfo> cagesInfo =
            await SendAsync(new GetExhibitionCagesInfoQuery { ExhibitionId = registrationToExhibition.ExhibitionId });

        cagesInfo.Count.Should().Be(2);
        cagesInfo[0].CageGroupInfos.Select(x => x.NumberOfUsedCages.NumberOfDoubleCages).Sum().Should()
            .Be(countOfDouble);
        cagesInfo[0].CageGroupInfos.Select(x => x.NumberOfUsedCages.NumberOfSingleCages).Sum().Should()
            .Be(countOfSingle);

        await RunAsVojtaAsync();
    }
}
