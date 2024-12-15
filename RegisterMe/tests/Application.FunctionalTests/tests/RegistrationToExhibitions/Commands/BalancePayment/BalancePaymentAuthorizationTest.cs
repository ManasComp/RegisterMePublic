#region

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.BalancePayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.FinishDelayedPayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.RequestDelayedPayment;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.BalancePayment;

#region

using static Testing;

#endregion

public class BalancePaymentAuthorizationTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();

    [Test]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    public async Task ShouldBalancePayment(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
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
        IWebHostEnvironment env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        RequestDelayedPaymentCommand payment = new()
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value,
            PaymentType = PaymentType.PayInPlaceByCache,
            Currency = Currency.Czk,
            WebAddress = "wwww.kocky.cz",
            RootPath = env.ContentRootPath
        };
        await SendAsync(payment);

        await RunAsOndrejAsync();
        await SendAsync(new FinishDelayedPaymentCommand
        {
            RegistrationToExhibitionId = payment.RegistrationToExhibitionId,
            WebAddress = "wwww.kocky.cz",
            RootPath = env.ContentRootPath
        });

        await RunAsVojtaAsync();

        await SendAsync(new GetRegistrationToExhibitionByIdQuery
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value
        });

        CreateExhibitedCatDto createCatRegistration2 = _testData.GetExhibitedCatDto(
            TestData.ExhibitedCats.ExhibitedCat2,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);

        CreateCatRegistrationCommand command1 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = createCatRegistration2,
                Litter = null,
                Note = null,
                CatDays = catDaysDto
            }
        };

        await RunAsOndrejAsync();
        await SendAsync(command1);
        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> balance = async () =>
            await SendAsync(new BalancePaymentCommand
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                WebAddress = "wwww.kocky.cz",
                RootPath = env.ContentRootPath
            });

        // Assert
        await balance.Should().NotThrowAsync();
    }


    [Test]
    [TestCase(RunAsSpecificUser.RunAsAnonymous)]
    [TestCase(RunAsSpecificUser.RunAsVojta)]
    [TestCase(RunAsSpecificUser.RunAsSabrina)]
    public async Task ShouldFailBalancePayment(RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
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
        IWebHostEnvironment env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        RequestDelayedPaymentCommand payment = new()
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value,
            PaymentType = PaymentType.PayInPlaceByCache,
            Currency = Currency.Czk,
            WebAddress = "wwww.kocky.cz",
            RootPath = env.ContentRootPath
        };
        await SendAsync(payment);

        await RunAsOndrejAsync();
        await SendAsync(new FinishDelayedPaymentCommand
        {
            RegistrationToExhibitionId = payment.RegistrationToExhibitionId,
            WebAddress = "wwww.kocky.cz",
            RootPath = env.ContentRootPath
        });

        await RunAsVojtaAsync();

        await SendAsync(new GetRegistrationToExhibitionByIdQuery
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value
        });

        CreateExhibitedCatDto createCatRegistration2 = _testData.GetExhibitedCatDto(
            TestData.ExhibitedCats.ExhibitedCat2,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);

        CreateCatRegistrationCommand command1 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = createCatRegistration2,
                Litter = null,
                Note = null,
                CatDays = catDaysDto
            }
        };

        await RunAsOndrejAsync();
        await SendAsync(command1);
        await RunAsExecutor(runAsSpecificUser);

        // Act
        Func<Task> balance = async () =>
            await SendAsync(new BalancePaymentCommand
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                WebAddress = "wwww.kocky.cz",
                RootPath = env.ContentRootPath
            });

        // Assert
        await balance.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
