#region

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.RequestDelayedPayment;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.RequestDelayedPayment;

#region

using static Testing;

#endregion

public class RequestDelayedPaymentAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();

    [Test]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    [TestCase(RunAsSpecificUser.RunAsVojta)]
    public async Task ShouldRequestDelayedPayment(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
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
        IWebHostEnvironment env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        await RunAsExecutor(runAsSpecificUser);

        // Act
        RequestDelayedPaymentCommand payment = new()
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value,
            PaymentType = PaymentType.PayInPlaceByCache,
            Currency = Currency.Czk,
            WebAddress = "wwww.kocky.cz",
            RootPath = env.ContentRootPath
        };

        // Assert
        Func<Task> act = async () =>
            await SendAsync(payment);
        await act.Should().NotThrowAsync();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    public async Task ShouldFailRequestDelayedPayment(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
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
        IWebHostEnvironment env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        // Act
        RequestDelayedPaymentCommand payment = new()
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value,
            PaymentType = PaymentType.PayInPlaceByCache,
            Currency = Currency.Czk,
            WebAddress = "wwww.kocky.cz",
            RootPath = env.ContentRootPath
        };
        await RunAsExecutor(runAsSpecificUser);

        // Assert
        Func<Task> act = async () =>
            await SendAsync(payment);
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
