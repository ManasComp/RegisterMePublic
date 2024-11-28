#region

using RegisterMe.Application.Cages;
using RegisterMe.Application.CatRegistrations;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Utils;
using RegisterMe.Application.Exhibitions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Organizations;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Pricing;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Services.Converters;

public class InvoiceDataProvider(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IExhibitionService exhibitionService,
    IPricingFacade pricingFacade,
    ICatRegistrationService catRegistrationService,
    IOrganizationService organizationService,
    TimeProvider timeProvider) : IInvoiceDataProvider
{
    public async Task<List<InvoiceModel>> GetInvoiceModel(int catRegistrationId, string serverUrl)
    {
        CatRegistrationDto catRegistration = await catRegistrationService.GetCatRegistrationById(catRegistrationId);
        RegistrationToExhibitionDto registrationToExhibition =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(catRegistration
                .RegistrationToExhibitionId);
        BriefExhibitionDto briefExhibition =
            await exhibitionService.GetExhibitionById(registrationToExhibition.ExhibitionId);
        RegistrationToExhibitionPrice price = await pricingFacade.GetPrice(registrationToExhibition.Id);
        List<AdvertisementDto> advertisements =
            await exhibitionService.GetAdvertisementsByExhibitionId(registrationToExhibition.ExhibitionId);
        OrganizationDto organization =
            await organizationService.GetOrganizationByIdAsync(briefExhibition.OrganizationId);
        List<List<CatDayDto>> catDays = GetValue(catRegistration);
        List<ExhibitionDayDto> exhibitionDays =
            await exhibitionService.GetExhibitionDayByExhibitionId(registrationToExhibition.ExhibitionId);

        return catDays.Select((catDayList, index) => GenerateData(catRegistration, registrationToExhibition,
                briefExhibition,
                price, advertisements, organization, serverUrl, catDayList, exhibitionDays, catDays.Count, index + 1))
            .ToList();
    }

    private List<List<CatDayDto>> GetValue(CatRegistrationDto catRegistration)
    {
        List<List<CatDayDto>> catDays = [];
        ListComparer<string> comparer = new();
        foreach (CatDayDto catDay in catRegistration.CatDays)
        {
            if (catDays.Count == 0)
            {
                catDays.Add([catDay]);
                continue;
            }

            bool added = false;
            IEnumerable<List<CatDayDto>> differentDays = catDays.Where(day =>
                SamePersonCages(day) && SameRentedCages(day) &&
                comparer.Equals(day[0].GroupsIds, catDay.GroupsIds));
            foreach (List<CatDayDto> day in differentDays)
            {
                day.Add(catDay);
                added = true;
                break;
            }

            if (!added)
            {
                catDays.Add([catDay]);
            }

            continue;

            bool SameRentedCages(List<CatDayDto> day)
            {
                return (day[0].RentedCageTypeId == null && catDay.RentedCageTypeId == null) ||
                       day[0].RentedCageTypeId?.Equals(catDay.RentedCageTypeId) == true;
            }

            bool SamePersonCages(List<CatDayDto> day)
            {
                return (day[0].Cage == null && catDay.Cage == null) || day[0].Cage?.Equals(catDay.Cage) == true;
            }
        }

        return catDays;
    }

    private (string? FeeName, decimal? Price) GetCatRegistrationFeeAndPrice(
        RegistrationToExhibitionPrice registrationToExhibitionPrice, int catRegistrationId, int index,
        Currency? currency)
    {
        if (currency == null)
        {
            return (null, null);
        }

        FeeRecord? fee = registrationToExhibitionPrice.CatRegistrationPrices
            .Where(x => x.CatRegistrationId == catRegistrationId)
            .Select(x => x.CatRegistrationFees.Count > index ? x.CatRegistrationFees[index] : null)
            .SingleOrDefault();

        if (fee == null)
        {
            return (null, null);
        }

        return (fee.FeeName, fee.Price.GetPriceForCurrency(currency.Value));
    }

    private InvoiceModel GenerateData(
        CatRegistrationDto catRegistration,
        RegistrationToExhibitionDto registrationToExhibition,
        BriefExhibitionDto briefExhibition,
        RegistrationToExhibitionPrice price,
        List<AdvertisementDto> advertisements,
        OrganizationDto organizationDto,
        string serverUrl,
        List<CatDayDto> catDays,
        List<ExhibitionDayDto> exhibitionDays,
        int totalList,
        int actualList)
    {
        Currency? currency = registrationToExhibition.PaymentInfo?.Currency;

        CreateExhibitedCatDto? cat = catRegistration.ExhibitedCat;
        CreateLitterDto? litter = catRegistration.Litter;
        BreederDto? breeder = cat?.Breeder ?? litter?.Breeder;
        FatherDto? father = cat?.Father ?? litter?.Father;
        MotherDto? mother = cat?.Mother ?? litter?.Mother;
        PersonRegistrationDto personReg = registrationToExhibition.PersonRegistration;
        PaymentInfoDto? paymentInfo = registrationToExhibition.PaymentInfo;

        return new InvoiceModel
        {
            NameOfBreedingStation = litter?.NameOfBreedingStation,
            PassOfOrigin = litter?.PassOfOrigin,
            CatBreed = cat?.Breed ?? litter?.Breed,
            IsLitter = cat == null,
            Url = serverUrl,
            Id = registrationToExhibition.Id,
            NOfCatsAndLitters = registrationToExhibition.CatRegistrationIds.Count,
            TotalListsForCat = totalList,
            ActualList = actualList,
            Currency = currency,
            DateSend = paymentInfo?.PaymentRequestDate,
            CatPrice =
                GetPriceForCurrency(price.CatRegistrationPrices
                    .FirstOrDefault(x => x.CatRegistrationId == catRegistration.Id)?.GetPrice()),
            RegistrationPrice = GetPriceForCurrency(price.GTotalPrice),
            DatePaymentAccepted = paymentInfo?.PaymentCompletedDate,
            PaymentType = paymentInfo?.PaymentType,
            ExhibitorPrice = GetPriceForCurrency(price.GExhibitorPrice),
            ExhibitionName = briefExhibition.Name,
            OrganizationName = organizationDto.Name,
            VisitedDays =
                exhibitionDays.Where(x => catDays.Any(catDay => catDay.ExhibitionDayId == x.Id)).Select(x => x.Date)
                    .ToList(),
            Advertisement =
                advertisements.SingleOrDefault(a => a.Id == registrationToExhibition.AdvertisementId)?.Description,
            IsCsch = personReg.IsPartOfCsch,
            CatName = cat?.FullName ?? "Vrh",
            CatEms = cat?.Ems,
            CatGroup = cat?.Group,
            CatColour = cat?.Colour,
            CatPedigreeNumber = cat?.PedigreeNumber,
            CatBorn = cat?.BirthDate ?? litter?.BirthDate,
            CatGender = cat?.Sex,
            IsCastrated = cat?.Neutered ?? false,
            BreederFirstName = breeder?.FirstName,
            BreederLastName = breeder?.LastName,
            BreederCountryName = breeder?.Country,
            BreederSameAsExhibitor = breeder?.BreederIsSameAsExhibitor ?? true,
            FatherName = father?.FullName,
            FatherEms = father?.Ems,
            FatherColour = father?.Colour,
            FatherPedigreeNumber = father?.PedigreeNumber,
            MotherName = mother?.Name,
            MotherEms = mother?.Ems,
            MotherColour = mother?.Colour,
            MotherPedigreeNumber = mother?.PedigreeNumber,
            ExhibitorSurname = personReg.LastName,
            ExhibitorFirstname = personReg.FirstName,
            ExhibitorEmail = personReg.Email,
            ExhibitorPhoneNumber = personReg.PhoneNumber,
            ExhibitorStreet = personReg.Street,
            ExhibitorHouse = personReg.HouseNumber,
            ExhibitorZip = personReg.ZipCode,
            ExhibitorCountry = personReg.Country,
            ExhibitorCity = personReg.City,
            ExhibitorDateOfBirth = personReg.DateOfBirth,
            EOrganizationName = personReg.Organization,
            EMemberNumber = personReg.MemberNumber,
            Note = catRegistration.Note,
            One = ContainsGroupId("1"),
            Two = ContainsGroupId("2"),
            Three = ContainsGroupId("3"),
            Four = ContainsGroupId("4"),
            Five = ContainsGroupId("5"),
            Six = ContainsGroupId("6"),
            Seven = ContainsGroupId("7"),
            Eight = ContainsGroupId("8"),
            Nine = ContainsGroupId("9"),
            Ten = ContainsGroupId("10"),
            Eleven = ContainsGroupId("11"),
            Twelve = ContainsGroupId("12"),
            ThirteenA = ContainsGroupId("13a"),
            ThirteenB = ContainsGroupId("13b"),
            ThirteenC = ContainsGroupId("13c"),
            Fourteen = ContainsGroupId("14"),
            Fifteen = ContainsGroupId("15"),
            Sixteen = ContainsGroupId("16"),
            Seventeen = ContainsGroupId("17"),
            CatFeeOne = "Základ",
            CatPriceOne = GetPriceForCurrency(price.CatRegistrationPrices
                .FirstOrDefault(x => x.CatRegistrationId == catRegistration.Id)?.CatRegistrationPriceWithoutFees),
            CatFeeTwo = GetCatFeeAndPrice(0).FeeName,
            CatPriceTwo = GetCatFeeAndPrice(0).Price,
            CatFeeThree = GetCatFeeAndPrice(1).FeeName,
            CatPriceThree = GetCatFeeAndPrice(1).Price,
            CatFeeFour = GetCatFeeAndPrice(2).FeeName,
            CatPriceFour = GetCatFeeAndPrice(2).Price,
            CatFeeFive = GetCatFeeAndPrice(3).FeeName,
            CatPriceFive = GetCatFeeAndPrice(3).Price,
            CatFeeSix = GetCatFeeAndPrice(4).FeeName,
            CatPriceSix = GetCatFeeAndPrice(4).Price,
            IsRentedCage = catDays.Count > 0 && catDays[0].RentedCageTypeId != null,
            IsDoubleCage =
                catDays.Count > 0 &&
                (catDays[0].RentedCageTypeId?.Unparse().RentedType ??
                 CagesService.GetCageTypeForCage(catDays[0].Cage!)) == RentedType.Double,
            CageLength =
                catDays.Count > 0 ? catDays[0].Cage?.Length ?? catDays[0].RentedCageTypeId?.Unparse().Length : null,
            CageWidth =
                catDays.Count > 0 ? catDays[0].Cage?.Width ?? catDays[0].RentedCageTypeId?.Unparse().Width : null,
            CageHeight =
                catDays.Count > 0 ? catDays[0].Cage?.Height ?? catDays[0].RentedCageTypeId?.Unparse().Height : null,
            ReportGenerated = timeProvider.GetLocalNow(),
            AmountPaid = paymentInfo?.PaymentCompletedDate != null ? paymentInfo.Amount : 0m,
            EmailToOrganization = personReg.EmailToOrganization
        };

        (string? FeeName, decimal? Price) GetCatFeeAndPrice(int index)
        {
            (string? FeeName, decimal? Price) feePrice =
                GetCatRegistrationFeeAndPrice(price, catRegistration.Id, index, currency);
            return (feePrice.FeeName, feePrice.Price);
        }

        decimal? GetPriceForCurrency(MultiCurrencyPrice? priceDetail)
        {
            return currency == null ? null : priceDetail?.GetPriceForCurrency(currency.Value);
        }

        bool ContainsGroupId(string groupId)
        {
            return catDays.Count > 0 && catDays[0].GroupsIds.Contains(groupId);
        }
    }
}
