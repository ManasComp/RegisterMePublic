#region

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.StartOnlinePayment;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.StartOnlinePayment;

#region

using static Testing;

#endregion

public class StartOnlinePaymentAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();

    [Test]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    [TestCase(RunAsSpecificUser.RunAsVojta)]
    public async Task ShouldStartOnlinePayment(RunAsSpecificUser runAsSpecificUser)
    {
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();

        await RunAsVojtaAsync();
        CreateExhibitedCatDto creaeCatRegistration = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> catDaysDto =
            _testData.GetCatDayDto(TestData.CatDays.BothDays, exhibitionDays.Select(x => x.Id).ToList());
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = creaeCatRegistration,
                Litter = null,
                Note = null,
                CatDays = catDaysDto
            }
        };
        Result<int> result = await SendAsync(command);
        result.IsSuccess.Should().BeTrue();

        IServiceScopeFactory scopeFactory = GetScopeFactory();

        IServiceScope scope = scopeFactory.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        await RunAsExecutor(runAsSpecificUser);
        StartOnlinePaymentCommand payment = new()
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value,
            SuccessUrl = "https://www.kocky.cz/success",
            Currency = Currency.Czk,
            CancelUrl = "https://www.kocky.cz/cancel"
        };
        await SendAsync(payment);
    }


    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    public async Task ShouldFailStartOnlinePayment(RunAsSpecificUser runAsSpecificUser)
    {
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();

        await RunAsVojtaAsync();
        CreateExhibitedCatDto createCatRegistration = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> catDaysDto =
            _testData.GetCatDayDto(TestData.CatDays.BothDays, exhibitionDays.Select(x => x.Id).ToList());
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
        Result<int> result = await SendAsync(command);
        result.IsSuccess.Should().BeTrue();

        IServiceScopeFactory scopeFactory = GetScopeFactory();

        IServiceScope scope = scopeFactory.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        StartOnlinePaymentCommand payment = new()
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value,
            SuccessUrl = "https://www.kocky.cz/success",
            Currency = Currency.Czk,
            CancelUrl = "https://www.kocky.cz/cancel"
        };
        await RunAsExecutor(runAsSpecificUser);

        Func<Task> act = async () =>
            await SendAsync(payment);
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
