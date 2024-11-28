#region

using RegisterMe.Application.Cages;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.CatRegistrations.Queries.GetCatRegistrationById;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetGroupsCanBeRegisteredIn;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Application.Services.Groups;
using RegisterMe.Domain.Common;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.CatRegistrations.Commands.
    CreateCatRegistration;

public class CreateCatRegistrationSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldFailCreateCatRegistrationForEmptyLitterAndEmptyCat()
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = null,
                Litter = null,
                Note = null,
                CatDays =
                [
                    new CreateCatDayDto
                    {
                        RentedCageTypeId = new RentedCageGroup("120;120;120;Double;RentedWithZeroOtherCats"),
                        ExhibitorsCage = null,
                        ExhibitionDayId = exhibitionDays.First().Id,
                        GroupsIds = ["1"],
                        Cage = null
                    }
                ]
            }
        };

        // Act
        Result<int> catRegistrationId = await SendAsync(command);

        // Assert
        catRegistrationId.IsSuccess.Should().BeFalse();
    }

    [Test]
    public async Task ShouldCreateCatRegistrationForCat()
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();

        CreateExhibitedCatDto createCatRegistration = CatRegistrationDataGenerator.CreateExhibitedCat;

        UpsertExhibitorDto exhibitor = ExhibitorDataGenerator.GetExhibitorDto1();
        List<DatabaseGroupDto> groups = await SendAsync(new GetGroupsCanBeRegisteredInQuery
            {
                CatRegistration =
                    new LitterOrExhibitedCatDto
                    {
                        ExhibitedCat = createCatRegistration, LitterDto = null, ExhibitorDto = exhibitor
                    },
                ExhibitionDayId = exhibitionDays.First().Id
            }
        );
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = createCatRegistration,
                Litter = null,
                Note = null,
                CatDays =
                [
                    new CreateCatDayDto
                    {
                        RentedCageTypeId = new RentedCageGroup("120;120;120;Double;RentedWithZeroOtherCats"),
                        ExhibitorsCage = null,
                        ExhibitionDayId = exhibitionDays.First().Id,
                        GroupsIds = [groups.First().GroupId],
                        Cage = null
                    }
                ]
            }
        };

        // Act
        Result<int> catRegistrationId = await SendAsync(command);

        // Assert
        catRegistrationId.IsSuccess.Should().BeTrue();

        CatRegistrationDto catRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = catRegistrationId.Value });
        catRegistration.Should().NotBeNull();
        CompareUtils.Equals(command.CatRegistration, catRegistration).Should().BeTrue();
    }

    [Test]
    public async Task ShouldCreateCatRegistrationForLitter()
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();

        CreateLitterDto litter = CatRegistrationDataGenerator.CreateLitter;

        UpsertExhibitorDto exhibitor = ExhibitorDataGenerator.GetExhibitorDto1();

        List<DatabaseGroupDto> groups = await SendAsync(new GetGroupsCanBeRegisteredInQuery
            {
                CatRegistration =
                    new LitterOrExhibitedCatDto { LitterDto = litter, ExhibitedCat = null, ExhibitorDto = exhibitor },
                ExhibitionDayId = exhibitionDays.First().Id
            }
        );
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = null,
                Litter = litter,
                Note = null,
                CatDays =
                [
                    new CreateCatDayDto
                    {
                        RentedCageTypeId = new RentedCageGroup("120;120;120;Double;RentedWithZeroOtherCats"),
                        ExhibitorsCage = null,
                        ExhibitionDayId = exhibitionDays.First().Id,
                        GroupsIds = [groups.First().GroupId],
                        Cage = null
                    }
                ]
            }
        };

        // Act
        Result<int> catRegistrationId = await SendAsync(command);

        // Assert
        catRegistrationId.IsSuccess.Should().BeTrue();
        CatRegistrationDto catRegistration =
            await SendAsync(new GetCatRegistrationByIdQuery { Id = catRegistrationId.Value });
        catRegistration.Should().NotBeNull();
        RegistrationToExhibitionDto regToExhibition =
            await SendAsync(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value
            });
        regToExhibition.Should().NotBeNull();
        regToExhibition.CatRegistrationIds.Should().Contain(catRegistrationId.Value);
        regToExhibition.CatRegistrationIds.Count.Should().Be(1);

        CompareUtils.Equals(command.CatRegistration, catRegistration).Should().BeTrue();
    }
}
