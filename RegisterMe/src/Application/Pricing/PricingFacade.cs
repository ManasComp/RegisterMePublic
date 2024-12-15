#region

using Microsoft.Extensions.Configuration;
using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.CatRegistrations;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Extensions;
using RegisterMe.Application.Exhibitions;
using RegisterMe.Application.Exhibitors;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Pricing.Enums;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.Services.Enums;
using RegisterMe.Application.Services.Workflows;
using RegisterMe.Application.System;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.Pricing;

public class PricingFacade(
    ICatRegistrationService catRegistrationService,
    IRegistrationToExhibitionService registrationToExhibitionService,
    WorkflowService workflowService,
    IExhibitorService exhibitorService,
    IExhibitionService exhibitionService,
    ICagesService cagesService,
    IConfiguration configuration,
    ISystemService systemService)
    : IPricingFacade
{
    public async Task<RegistrationToExhibitionPrice> GetPrice(int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        RegistrationToExhibitionDto registrationToExhibition =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(
                registrationToExhibitionId, cancellationToken);

        List<MiddleCatRegistrationDto> catRegistrations =
            await catRegistrationService.GetCatRegistrationsByRegistrationToExhibitionId(registrationToExhibitionId,
                cancellationToken);

        List<Tuple<MiddleCatRegistrationDto, MultiCurrencyPrice>>
            catRegistrationsWithPrices = [];
        foreach (MiddleCatRegistrationDto? catRegistration in catRegistrations)
        {
            MultiCurrencyPrice price =
                await catRegistrationService.GetCatRegistrationPrice(catRegistration.Id, cancellationToken);
            catRegistrationsWithPrices.Add(
                new Tuple<MiddleCatRegistrationDto, MultiCurrencyPrice>(catRegistration,
                    price));
        }

        var catRegistrationsWIthIndexes = catRegistrationsWithPrices
            .Select(x => new { validated = x.Item1, price = x.Item2 })
            .OrderBy(x => x.price.PriceCzk)
            .Select((value, index) => new { cat = value, index });

        List<CatRegistrationStructure> catRegistrationStructure = [];
        foreach (var item in catRegistrationsWIthIndexes)
        {
            List<RentedTypeEntity> rentedTypes =
                await cagesService.GetRentedTypes(registrationToExhibition.ExhibitionId,
                    registrationToExhibitionId,
                    cancellationToken);

            List<PersonCageWithCatDays> allCagesFromRegistrationToExhibition =
                await cagesService.GetPersonCagesByRegistrationToExhibitionId(registrationToExhibitionId,
                    cancellationToken);

            CatRegistrationStructure structure = new()
            {
                SortedAscendingByPriceIndex = item.index,
                NumberOfVisitedDays = item.cat.validated.CatDays.Count,
                OriginalPrice = item.cat.price,
                CatRegistrationId = item.cat.validated.Id,
                CatName = item.cat.validated.ExhibitedCat?.Name ?? "Vrh",
                RentedCageTypesIds = item.cat.validated.CatDays.Select(x => x.RentedCageTypeId).Distinct()
                    .Where(x => x.HasValue).Select(x => x!.Value).ToList(),
                CountOfUsedCagesPerRentedCageType =
                    GetCountOfUsedCagesPerRentedCageType(rentedTypes, item.cat.validated.Id),
                OwnCages = GetPersonCages(allCagesFromRegistrationToExhibition, item.cat.validated.Id),
                IsLitter = item.cat.validated.ExhibitedCat == null
            };
            catRegistrationStructure.Add(structure);
        }

        Workflow[] wfr =
            (await workflowService.GetDiscountsByExhibitionId(registrationToExhibition.ExhibitionId, cancellationToken))
            .Select(x => x.ToWorkflow()).ToArray();

        RulesEngine.RulesEngine rulesEngine = new(wfr);
        RegistrationToExhibitionPrice registrationToExhibitionPrice = new()
        {
            RegistrationToExhibitionId = registrationToExhibitionId,
            AdvertisementPrice =
                await exhibitionService.GetAdvertisementPrice(registrationToExhibition.AdvertisementId,
                    cancellationToken),
            CatRegistrationPrices = []
        };


        foreach (CatRegistrationStructure structure in catRegistrationStructure)
        {
            CatRegistrationPrice catRegistrationPrice = new()
            {
                CatRegistrationId = structure.CatRegistrationId,
                CatRegistrationPriceWithoutFees = structure.OriginalPrice,
                CatRegistrationFees = [],
                CatName = structure.CatName
            };

            List<FeeRecord> regigstrationFees =
                await GetFeesForRegistrationToExhibitions(rulesEngine, structure, wfr);

            catRegistrationPrice.CatRegistrationFees.AddRange(regigstrationFees);
            registrationToExhibitionPrice.CatRegistrationPrices.Add(catRegistrationPrice);
        }

        registrationToExhibitionPrice.GExhibitorPrice = registrationToExhibitionPrice.AdvertisementPrice;
        registrationToExhibitionPrice.GTotalPrice = registrationToExhibitionPrice.GExhibitorPrice +
                                                    registrationToExhibitionPrice.CatRegistrationPrices
                                                        .Select(x => x.GetPrice()).Sum();


        return registrationToExhibitionPrice;
    }

    public async Task<string> GetBeneficiaryMessage(int registrationToExhibitionId)
    {
        // follows the standard https://mojebanka.kb.cz/file/cs/format_qr_kb.pdf
        const int maximalLength = 60;
        RegistrationToExhibitionDto registrationToExhibition =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(registrationToExhibitionId);
        ExhibitorAndUserDto exhibitor = await exhibitorService.GetExhibitorById(registrationToExhibition.ExhibitorId);

        int memberNumberLength = exhibitor.MemberNumber.Length;
        if (memberNumberLength >= maximalLength)
        {
            throw new InvalidDataException();
        }

        string message = exhibitor.MemberNumber;
        List<string> potentialBeneficiaryMessagesParts =
            [exhibitor.LastName, exhibitor.FirstName, exhibitor.Email, exhibitor.PhoneNumber];

        string separator = ";";
        foreach (string part in potentialBeneficiaryMessagesParts)
        {
            if (message.Length + separator.Length + part.Length <= maximalLength)
            {
                message += separator + part;
            }
        }

        return message;
    }

    public async Task<List<PaymentTypeWithCurrency>> GetAvailablePaymentTypes(int registrationToExhibitionId)
    {
        bool allowPaymentByCard = systemService.PaymentByCardIsConfigured(configuration);
        RegistrationToExhibitionDto registrationToExhibition =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(registrationToExhibitionId);
        Workflow wfr = await workflowService.GetPaymentTypesByExhibitionId(registrationToExhibition.ExhibitionId) ??
                       throw new Exception("Workflow not found");
        List<PaymentTypeWithCurrency> paymentTypes =
            await workflowService.ExecutePaymentTypesByExhibition(registrationToExhibition, wfr, allowPaymentByCard);
        return paymentTypes;
    }

    private async Task<List<FeeRecord>> GetFeesForRegistrationToExhibitions(RulesEngine.RulesEngine rulesEngine,
        CatRegistrationStructure structure, Workflow[] workflows)
    {
        List<FeeRecord> registrationFees = [];
        foreach (Workflow workflow in workflows)
        {
            ICollection<MultiCurrencyPrice> results =
                await workflowService.ExecuteDiscountWorkflow(rulesEngine, structure, workflow);
            registrationFees.AddRange(results.Select(price =>
                new FeeRecord { FeeName = workflow.WorkflowName, Price = price }));
        }

        return registrationFees;
    }

    private Dictionary<CagesAndCatsEnum, int> GetCountOfUsedCagesPerRentedCageType(List<RentedTypeEntity> rentedTypes,
        int catRegistrationId)
    {
        Dictionary<CagesAndCatsEnum, int> countOfUsedCagesPerRentedCageType = new()
        {
            { CagesAndCatsEnum.SingleCageSingleCat, 0 },
            { CagesAndCatsEnum.DoubleCageSingleCat, 0 },
            { CagesAndCatsEnum.DoubleCageMultipleCats, 0 }
        };

        IEnumerable<RentedTypeEntity> rentedTypesForGivenCatRegistration = rentedTypes
            .Where(x => x.CatDays.Any(catDay =>
                catDay.CatRegistrationId == catRegistrationId));

        foreach (RentedTypeEntity x in rentedTypesForGivenCatRegistration)
        {
            List<int> exhibitionDaysUsedInCatRegistration = x.CatDays
                .Where(catDay => catDay.CatRegistrationId == catRegistrationId)
                .Select(catDay => catDay.ExhibitionDayId).Distinct().ToList();

            foreach (int count in
                     exhibitionDaysUsedInCatRegistration.Select(day =>
                         x.CatDays.Count(catDay => catDay.ExhibitionDayId == day)))
            {
                switch (x.RentedType)
                {
                    case RentedType.Double when count == 1:
                        countOfUsedCagesPerRentedCageType[CagesAndCatsEnum.DoubleCageSingleCat]++;
                        break;
                    case RentedType.Double when count > 1:
                        countOfUsedCagesPerRentedCageType[CagesAndCatsEnum.DoubleCageMultipleCats]++;
                        break;
                    case RentedType.Single:
                        countOfUsedCagesPerRentedCageType[CagesAndCatsEnum.SingleCageSingleCat]++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        return countOfUsedCagesPerRentedCageType;
    }

    private List<PricingCage> GetPersonCages(List<PersonCageWithCatDays> allCages,
        int catRegistrationId)
    {
        IEnumerable<PersonCageWithCatDays> cagesRentedByCat = allCages.Where(cage =>
            cage.CatDays.Any(day =>
                day.CatRegistration.CatDays.Any(catDay => catDay.CatRegistrationId == catRegistrationId)));
        List<PricingCage> pricingCages = [];

        foreach (PersonCageWithCatDays rentedCage in cagesRentedByCat)
        {
            List<int> exhibitionDays = rentedCage.CatDays
                .Where(day => day.CatRegistrationId == catRegistrationId)
                .Select(day => day.ExhibitionDayId)
                .Distinct()
                .ToList();

            foreach (PricingCage? pricingCage in exhibitionDays
                         .Select(exhibitionDay => rentedCage.CatDays.Count(day => day.ExhibitionDayId == exhibitionDay))
                         .Select(catCount => AddOrUpdatePricingCage(rentedCage, catCount, pricingCages))
                         .OfType<PricingCage>())
            {
                pricingCages.Add(pricingCage);
            }
        }

        return pricingCages;
    }

    private static PricingCage? AddOrUpdatePricingCage(PersonCageWithCatDays rentedCage, int catCount,
        List<PricingCage> pricingCages)
    {
        CageDto cage = rentedCage.PersonCageWithId.PersonCage;
        OwnCageEnum type = catCount == 1 ? OwnCageEnum.SingleCat : OwnCageEnum.MultipleCats;

        foreach (PricingCage pricingCage in pricingCages.Where(pricingCage => pricingCage.Length == cage.Length &&
                                                                              pricingCage.Width == cage.Width &&
                                                                              pricingCage.Height == cage.Height &&
                                                                              pricingCage.Type == type))
        {
            pricingCage.NumberOfDays++;
            return null;
        }

        return new PricingCage
        {
            NumberOfDays = 1,
            Length = cage.Length,
            Width = cage.Width,
            Height = cage.Height,
            Type = type
        };
    }
}
