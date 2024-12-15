#region

using Bogus;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.Services.Converters;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.UnitTests.Pricing;

public class RegistrationToExhibitionMock : IRegistrationToExhibitionService
{
    public Task<Result> ChangeAdvertisement(int advertisementId, int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateSendNotifications(List<SimpleTypedEmail> emails,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RequestDelayedPayment(int registrationToExhibitionId, PaymentType paymentType,
        Currency currency,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TypedEmail>> GetTemporaryRegistrationsEmails(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> FinishOnlinePayment(int registrationToExhibitionId, string sessionId, string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> FinishDelayedPayment(int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> StartOnlinePayment(int registrationToExhibitionId, string sessionId, Currency currency,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteRegistration(int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<TypedEmail>>> DeleteTemporaryRegistrations(
        int? exhibitionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> CreateRegistrationToExhibition(
        CreateRegistrationToExhibitionDto registrationToExhibitionDto,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<RegistrationToExhibitionDto>> GetRegistrationsToExhibitionByExhibitorId(int exhibitorId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasValidEms(int registrationToExhibitionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<RegistrationToExhibitionDto>> GetRegistrationsToExhibitionByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<RegistrationToExhibitionDto?> GetRegistrationToExhibitionByExhibitorIdAndExhibitionId(int exhibitionId,
        int exhibitorId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<RegistrationToExhibitionDto> GetRegistrationToExhibitionById(int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasActiveRegistrations(int exhibitionId, string userId, bool? paid,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasDrafts(int exhibitionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> BalanceThePayment(int registrationToExhibitionId, decimal price,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class InvoiceDataProviderMock : IInvoiceDataProvider
{
    public Task<List<InvoiceModel>> GetInvoiceModel(int catRegistrationId, string serverUrl)
    {
        Faker faker = new("cz") { Random = new Randomizer(123) };
        InvoiceModel invoice1 = new()
        {
            NameOfBreedingStation = faker.Company.CompanyName(),
            PassOfOrigin = faker.Address.Country(),
            CatBreed = faker.Commerce.ProductName(),
            Url = faker.Internet.Url(),
            Id = faker.Random.Int(1, 1),
            NOfCatsAndLitters = faker.Random.Int(1, 10),
            TotalListsForCat = faker.Random.Int(1, 5),
            ActualList = faker.Random.Int(1, 5),
            Currency = faker.PickRandom<Currency>(),
            DateSend = faker.Date.RecentOffset(),
            CatPrice = faker.Finance.Amount(),
            RegistrationPrice = faker.Finance.Amount(),
            DatePaymentAccepted = faker.Date.RecentOffset(),
            PaymentType = faker.PickRandom<PaymentType>(),
            ExhibitorPrice = faker.Finance.Amount(),
            ExhibitionName = faker.Company.CompanyName(),
            OrganizationName = faker.Company.CompanyName(),
            VisitedDays = [DateOnly.FromDateTime(faker.Date.Past())],
            Advertisement = faker.Lorem.Sentence(),
            IsCsch = faker.Random.Bool(),
            CatName = faker.Name.FirstName(),
            CatEms = faker.Random.String2(10),
            CatGroup = faker.Random.Int(1, 10),
            CatColour = faker.Commerce.Color(),
            CatPedigreeNumber = faker.Random.String2(10),
            CatBorn = DateOnly.FromDateTime(faker.Date.Past()),
            CatGender = faker.PickRandom<Gender>(),
            IsCastrated = faker.Random.Bool(),
            BreederFirstName = faker.Name.FirstName(),
            BreederLastName = faker.Name.LastName(),
            BreederCountryName = faker.Address.Country(),
            BreederSameAsExhibitor = faker.Random.Bool(),
            IsLitter = faker.Random.Bool(),
            FatherName = faker.Name.FirstName(),
            FatherEms = faker.Random.String2(10),
            FatherColour = faker.Commerce.Color(),
            FatherPedigreeNumber = faker.Random.String2(10),
            MotherName = faker.Name.FirstName(),
            MotherEms = faker.Random.String2(10),
            MotherColour = faker.Commerce.Color(),
            MotherPedigreeNumber = faker.Random.String2(10),
            ExhibitorSurname = faker.Name.LastName(),
            ExhibitorFirstname = faker.Name.FirstName(),
            ExhibitorEmail = faker.Internet.Email(),
            ExhibitorPhoneNumber = faker.Phone.PhoneNumber(),
            ExhibitorStreet = faker.Address.StreetName(),
            ExhibitorHouse = faker.Address.BuildingNumber(),
            ExhibitorZip = faker.Address.ZipCode(),
            ExhibitorCountry = faker.Address.Country(),
            ExhibitorCity = faker.Address.City(),
            ExhibitorDateOfBirth = DateOnly.FromDateTime(faker.Date.Past()),
            EOrganizationName = faker.Company.CompanyName(),
            EMemberNumber = faker.Random.String2(10),
            Note = faker.Lorem.Sentence(),
            One = faker.Random.Bool(),
            Two = faker.Random.Bool(),
            Three = faker.Random.Bool(),
            Four = faker.Random.Bool(),
            Five = faker.Random.Bool(),
            Six = faker.Random.Bool(),
            Seven = faker.Random.Bool(),
            Eight = faker.Random.Bool(),
            Nine = faker.Random.Bool(),
            Ten = faker.Random.Bool(),
            Eleven = faker.Random.Bool(),
            Twelve = faker.Random.Bool(),
            ThirteenA = faker.Random.Bool(),
            ThirteenB = faker.Random.Bool(),
            ThirteenC = faker.Random.Bool(),
            Fourteen = faker.Random.Bool(),
            Fifteen = faker.Random.Bool(),
            Sixteen = faker.Random.Bool(),
            Seventeen = faker.Random.Bool(),
            CatFeeOne = faker.Commerce.ProductName(),
            CatFeeTwo = faker.Commerce.ProductName(),
            CatFeeThree = faker.Commerce.ProductName(),
            CatFeeFour = faker.Commerce.ProductName(),
            CatFeeFive = faker.Commerce.ProductName(),
            CatFeeSix = faker.Commerce.ProductName(),
            CatPriceOne = faker.Finance.Amount(),
            CatPriceTwo = faker.Finance.Amount(),
            CatPriceThree = faker.Finance.Amount(),
            CatPriceFour = faker.Finance.Amount(),
            CatPriceFive = faker.Finance.Amount(),
            CatPriceSix = faker.Finance.Amount(),
            IsRentedCage = faker.Random.Bool(),
            IsDoubleCage = faker.Random.Bool(),
            CageLength = faker.Random.Int(50, 100),
            CageWidth = faker.Random.Int(50, 100),
            CageHeight = faker.Random.Int(50, 100),
            ReportGenerated = faker.Date.Recent(),
            AmountPaid = faker.Random.Int(1, 1000),
            EmailToOrganization = faker.Internet.Email()
        };
        faker.Random = new Randomizer(321);
        InvoiceModel invoice2 = new()
        {
            NameOfBreedingStation = faker.Company.CompanyName(),
            PassOfOrigin = faker.Address.Country(),
            CatBreed = faker.Commerce.ProductName(),
            Url = faker.Internet.Url(),
            Id = faker.Random.Int(1, 1),
            NOfCatsAndLitters = faker.Random.Int(1, 10),
            TotalListsForCat = faker.Random.Int(1, 5),
            ActualList = faker.Random.Int(1, 5),
            Currency = faker.PickRandom<Currency>(),
            DateSend = faker.Date.RecentOffset(),
            CatPrice = faker.Finance.Amount(),
            RegistrationPrice = faker.Finance.Amount(),
            DatePaymentAccepted = faker.Date.RecentOffset(),
            PaymentType = faker.PickRandom<PaymentType>(),
            ExhibitorPrice = faker.Finance.Amount(),
            ExhibitionName = faker.Company.CompanyName(),
            OrganizationName = faker.Company.CompanyName(),
            VisitedDays = [DateOnly.FromDateTime(faker.Date.Past())],
            Advertisement = faker.Lorem.Sentence(),
            IsCsch = faker.Random.Bool(),
            CatName = faker.Name.FirstName(),
            CatEms = faker.Random.String2(10),
            CatGroup = faker.Random.Int(1, 10),
            CatColour = faker.Commerce.Color(),
            CatPedigreeNumber = faker.Random.String2(10),
            CatBorn = DateOnly.FromDateTime(faker.Date.Past()),
            CatGender = faker.PickRandom<Gender>(),
            IsCastrated = faker.Random.Bool(),
            BreederFirstName = faker.Name.FirstName(),
            BreederLastName = faker.Name.LastName(),
            BreederCountryName = faker.Address.Country(),
            BreederSameAsExhibitor = faker.Random.Bool(),
            IsLitter = faker.Random.Bool(),
            FatherName = faker.Name.FirstName(),
            FatherEms = faker.Random.String2(10),
            FatherColour = faker.Commerce.Color(),
            FatherPedigreeNumber = faker.Random.String2(10),
            MotherName = faker.Name.FirstName(),
            MotherEms = faker.Random.String2(10),
            MotherColour = faker.Commerce.Color(),
            MotherPedigreeNumber = faker.Random.String2(10),
            ExhibitorSurname = faker.Name.LastName(),
            ExhibitorFirstname = faker.Name.FirstName(),
            ExhibitorEmail = faker.Internet.Email(),
            ExhibitorPhoneNumber = faker.Phone.PhoneNumber(),
            ExhibitorStreet = faker.Address.StreetName(),
            ExhibitorHouse = faker.Address.BuildingNumber(),
            ExhibitorZip = faker.Address.ZipCode(),
            ExhibitorCountry = faker.Address.Country(),
            ExhibitorCity = faker.Address.City(),
            ExhibitorDateOfBirth = DateOnly.FromDateTime(faker.Date.Past()),
            EOrganizationName = faker.Company.CompanyName(),
            EMemberNumber = faker.Random.String2(10),
            Note = faker.Lorem.Sentence(),
            One = faker.Random.Bool(),
            Two = faker.Random.Bool(),
            Three = faker.Random.Bool(),
            Four = faker.Random.Bool(),
            Five = faker.Random.Bool(),
            Six = faker.Random.Bool(),
            Seven = faker.Random.Bool(),
            Eight = faker.Random.Bool(),
            Nine = faker.Random.Bool(),
            Ten = faker.Random.Bool(),
            Eleven = faker.Random.Bool(),
            Twelve = faker.Random.Bool(),
            ThirteenA = faker.Random.Bool(),
            ThirteenB = faker.Random.Bool(),
            ThirteenC = faker.Random.Bool(),
            Fourteen = faker.Random.Bool(),
            Fifteen = faker.Random.Bool(),
            Sixteen = faker.Random.Bool(),
            Seventeen = faker.Random.Bool(),
            CatFeeOne = faker.Commerce.ProductName(),
            CatFeeTwo = faker.Commerce.ProductName(),
            CatFeeThree = faker.Commerce.ProductName(),
            CatFeeFour = faker.Commerce.ProductName(),
            CatFeeFive = faker.Commerce.ProductName(),
            CatFeeSix = faker.Commerce.ProductName(),
            CatPriceOne = faker.Finance.Amount(),
            CatPriceTwo = faker.Finance.Amount(),
            CatPriceThree = faker.Finance.Amount(),
            CatPriceFour = faker.Finance.Amount(),
            CatPriceFive = faker.Finance.Amount(),
            CatPriceSix = faker.Finance.Amount(),
            IsRentedCage = faker.Random.Bool(),
            IsDoubleCage = faker.Random.Bool(),
            CageLength = faker.Random.Int(50, 100),
            CageWidth = faker.Random.Int(50, 100),
            CageHeight = faker.Random.Int(50, 100),
            ReportGenerated = faker.Date.Recent(),
            AmountPaid = faker.Random.Int(1, 1000),
            EmailToOrganization = faker.Internet.Email()
        };
        return Task.FromResult(new List<InvoiceModel> { invoice1, invoice2 });
    }
}
