#region

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.DeleteTemporaryRegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Commands.RequestDelayedPayment;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationsToExhibitionByExhibitorId;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionByExhibitionId;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    DeleteTemporaryRegistrationToExhibition;

public class DeleteTemporaryRegistrationToExhibitionCommandSuccessTest(DatabaseTypes databaseType)
    : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();


    [Test]
    public async Task ShouldDeleteTemporaryRegistrationToExhibition()
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
        await SendAsync(command);

        RegistrationToExhibitionDto registrationToExhibition = await SendAsync(
            new GetRegistrationToExhibitionByIdQuery { RegistrationToExhibitionId = registrationToExhibitionId.Value });

        RegistrationToExhibitionDto registration = await SendAsync(new GetRegistrationToExhibitionByIdQuery
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value
        });
        List<RegistrationToExhibitionDto> registrations =
            await SendAsync(
                new GetRegistrationsToExhibitionByExhibitorIdQuery { ExhibitorId = registration.ExhibitorId });

        registrations.Count.Should().Be(1);


        await RunAsOndrejAsync();

        // Act
        Result result = await SendAsync(new DeleteTemporaryRegistrationToExhibitionCommand
        {
            ExhibitionId = registrationToExhibition.ExhibitionId, WebAddress = "https://www.google.com"
        });

        // Assert
        result.IsSuccess.Should().BeTrue();

        await RunAsVojtaAsync();

        registrations = await SendAsync(new GetRegistrationsToExhibitionByExhibitorIdQuery
        {
            ExhibitorId = registration.ExhibitorId
        });

        registrations.Count.Should().Be(0);
    }

    [Test]
    public async Task ShouldFailDeleteTemporaryRegistrationToExhibition()
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


        RegistrationToExhibitionDto registrationToExhibition = await SendAsync(
            new GetRegistrationToExhibitionByIdQuery { RegistrationToExhibitionId = registrationToExhibitionId.Value });

        await RunAsOndrejAsync();

        List<RegistrationToExhibitionDto> registrations = await SendAsync(
            new GetRegistrationsToExhibitionByExhibitionIdQuery
            {
                ExhibitionId = registrationToExhibition.ExhibitionId
            });

        registrations.Count.Should().Be(1);

        // Act
        Result result = await SendAsync(new DeleteTemporaryRegistrationToExhibitionCommand
        {
            ExhibitionId = registrationToExhibition.ExhibitionId, WebAddress = "https://www.google.com"
        });

        // Assert
        result.IsSuccess.Should().BeTrue();

        registrations = await SendAsync(new GetRegistrationsToExhibitionByExhibitionIdQuery
        {
            ExhibitionId = registrationToExhibition.ExhibitionId
        });

        registrations.Count.Should().Be(1);
    }
}
