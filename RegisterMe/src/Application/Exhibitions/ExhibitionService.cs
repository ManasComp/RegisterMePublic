#region

using Newtonsoft.Json;
using RegisterMe.Application.Cages;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Mappings;
using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Common.Utils;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Enums;
using RegisterMe.Application.Services.Groups;
using RegisterMe.Application.Services.Workflows;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Entities.RulesEngine;
using RegisterMe.Domain.Enums;
using RulesEngine.Models;
using TimeProvider = System.TimeProvider;

#endregion

namespace RegisterMe.Application.Exhibitions;

public class ExhibitionService(
    IApplicationDbContext appContext,
    IMapper mapper,
    TimeProvider timeProvider,
    GroupService groupService,
    WorkflowService workflowService)
    : IExhibitionService
{
    public async Task<Result<int>> CreateExhibition(CreateExhibitionDto exhibitionDto,
        CancellationToken cancellationToken = default)
    {
        bool isConfirmed = await appContext.Organizations.Where(x => x.Id == exhibitionDto.OrganizationId)
            .Select(x => x.IsConfirmed).SingleAsync(cancellationToken);
        if (!isConfirmed)
        {
            Result.Failure<int>(Errors.OrganizationNotConfirmedError);
        }

        Exhibition exhibition = mapper.Map<Exhibition>(exhibitionDto);
        appContext.Exhibitions.Add(exhibition);
        await appContext.SaveChangesAsync(cancellationToken);

        CreateExhibitionDaysForGivenExhibition(exhibitionDto.ExhibitionStart, exhibitionDto.ExhibitionEnd,
            exhibition.Id);

        await appContext.SaveChangesAsync(cancellationToken);


        const string paymentTypes = """
                                    [
                                      {
                                        "WorkflowName": "Payment",
                                        "Rules": [
                                          {
                                            "RuleName": "PayByBankTransfer_CZK",
                                            "Expression": "registrationToExhibition.PersonRegistration.IsPartOfCsch"
                                          },
                                          {
                                            "RuleName": "PayOnlineByCard_CZK",
                                            "Expression": "false"
                                          },
                                          {
                                            "RuleName": "PayInPlaceByCache_CZK",
                                            "Expression": "false"
                                          },
                                          {
                                            "RuleName": "PayByBankTransfer_EUR",
                                            "Expression": "false"
                                          },
                                          {
                                            "RuleName": "PayOnlineByCard_EUR",
                                            "Expression": "false"
                                          },
                                          {
                                            "RuleName": "PayInPlaceByCache_EUR",
                                            "Expression": "!registrationToExhibition.PersonRegistration.IsPartOfCsch"
                                          }
                                        ]
                                      }
                                    ]
                                    """;


        PriceTypeWorkflow priceTypeWorkflow =
            (JsonConvert.DeserializeObject<List<Workflow>>(paymentTypes,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None }) ?? [])
            .Select(x => new PriceTypeWorkflow(x, exhibition.Id)).First();


        Result result = await workflowService.CreatePaymentWorkflow(priceTypeWorkflow, cancellationToken);

        return result.IsFailure ? Result.Failure<int>(result.Error) : Result.Success(exhibition.Id);
    }

    public async Task<PaginatedList<ExhibitionDto>> GetExhibitions(int pageNumber, int pageSize,
        ExhibitionsFilterDto? parameters = null)
    {
        IQueryable<Exhibition> exhibitionQuery = appContext.Exhibitions.AsQueryable();

        parameters ??= new ExhibitionsFilterDto
        {
            OrganizationId = null,
            UserId = null,
            SearchString = null,
            ExhibitionRegistrationStatus = null,
            OrganizationPublishStatus = OrganizationPublishStatus.Published
        };
        exhibitionQuery = ApplyFilters(parameters, exhibitionQuery);

        PaginatedList<ExhibitionDto> exhibitions = await exhibitionQuery
            .Include(x => x.Days)
            .Include(x => x.Address)
            .OrderBy(x => x.Id)
            .Select(x => mapper.Map<ExhibitionDto>(x))
            .PaginatedListAsync(pageNumber, pageSize);

        return exhibitions;
    }

    public async Task<Result> DeleteUnpublishedExhibition(int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        Exhibition exhibition = await appContext.Exhibitions
            .Where(x => x.Id == exhibitionId)
            .Include(x => x.Days)
            .ThenInclude(x => x.Prices)
            .Include(x => x.Days)
            .ThenInclude(x => x.CagesForRent)
            .Include(x => x.Address)
            .Include(x => x.Advertisements)
            .Include(x => x.Workflows)
            .Include(x => x.PaymentTypesWorkflow)
            .SingleAsync(cancellationToken);

        if (exhibition.IsPublished)
        {
            return Result.Failure(Errors.CannotDeletePublishedExhibitionError);
        }

        appContext.Prices.RemoveRange(exhibition.Days.SelectMany(x => x.Prices));
        appContext.RentedCages.RemoveRange(exhibition.Days.SelectMany(x => x.CagesForRent));
        appContext.Addresses.Remove(exhibition.Address);

        appContext.Exhibitions.Remove(exhibition);
        await appContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> CancelExhibition(int exhibitionId, CancellationToken cancellationToken = default)
    {
        Exhibition exhibition = await appContext.Exhibitions
            .SingleAsync(x => x.Id == exhibitionId, cancellationToken);
        if (!exhibition.IsPublished)
        {
            return Result.Failure(Errors.CannotCancelUnpublishedExhibitionError);
        }

        exhibition.IsCancelled = true;
        appContext.Exhibitions.Update(exhibition);
        await appContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> PublishExhibition(int exhibitionId, CancellationToken cancellationToken = default)
    {
        Exhibition? exhibition = await appContext.Exhibitions
            .Include(x => x.Organization)
            .Include(x => x.Address)
            .Include(x => x.Advertisements)
            .Include(x => x.Days)
            .ThenInclude(x => x.Prices)
            .ThenInclude(x => x.Groups)
            .SingleOrDefaultAsync(x => x.Id == exhibitionId, cancellationToken);


        Guard.Against.NotFound(exhibitionId, exhibition);

        if (exhibition.IsPublished)
        {
            return Result.Failure(Errors.IsAlreadyPublishedAdvertisementError);
        }

        if (exhibition.Days.Count == 0)
        {
            return Result.Failure(Errors.NoDaysError);
        }

        if (!exhibition.Days.SelectMany(x => x.Prices).Any())
        {
            return Result.Failure(Errors.NoPricingError);
        }

        if (!exhibition.Advertisements.Where(x => x.IsDefault).Any())
        {
            return Result.Failure(Errors.NoDefaultAdvertisementsError);
        }

        if (!exhibition.Organization.IsConfirmed)
        {
            return Result.Failure(Errors.OrganizationNotConfirmedError);
        }

        exhibition.IsPublished = true;
        appContext.Exhibitions.Update(exhibition);
        await appContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    public Task<BriefExhibitionDto> GetExhibitionById(int exhibitionId, CancellationToken cancellationToken = default)
    {
        Exhibition? exhibition = appContext.Exhibitions.AsQueryable()
            .Include(x => x.Address)
            .Include(x => x.Days)
            .ThenInclude(x => x.Prices)
            .ThenInclude(x => x.Groups)
            .FirstOrDefault(x => x.Id == exhibitionId);

        Guard.Against.NotFound(exhibitionId, exhibition);

        BriefExhibitionDto model = mapper.Map<BriefExhibitionDto>(exhibition);
        model.Status = GetExhibitionStatus(model);

        return Task.FromResult(model);
    }

    public async Task<List<AdvertisementDto>> GetAdvertisementsByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        List<AdvertisementDto> advertisements = await appContext.Advertisements.AsQueryable()
            .Where(x => x.ExhibitionId == exhibitionId)
            .Select(x => mapper.Map<AdvertisementDto>(x))
            .ToListAsync(cancellationToken);

        return advertisements;
    }

    public async Task<Result<int>> CreateAdvertisement(UpsertAdvertisementDto advertisementDto, int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        Advertisement advertisement = mapper.Map<Advertisement>(advertisementDto);
        advertisement.ExhibitionId = exhibitionId;

        var advertisements = appContext.Advertisements
            .Where(x => x.ExhibitionId == advertisement.ExhibitionId)
            .Select(x => new { descriptionn = x.Description, isDefault = x.IsDefault })
            .ToHashSet();
        if (advertisements.Select(x => x.descriptionn).Contains(advertisementDto.Description))
        {
            return Result.Failure<int>(Errors.AdvertisementWithThisDescriptionAlreadyExistsError);
        }

        if (advertisements.Select(x => x.isDefault).Contains(true) && advertisementDto.IsDefault)
        {
            return Result.Failure<int>(Errors.DefaultAdvertisementAlreadyExistsError);
        }

        appContext.Advertisements.Add(advertisement);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success(advertisement.Id);
    }

    public async Task<List<BigPriceDto>> GetPricesGroupsByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        List<SmallPriceDto> prices = await appContext.Prices
            .Where(x => x.ExhibitionDays.Any(y => y.ExhibitionId == exhibitionId))
            .Include(x => x.Groups)
            .Include(x => x.ExhibitionDays)
            .Select(x => mapper.Map<SmallPriceDto>(x))
            .ToListAsync(cancellationToken);

        List<BigPriceDto> groupedPrices = prices
            .GroupBy(x => x.Groups.OrderBy(y => new GroupId(y)).ToList(), new ListComparer<string>())
            .Select(g => new BigPriceDto
            {
                Groups = string.Join(", ", g.Key),
                PriceIds = string.Join(",", g.Select(x => x.Id)),
                Prices = g.ToList(),
                ExhibitionId = exhibitionId
            })
            .ToList();

        return groupedPrices;
    }

    public async Task<Result<BigPriceDto>> GetPricesGroupsById(string groupsId,
        CancellationToken cancellationToken = default)
    {
        HashSet<int> pricesId = CagesService.FromGroupIdToIds(groupsId).ToHashSet();
        List<Price> prices1 = await appContext.Prices
            .Where(x => pricesId.Contains(x.Id))
            .Include(x => x.Groups)
            .Include(x => x.ExhibitionDays)
            .ToListAsync(cancellationToken);

        List<(SmallPriceDto smallprice, int exhibitionId)> prices;

        if (prices1.Count == 0)
        {
            prices = [];
        }
        else
        {
            prices = prices1.Select(x =>
                (mapper.Map<SmallPriceDto>(x), x.ExhibitionDays.First().ExhibitionId)).ToList();
        }

        if (prices.Count != pricesId.Count)
        {
            return Result.Failure<BigPriceDto>(Errors.PricesNotFoundError);
        }

        if (prices.Select(x => x.exhibitionId).Distinct().Count() > 1)
        {
            return Result.Failure<BigPriceDto>(Errors.PricesAreFromDifferentExhibitionsError);
        }

        int exhibitionId = prices.First().exhibitionId;

        IEnumerable<SmallPriceDto> grPrices = prices.Select(x => x.smallprice);
        BigPriceDto groupedPrices = grPrices
            .GroupBy(x => x.Groups.OrderBy(y => new GroupId(y)).ToList(), new ListComparer<string>())
            .Select(g => new BigPriceDto
            {
                Groups = string.Join(",", g.Key),
                PriceIds = string.Join(",", g.Select(x => x.Id)),
                Prices = g.ToList(),
                ExhibitionId = exhibitionId
            })
            .Single();

        return Result.Success(groupedPrices);
    }


    public async Task<List<DatabaseGroupDto>> GetGroupsCatRegistrationCanBeRegisteredIn(
        LitterOrExhibitedCatDto catRegistration,
        int exhibitionDayId)
    {
        ExhibitionDay exhibitionDay = await appContext.ExhibitionDays
            .Where(x => x.Id == exhibitionDayId)
            .Include(x => x.Prices)
            .ThenInclude(x => x.Groups)
            .SingleAsync();

        List<GroupDto> groupsYouCanRegisterTo = exhibitionDay.Prices.SelectMany(x => x.Groups).Select(x => x.GroupId)
            .Distinct()
            .Select(groupService.GetGroupById)
            .Where(x => x
                .Filter(
                    new GroupInitializer.FilterParameter(exhibitionDay.Date, catRegistration)))
            .ToList();

        List<DatabaseGroupDto> databaseGroups = groupsYouCanRegisterTo
            .Select(x => new DatabaseGroupDto { GroupId = x.GroupId, Name = x.Name }).ToList();
        return databaseGroups;
    }

    public async Task<Result> UpdateExhibition(UpdateExhibitionDto briefExhibitionDto,
        CancellationToken cancellationToken = default)
    {
        Exhibition? exhibition = appContext.Exhibitions
            .Include(x => x.Days)
            .Include(x => x.Address)
            .SingleOrDefault(x => x.Id == briefExhibitionDto.Id);

        Guard.Against.NotFound(briefExhibitionDto.Id, exhibition);

        if (exhibition.IsCancelled)
        {
            return Result.Failure(Errors.CannotUpdateCancelledExhibitionError);
        }

        exhibition.Address = mapper.Map(briefExhibitionDto.Address, exhibition.Address);
        exhibition.RegistrationStart = briefExhibitionDto.RegistrationStart;
        exhibition.RegistrationEnd = briefExhibitionDto.RegistrationEnd;
        exhibition.BankAccount = briefExhibitionDto.BankAccount;
        exhibition.Iban = briefExhibitionDto.Iban;
        exhibition.Phone = briefExhibitionDto.Phone;
        exhibition.Email = briefExhibitionDto.Email;
        exhibition.Url = briefExhibitionDto.Url;
        exhibition.Name = briefExhibitionDto.Name;
        exhibition.Description = briefExhibitionDto.Description;
        exhibition.DeleteNotFinishedRegistrationsAfterHours =
            briefExhibitionDto.DeleteNotFinishedRegistrationsAfterHours;

        List<DateOnly> days = exhibition.Days.Select(x => x.Date).OrderByDescending(x => x).ToList();
        DateOnly exhibitionEnd = days.First();
        DateOnly exhibitionStart = days.Last();

        if (briefExhibitionDto.ExhibitionEnd != exhibitionEnd || briefExhibitionDto.ExhibitionStart != exhibitionStart)
        {
            CreateExhibitionDaysForGivenExhibition(briefExhibitionDto.ExhibitionStart, briefExhibitionDto.ExhibitionEnd,
                briefExhibitionDto.Id);
        }

        appContext.Exhibitions.Update(exhibition);
        await appContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<MultiCurrencyPrice> GetAdvertisementPrice(int advertisementId,
        CancellationToken cancellationToken = default)
    {
        Advertisement advertisement = await appContext.Advertisements
            .SingleAsync(x => x.Id == advertisementId, cancellationToken);

        return new MultiCurrencyPrice(advertisement.Amounts.Where(x => x.Currency == Currency.Czk).Single().Amount,
            advertisement.Amounts.Where(x => x.Currency == Currency.Eur).Single().Amount);
    }

    public async Task<Result> DeleteAdvertisement(int advertisementId, CancellationToken cancellationToken = default)
    {
        Advertisement advertisement = await appContext.Advertisements
            .SingleAsync(x => x.Id == advertisementId, cancellationToken);

        appContext.Advertisements.Remove(advertisement);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UpdateAdvertisement(UpsertAdvertisementDto advertisementDto, int advertisementId,
        CancellationToken cancellationToken = default)
    {
        Advertisement advertisement = await appContext.Advertisements
            .SingleAsync(x => x.Id == advertisementId, cancellationToken);

        var advertisements = appContext.Advertisements
            .Where(x => x.ExhibitionId == advertisement.ExhibitionId)
            .Select(x => new { descriptionn = x.Description, isDefault = x.IsDefault })
            .ToHashSet();
        if (advertisement.Description != advertisementDto.Description &&
            advertisements.Select(x => x.descriptionn).Contains(advertisementDto.Description))
        {
            return Result.Failure<int>(Errors.AdvertisementWithThisDescriptionAlreadyExistsError);
        }

        if (!advertisement.IsDefault && advertisements.Select(x => x.isDefault).Contains(true) &&
            advertisementDto.IsDefault)
        {
            return Result.Failure<int>(Errors.DefaultAdvertisementAlreadyExistsError);
        }

        appContext.Amounts.RemoveRange(advertisement.Amounts);
        appContext.Advertisements.Update(mapper.Map(advertisementDto, advertisement));
        await appContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<AdvertisementDto> GetAdvertisementById(int advertisementId,
        CancellationToken cancellationToken = default)
    {
        Advertisement? advertisement = await appContext.Advertisements
            .FindAsync([advertisementId], cancellationToken);

        Guard.Against.NotFound(advertisementId, advertisement);

        AdvertisementDto model = mapper.Map<AdvertisementDto>(advertisement);

        return model;
    }

    public async Task<List<ExhibitionDayDto>> GetExhibitionDayByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        List<ExhibitionDayDto> exhibitionDays = await appContext.ExhibitionDays
            .Include(x => x.Exhibition)
            .Include(x => x.CagesForRent)
            .Where(x => x.ExhibitionId == exhibitionId)
            .Select(x => mapper.Map<ExhibitionDayDto>(x))
            .ToListAsync(cancellationToken);

        return exhibitionDays;
    }


    public async Task<List<DatabaseGroupDto>> GetExhibitionGroupsThatAreNotFullyRegistered(int exhibitionId)
    {
        List<DatabaseGroupDto> groups = await appContext.Groups
            .Where(x => x.Prices.SelectMany(y => y.ExhibitionDays).All(y => y.ExhibitionId != exhibitionId))
            .Select(x => new DatabaseGroupDto { GroupId = x.GroupId, Name = x.Name })
            .ToListAsync();
        return groups;
    }

    public async Task<Result<string>> CreatePrices(List<string> groupsIds, int exhibitionId, List<PriceDays> priceDays,
        CancellationToken cancellationToken = default)
    {
        bool isFromSameExhibition = await AllDaysAreFromGivenExhibition(exhibitionId, priceDays, cancellationToken);
        if (!isFromSameExhibition)
        {
            return Result.Failure<string>(Errors.ExhibitionDaysAreNotFromSameExhibitionError);
        }

        var actualPrices = await appContext.Prices
            .Include(x => x.ExhibitionDays)
            .Where(x => x.ExhibitionDays.Any(y => y.ExhibitionId == exhibitionId))
            .Select(x => new { x.ExhibitionDays, Groups = x.Groups.Select(g => g.GroupId) })
            .ToListAsync(cancellationToken);

        List<CreatePriceDto> prices = [];
        ListComparer<int> comparer = new();
        foreach (CreatePriceDto? price in priceDays.Select(day => new CreatePriceDto
                 {
                     Price = day.Price, Groups = groupsIds, ExhibitionDayIds = day.ExhibitionDayIds
                 }))
        {
            bool priceForGivenDaysAndGroupsAlreadyExists = false;
            foreach (var actualPrice in actualPrices)
            {
                if (priceForGivenDaysAndGroupsAlreadyExists)
                {
                    break;
                }

                bool sameExhibitionDays = comparer.Equals(actualPrice.ExhibitionDays.Select(y => y.Id).ToList(),
                    price.ExhibitionDayIds);
                bool containsGroup = actualPrice.Groups.ToHashSet().Except(price.Groups).Count() !=
                                     actualPrice.Groups.Count();

                if (sameExhibitionDays && containsGroup)
                {
                    priceForGivenDaysAndGroupsAlreadyExists = true;
                }
            }

            if (priceForGivenDaysAndGroupsAlreadyExists)
            {
                return Result.Failure<string>(Errors.PriceAlreadyExistsError);
            }

            prices.Add(price);
        }

        List<Price> pricesToAdd = prices.Select(x => new Price
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = x.Price.PriceCzk },
                new Amounts { Currency = Currency.Eur, Amount = x.Price.PriceEur }
            ],
            Groups = appContext.Groups.Where(g => x.Groups.Contains(g.GroupId)).ToList(),
            ExhibitionDays = appContext.ExhibitionDays.Where(ed => x.ExhibitionDayIds.Contains(ed.Id)).ToList()
        }).ToList();

        await appContext.Prices.AddRangeAsync(pricesToAdd, cancellationToken);
        await appContext.SaveChangesAsync(cancellationToken);

        List<int> ids = pricesToAdd.Select(x => x.Id).ToList();
        return Result.Success(string.Join(",", ids));
    }

    public async Task<string> UpdatePriceGroup(string originalPricesIds, List<string> groupsIds,
        List<PriceDays> priceDays,
        CancellationToken cancellationToken = default)
    {
        // this can be done in more intelligent way - update existing items and create/delete only what is necessary
        Result<BigPriceDto> prices = await GetPricesGroupsById(originalPricesIds, cancellationToken);
        await DeletePriceGroup(originalPricesIds, cancellationToken);
        Result<string> data = await CreatePrices(groupsIds, prices.Value.ExhibitionId, priceDays, cancellationToken);
        return data.Value;
    }

    public async Task<Result> DeletePriceGroup(string priceId, CancellationToken cancellationToken)
    {
        HashSet<int> prices = CagesService.FromGroupIdToIds(priceId).ToHashSet();

        List<Price> price = await appContext.Prices
            .Include(x => x.ExhibitionDays)
            .Where(x => prices.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (price.SelectMany(x => x.ExhibitionDays.Select(y => y.ExhibitionId)).Distinct().Count() > 1)
        {
            return Result.Failure(Errors.PricesAreFromDifferentExhibitionsError);
        }

        foreach (Price p in price)
        {
            p.ExhibitionDays.Clear();
        }

        appContext.Prices.UpdateRange(price);
        await appContext.SaveChangesAsync(cancellationToken);
        appContext.Prices.RemoveRange(price);
        await appContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<List<DatabaseGroupDto>> GetGroupsByGroupId(string priceGroupId)
    {
        HashSet<int> prices = CagesService.FromGroupIdToIds(priceGroupId).ToHashSet();
        List<DatabaseGroupDto> groupsDtos = (await appContext.Prices
                .Where(x => prices.Contains(x.Id))
                .SelectMany(x => x.Groups)
                .Select(x => new DatabaseGroupDto { GroupId = x.GroupId, Name = x.Name })
                .ToListAsync())
            .DistinctBy(x => x.GroupId)
            .OrderBy(x => new GroupId(x.GroupId))
            .ToList();
        return groupsDtos;
    }

    public async Task<List<ExhibitionDayDto>> GetDaysByGroupId(string priceGroupIds,
        CancellationToken cancellationToken = default)
    {
        HashSet<string> pricesIds = priceGroupIds.Split(',').ToHashSet();
        List<ExhibitionDayDto> days = await appContext.Prices
            .Include(x => x.ExhibitionDays)
            .Where(x => pricesIds.Contains(x.Id.ToString()))
            .SelectMany(x => x.ExhibitionDays)
            .Select(x => mapper.Map<ExhibitionDayDto>(x))
            .ToListAsync(cancellationToken);
        return days;
    }

    private void CreateExhibitionDaysForGivenExhibition(DateOnly exhibitionStart, DateOnly exhibitionEnd,
        int exhibitionId)
    {
        appContext.ExhibitionDays.RemoveRange(appContext.ExhibitionDays.Where(x => x.ExhibitionId == exhibitionId));
        for (DateOnly day = exhibitionStart;
             day <= exhibitionEnd;
             day = day.AddDays(1))
        {
            ExhibitionDay exhibitionDay = new() { Date = day, ExhibitionId = exhibitionId };
            appContext.ExhibitionDays.Add(exhibitionDay);
        }
    }

    private ExhibitionStatus GetExhibitionStatus(BriefExhibitionDto briefExhibitionDto)
    {
        DateOnly now = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);

        if (now < briefExhibitionDto.RegistrationStart)
        {
            return ExhibitionStatus.BeforeRegistration;
        }

        if (now <= briefExhibitionDto.RegistrationEnd && now >= briefExhibitionDto.RegistrationStart)
        {
            return ExhibitionStatus.RegistrationInProgress;
        }

        if (now < briefExhibitionDto.ExhibitionStart && now > briefExhibitionDto.RegistrationEnd)
        {
            return ExhibitionStatus.AfterRegistration;
        }

        if (now <= briefExhibitionDto.ExhibitionEnd && now >= briefExhibitionDto.ExhibitionStart)
        {
            return ExhibitionStatus.ExhibitionInProgress;
        }

        if (now > briefExhibitionDto.ExhibitionEnd)
        {
            return ExhibitionStatus.Finished;
        }

        throw new InvalidDatabaseStateException("Exhibition status not found");
    }

    private IQueryable<Exhibition> ApplyFilters(ExhibitionsFilterDto exhibitionsFilter,
        IQueryable<Exhibition> exhibitionQuery)
    {
        exhibitionQuery = exhibitionsFilter.OrganizationPublishStatus switch
        {
            OrganizationPublishStatus.Published => exhibitionQuery.Where(x => x.IsPublished),
            OrganizationPublishStatus.NotPublished => exhibitionQuery.Where(x => !x.IsPublished),
            OrganizationPublishStatus.All => exhibitionQuery,
            _ => throw new InvalidOperationException("Invalid OrganizationPublishStatus")
        };

        if (exhibitionsFilter.OrganizationId != null)
        {
            exhibitionQuery =
                exhibitionQuery.Where(exhibition => exhibition.OrganizationId == exhibitionsFilter.OrganizationId);
        }

        if (exhibitionsFilter.UserId != null)
        {
            HashSet<int> exhibitionIds = appContext.RegistrationsToExhibition
                .Where(x => x.Exhibitor.AspNetUserId == exhibitionsFilter.UserId)
                .Select(x => x.ExhibitionId)
                .ToHashSet();

            exhibitionQuery = exhibitionQuery.Where(exhibition => exhibitionIds.Contains(exhibition.Id));
        }

        if (!string.IsNullOrWhiteSpace(exhibitionsFilter.SearchString))
        {
            string searchString = exhibitionsFilter.SearchString.ToLower();
            exhibitionQuery = exhibitionQuery
                .Where(
                    exhibition => exhibition.Name.ToLower().Contains(searchString)
                                  || exhibition.Description.ToLower().Contains(searchString)
                                  || exhibition.Address.StreetAddress.ToLower().Contains(searchString)
                                  || exhibition.Organization.Name.ToLower().Contains(searchString)
                );
        }

        if (exhibitionsFilter.ExhibitionRegistrationStatus == null)
        {
            return exhibitionQuery;
        }

        {
            DateOnly now = DateOnly.FromDateTime(timeProvider.GetLocalNow().ToUniversalTime().DateTime);
            exhibitionQuery = exhibitionsFilter.ExhibitionRegistrationStatus switch
            {
                ExhibitionRegistrationStatus.All => exhibitionQuery,
                ExhibitionRegistrationStatus.Future => exhibitionQuery.Where(x => x.RegistrationStart > now),
                ExhibitionRegistrationStatus.Old => exhibitionQuery.Where(x => x.RegistrationEnd < now),
                ExhibitionRegistrationStatus.CanRegisterTo => exhibitionQuery.Where(x =>
                    x.RegistrationStart <= now && x.RegistrationEnd >= now),
                _ => throw new InvalidOperationException("Invalid ExhibitionRegistrationStatus")
            };
        }

        return exhibitionQuery;
    }

    private async Task<bool> AllDaysAreFromGivenExhibition(int exhibitionId, List<PriceDays> priceDays,
        CancellationToken cancellationToken)
    {
        List<int> exhibitionDayIds = priceDays.SelectMany(pd => pd.ExhibitionDayIds).Distinct().ToList();

        List<int> exhibitionDays = await appContext.ExhibitionDays
            .Where(x => x.ExhibitionId == exhibitionId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        bool existsDayThatIsNotInGivenExhibition = exhibitionDayIds.Except(exhibitionDays).Any();

        return !existsDayThatIsNotInGivenExhibition;
    }
}
