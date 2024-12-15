#region

using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Commands.DeleteCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.CatRegistrations.Queries.GetCatRegistrationById;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Domain.Common;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.CatRegistrations.Commands.
    DeleteCatRegistration;

public class DeleteCatRegistrationSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();

    [Test]
    public async Task ShouldDeleteCatRegistration()
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        CreateExhibitedCatDto createCatRegistration = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> catDaysDto =
            _testData.GetCatDayDto(TestData.CatDays.NineDoubleCage, exhibitionDays.Select(x => x.Id).ToList());
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = createCatRegistration,
                Litter = null,
                Note = null,
                CatDays = catDaysDto
            }
        };
        Result<int> catRegistrationId = await SendAsync(command);
        catRegistrationId.IsSuccess.Should().BeTrue();

        // Act
        await SendAsync(new DeleteCatRegistrationCommand { CatRegistrationId = catRegistrationId.Value });

        // Assert
        Func<Task> getCatRegistration = async () =>
            await SendAsync(new GetCatRegistrationByIdQuery { Id = catRegistrationId.Value });
        await getCatRegistration.Should().ThrowAsync<NotFoundException>();
    }
}
