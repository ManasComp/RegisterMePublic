#region

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.RegistrationToExhibition.Commands.StartOnlinePayment;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    StartOnlinePayment;

public class StartOnlinePaymentSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    private readonly TestData _testData = new();


    [Test]
    public async Task ShouldStartOnlinePayment()
    {
        // Arrange
        await RunAsVojtaAsync();
        (List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId) =
            await InitializeTestEnvironmentFactory.InitializeTestEnvironment();
        CreateExhibitedCatDto createExhibitedCatDto = _testData.GetExhibitedCatDto(TestData.ExhibitedCats.ExhibitedCat1,
            TestData.Breeders.Breeder1, TestData.Fathers.Father1, TestData.Mothers.Mother1);
        List<CreateCatDayDto> catDaysDto =
            _testData.GetCatDayDto(TestData.CatDays.BothDays, exhibitionDays.Select(x => x.Id).ToList());
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value,
                ExhibitedCat = createExhibitedCatDto,
                Litter = null,
                Note = null,
                CatDays = catDaysDto
            }
        };
        await SendAsync(command);

        IServiceScopeFactory scopeFactory = GetScopeFactory();

        IServiceScope scope = scopeFactory.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        // Act
        StartOnlinePaymentCommand payment = new()
        {
            RegistrationToExhibitionId = registrationToExhibitionId.Value,
            SuccessUrl = "https://www.kocky.cz/success",
            Currency = Currency.Czk,
            CancelUrl = "https://www.kocky.cz/cancel"
        };
        await SendAsync(payment);

        // Assert
        RegistrationToExhibitionDto registration =
            await SendAsync(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value
            });

        registration.PaymentInfo?.PaymentType.Should().Be(PaymentType.PayOnlineByCard);
        registration.PaymentInfo?.Currency.Should().Be(Currency.Czk);
        registration.PaymentInfo?.SessionId.Should().NotBeNullOrEmpty();
        registration.PaymentInfo?.PaymentCompletedDate.Should().BeNull();
        registration.PaymentInfo?.Amount.Should().BeGreaterThan(0);
        registration.PaymentInfo?.PaymentIntentId.Should().BeNullOrEmpty();
        registration.PaymentInfo?.PaymentRequestDate.ToUniversalTime().Should()
            .BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(2000));
    }
}
