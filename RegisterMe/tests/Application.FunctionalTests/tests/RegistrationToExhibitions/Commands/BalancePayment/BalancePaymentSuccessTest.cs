#region

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.BalancePayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.FinishDelayedPayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.RequestDelayedPayment;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    BalancePayment;

public class BalancePaymentSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();


    [Test]
    public async Task ShouldBalancePayment()
    {
        // Arrange
        await RunAsVojtaAsync();
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
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
        await SendAsync(command);

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

        RegistrationToExhibitionDto originalData =
            await SendAsync(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value
            });

        CreateExhibitedCatDto creaeCatRegistration2 = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat2,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);

        CreateCatRegistrationCommand command1 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = creaeCatRegistration2,
                Litter = null,
                Note = null,
                CatDays = catDaysDto
            }
        };

        await RunAsOndrejAsync();
        await SendAsync(command1);

        // Act
        Result result = await SendAsync(new BalancePaymentCommand
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value,
            WebAddress = "wwww.kocky.cz",
            RootPath = env.ContentRootPath
        });

        // Assert
        result.IsSuccess.Should().BeTrue();

        RegistrationToExhibitionDto newData =
            await SendAsync(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value
            });

        newData.PaymentInfo!.Amount.Should().BeGreaterThan(originalData.PaymentInfo!.Amount);
    }

    [Test]
    public async Task ShouldFailBalancePayment()
    {
        // Arrange
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
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
        await SendAsync(command);

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

        CreateExhibitedCatDto creaeCatRegistration2 = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat2,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);

        await RunAsOndrejAsync();
        CreateCatRegistrationCommand command1 = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = creaeCatRegistration2,
                Litter = null,
                Note = null,
                CatDays = catDaysDto
            }
        };

        await SendAsync(command1);

        // Act
        Result result = await SendAsync(new BalancePaymentCommand
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value,
            WebAddress = "wwww.kocky.cz",
            RootPath = env.ContentRootPath
        });

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
