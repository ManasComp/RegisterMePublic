#region

using System.Runtime.Serialization;
using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Cages.Dtos.Combination;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.CatRegistrations.Enums;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Utils;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitors;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.Services.Ems;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.CatRegistrations;

public class CatRegistrationService(
    IApplicationDbContext appContext,
    IMapper mapper,
    ICagesService cagesService,
    IExhibitorService exhibitorService,
    IExhibitionService exhibitionService)
    : ICatRegistrationService
{
    public Task<List<CatModelP>> GetUserLittersNotInExhibition(int registrationToExhibitionId)
    {
        return GetUserCatsOrLittersNotInExhibitionBase(
            registrationToExhibitionId,
            x => x.Litter != null,
            x => x.Litter,
            (x, y) => x.Litter!.BirthDate == y.BirthDate && x.Litter.NameOfBreedingStation == y.NameOfBreedingStation,
            x => new CatModelP(x.Litter!.BirthDate, x.Litter.NameOfBreedingStation, x.Id)
        );
    }

    public Task<List<CatModelP>> GetUserCatsNotInExhibition(int registrationToExhibitionId)
    {
        return GetUserCatsOrLittersNotInExhibitionBase(
            registrationToExhibitionId,
            x => x.ExhibitedCat != null,
            x => x.ExhibitedCat,
            (x, y) => x.ExhibitedCat!.BirthDate == y.BirthDate && x.ExhibitedCat.PedigreeNumber == y.PedigreeNumber,
            x => new CatModelP(x.ExhibitedCat!.BirthDate, x.ExhibitedCat.Name, x.Id)
        );
    }

    /// <inheritdoc />
    public async Task<Result<int>> CreateCatRegistration(CreateCatRegistrationDto createCatRegistrationDto,
        bool isExhibitionOrganizer,
        CancellationToken cancellationToken = default)
    {
        Result validationResult = await Validations(createCatRegistrationDto, isExhibitionOrganizer, cancellationToken);
        if (validationResult.IsFailure)
        {
            return Result.Failure<int>(validationResult.Error);
        }

        CatRegistration catRegistration = new()
        {
            Note = createCatRegistrationDto.Note,
            RegistrationToExhibitionId = createCatRegistrationDto.RegistrationToExhibitionId
        };

        MapExhibitedCatOrLitter(createCatRegistrationDto, catRegistration);

        catRegistration.CatDays = new List<CatDay>();
        appContext.CatRegistrations.Add(catRegistration);
        await appContext.SaveChangesAsync(cancellationToken);

        List<CatDay> catDays = [];
        foreach (CreateCatDayDto? catDayDto in createCatRegistrationDto.CatDays)
        {
            Result<int?> cageResult = await cagesService.GetRandomRentedCageTypeIdFromHash(catDayDto.RentedCageTypeId,
                catDayDto.ExhibitionDayId, catRegistration.RegistrationToExhibitionId, cancellationToken);
            if (cageResult.IsFailure)
            {
                return Result.Failure<int>(cageResult.Error);
            }

            CatDay catDay = new()
            {
                RentedCageTypeId = cageResult.Value,
                ExhibitorsCage = catDayDto.ExhibitorsCage,
                CatRegistrationId = catRegistration.Id,
                ExhibitionDayId = catDayDto.ExhibitionDayId,
                Groups = catDayDto.GroupsIds.Select(x => appContext.Groups.Find(x)!).ToList()
            };
            if (catDayDto.Cage != null)
            {
                PersonCage personCage = new()
                {
                    Width = catDayDto.Cage.Width,
                    Height = catDayDto.Cage.Height,
                    Length = catDayDto.Cage.Length,
                    RegistrationToExhibitionId = catRegistration.RegistrationToExhibitionId,
                    CatDays = [catDay]
                };
                appContext.Cages.Add(personCage);
            }

            catDays.Add(catDay);
        }

        await appContext.CatDays.AddRangeAsync(catDays, cancellationToken);

        await appContext.SaveChangesAsync(cancellationToken);
        Result validation = await ValidateCagesAsync(createCatRegistrationDto, catRegistration.Id, cancellationToken);
        return validation.IsFailure ? Result.Failure<int>(validation.Error) : Result.Success(catRegistration.Id);
    }

    /// <inheritdoc />
    public async Task<Result> DeleteCatRegistration(int catRegistrationId, bool isExhibitionOrganizer,
        CancellationToken cancellationToken = default)
    {
        CatRegistration? catRegistration =
            await appContext.CatRegistrations.Where(x => x.Id == catRegistrationId)
                .Include(x => x.ExhibitedCat)
                .Include(x => x.Litter)
                .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(catRegistrationId, catRegistration);
        Domain.Entities.RegistrationToExhibition? registrationToExhibition = await appContext.RegistrationsToExhibition
            .Where(x => x.Id == catRegistration.RegistrationToExhibitionId)
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(catRegistration.RegistrationToExhibitionId, registrationToExhibition);
        if (RegistrationToExhibitionService.IsPaid(registrationToExhibition.PaymentInfo) && !isExhibitionOrganizer)
        {
            return Result.Failure<int>(Errors.YouCannotDeleteRegistrationThatHasAlreadyBeenPaidError);
        }

        if (registrationToExhibition is { PaymentInfo: not null, CatRegistrations.Count: 1 })
        {
            return Result.Failure<int>(Errors.DeleteLastCatRegistrationWithPaymentError);
        }

        HashSet<int> catDaysHashSet = [..catRegistration.CatDays.Select(x => x.Id)];
        foreach (CatDay catDay in catRegistration.CatDays)
        {
            if (catDay.Cage == null)
            {
                continue;
            }

            PersonCage? cages = await appContext.Cages
                .Where(x => x.Id == catDay.ExhibitorsCage)
                .Include(x => x.CatDays)
                .SingleOrDefaultAsync(cancellationToken);
            Guard.Against.NotFound((int)catDay.ExhibitorsCage!, cages);

            if (cages.CatDays.TrueForAll(x => !catDaysHashSet.Contains(x.Id)))
            {
                appContext.Cages.Remove(cages);
            }
        }

        appContext.CatRegistrations.Remove(catRegistration);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<int>> UpdateCatRegistration(UpdateCatRegistrationDto updateCatRegistrationDto,
        bool isExhibitionOrganizer,
        CancellationToken cancellationToken = default)
    {
        int registrationToExhibitionId = appContext.CatRegistrations
            .Where(x => x.Id == updateCatRegistrationDto.Id)
            .Select(x => x.RegistrationToExhibitionId)
            .FirstOrDefault();
        Guard.Against.NotFound(updateCatRegistrationDto.Id, registrationToExhibitionId);
        CreateCatRegistrationDto createCatRegistration = new()
        {
            CatDays = updateCatRegistrationDto.CatDays,
            ExhibitedCat = updateCatRegistrationDto.ExhibitedCat,
            Litter = updateCatRegistrationDto.Litter,
            Note = updateCatRegistrationDto.Note,
            RegistrationToExhibitionId = registrationToExhibitionId
        };
        Result validationResult = await Validations(createCatRegistration, isExhibitionOrganizer, cancellationToken);
        if (validationResult.IsFailure)
        {
            return Result.Failure<int>(validationResult.Error);
        }

        Result validation =
            await ValidateCagesAsync(createCatRegistration, updateCatRegistrationDto.Id, cancellationToken);
        if (validation.IsFailure)
        {
            return Result.Failure<int>(validation.Error);
        }

        CatRegistration? catRegistrationEntity = await appContext.CatRegistrations
            .Include(x => x.CatDays)
            .ThenInclude(x => x.RentedTypeEntity)
            .ThenInclude(x => x != null ? x.RentedCage : null)
            .Include(x => x.RegistrationToExhibition)
            .Include(x => x.ExhibitedCat)
            .Include(x => x.Litter)
            .SingleOrDefaultAsync(x => x.Id == updateCatRegistrationDto.Id, cancellationToken);

        Guard.Against.NotFound(updateCatRegistrationDto.Id, catRegistrationEntity);
        List<CatDay> catDays = catRegistrationEntity.CatDays.ToList();
        catRegistrationEntity.CatDays = new List<CatDay>();

        appContext.CatDays.RemoveRange(catDays);
        await appContext.SaveChangesAsync(cancellationToken);
        if (catRegistrationEntity.ExhibitedCat != null)
        {
            appContext.ExhibitedCats.Remove(catRegistrationEntity.ExhibitedCat);
        }

        if (catRegistrationEntity.Litter != null)
        {
            appContext.Litters.Remove(catRegistrationEntity.Litter);
        }

        catRegistrationEntity.Note = updateCatRegistrationDto.Note;
        catRegistrationEntity.ExhibitedCat = updateCatRegistrationDto.ExhibitedCat != null
            ? mapper.Map<ExhibitedCat>(updateCatRegistrationDto.ExhibitedCat)
            : null;
        catRegistrationEntity.Litter = updateCatRegistrationDto.Litter != null
            ? mapper.Map<Litter>(updateCatRegistrationDto.Litter)
            : null;

        await appContext.SaveChangesAsync(cancellationToken);

        foreach (CreateCatDayDto catDayDto in updateCatRegistrationDto.CatDays)
        {
            Result<int?> result = await cagesService.GetRandomRentedCageTypeIdFromHash(catDayDto.RentedCageTypeId,
                catDayDto.ExhibitionDayId,
                catRegistrationEntity.RegistrationToExhibitionId, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<int>(result.Error);
            }

            CatDay catDay = new()
            {
                RentedCageTypeId = result.Value,
                ExhibitorsCage = catDayDto.ExhibitorsCage,
                CatRegistrationId = catRegistrationEntity.Id,
                ExhibitionDayId = catDayDto.ExhibitionDayId,
                Groups = catDayDto.GroupsIds.Select(x => appContext.Groups.Find(x)!).ToList()
            };

            if (catDayDto.Cage != null)
            {
                PersonCage personCage = new()
                {
                    Width = catDayDto.Cage.Width,
                    Height = catDayDto.Cage.Height,
                    Length = catDayDto.Cage.Length,
                    RegistrationToExhibitionId = registrationToExhibitionId,
                    CatDays = [catDay]
                };
                appContext.Cages.Add(personCage);
            }

            await appContext.CatDays.AddAsync(catDay, cancellationToken);
        }

        await appContext.SaveChangesAsync(cancellationToken);

        await cagesService.DeleteUnusedPersonCages(registrationToExhibitionId, cancellationToken);
        return catRegistrationEntity.Id;
    }


    /// <inheritdoc />
    public async Task<CatRegistrationDto> GetCatRegistrationById(int catRegistrationId,
        CancellationToken cancellationToken = default)
    {
        CatRegistration? catRegistrationEntity = await appContext.CatRegistrations
            .Include(x => x.CatDays)
            .ThenInclude(x => x.ExhibitionDay)
            .Include(x => x.CatDays)
            .ThenInclude(x => x.Groups)
            .Include(x => x.CatDays)
            .ThenInclude(x => x.RentedTypeEntity)
            .ThenInclude(x => x != null ? x.RentedCage : null)
            .Where(x => x.Id == catRegistrationId)
            .Include(x => x.Litter)
            .Include(x => x.ExhibitedCat)
            .Include(x => x.CatDays)
            .ThenInclude(x => x.Cage)
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(catRegistrationId, catRegistrationEntity);

        catRegistrationEntity.CatDays = catRegistrationEntity.CatDays.OrderBy(x => x.ExhibitionDay.Date).ToList();
        MiddleCatRegistrationDto? middleCatRegistrationDto =
            mapper.Map<MiddleCatRegistrationDto>(catRegistrationEntity);
        CatRegistrationDto? catRegistrationDto = mapper.Map<CatRegistrationDto>(middleCatRegistrationDto);

        return catRegistrationDto;
    }

    /// <inheritdoc />
    public async Task<List<MiddleCatRegistrationDto>> GetCatRegistrationsByRegistrationToExhibitionId(
        int registrationToExhibitionId, CancellationToken cancellationToken = default)
    {
        List<MiddleCatRegistrationDto> catRegistrations = await appContext.CatRegistrations
            .Where(x => x.RegistrationToExhibitionId == registrationToExhibitionId)
            .Include(x => x.CatDays)
            .Include(x => x.Litter)
            .Include(x => x.ExhibitedCat)
            .Select(x => mapper.Map<MiddleCatRegistrationDto>(x))
            .ToListAsync(cancellationToken);

        return catRegistrations;
    }


    /// <inheritdoc />
    public async Task<MultiCurrencyPrice> GetCatRegistrationPrice(int catRegistrationId,
        CancellationToken cancellationToken = default)
    {
        CatRegistration? catRegistration = await appContext.CatRegistrations
            .Where(x => x.Id == catRegistrationId)
            .Include(x => x.CatDays)
            .ThenInclude(x => x.Groups)
            .Include(x => x.CatDays)
            .ThenInclude(x => x.RentedTypeEntity)
            .ThenInclude(x => x != null ? x.RentedCage : null)
            .Include(x => x.RegistrationToExhibition)
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(catRegistrationId, catRegistration);
        CatRegistrationDto? dto = mapper.Map<CatRegistrationDto>(mapper.Map<MiddleCatRegistrationDto>(catRegistration));

        List<Price> prices = appContext.Prices
            .Where(x => x.ExhibitionDays.Any(y =>
                y.ExhibitionId == catRegistration.RegistrationToExhibition.ExhibitionId))
            .Include(x => x.Groups)
            .Include(x => x.ExhibitionDays)
            .ToList();

        HashSet<Record> records = ComputeAttendance(dto);
        MultiCurrencyPrice price = ComputePrice(records, prices);

        return price;
    }

    private Task<List<CatModelP>> GetUserCatsOrLittersNotInExhibitionBase<T>(
        int registrationToExhibitionId,
        Func<CatRegistration, bool> predicate,
        Func<CatRegistration, T?> selector,
        Func<CatRegistration, T, bool> comparer,
        Func<CatRegistration, CatModelP> projector)
        where T : class
    {
        int exhibitorId = appContext.RegistrationsToExhibition
            .Where(x => x.Id == registrationToExhibitionId)
            .Select(x => x.ExhibitorId)
            .FirstOrDefault();
        IEnumerable<CatRegistration> catRegistrations = appContext.RegistrationsToExhibition
            .Where(x => x.ExhibitorId == exhibitorId)
            .SelectMany(x => x.CatRegistrations)
            .Include(x => x.ExhibitedCat)
            .Include(x => x.Litter)
            .Include(x => x.RegistrationToExhibition)
            .AsEnumerable().Where(predicate)
            .OrderBy(x => x.LastModified)
            .ToList();

        List<CatRegistration> currentRegistrations = catRegistrations
            .Where(x => x.RegistrationToExhibitionId == registrationToExhibitionId)
            .ToList();

        HashSet<CatRegistration> uniqueCatsNotRegisteredInGivenRegistrationYet = [];
        foreach (CatRegistration registration in catRegistrations)
        {
            if (currentRegistrations.Any(y => comparer(registration,
                    selector(y) ?? throw new InvalidOperationException())))
            {
                continue;
            }

            if (uniqueCatsNotRegisteredInGivenRegistrationYet.Any(x =>
                    comparer(x, selector(registration) ?? throw new InvalidOperationException())))
            {
                continue;
            }

            uniqueCatsNotRegisteredInGivenRegistrationYet.Add(registration);
        }

        List<CatModelP> result = uniqueCatsNotRegisteredInGivenRegistrationYet.Select(projector).ToList();

        return Task.FromResult(result);
    }

    private async Task<Result> ValidateCagesAsync(CreateCatRegistrationDto createCatRegistrationDto,
        int catRegistrationId, CancellationToken cancellationToken)
    {
        foreach (CreateCatDayDto day in createCatRegistrationDto.CatDays)
        {
            bool registeringLitter = createCatRegistrationDto.RegistrationType == RegistrationType.Litter;
            CagesPerDayDto cages = await cagesService.GetAvailableCageGroupTypesAndOwnCages(day.ExhibitionDayId,
                createCatRegistrationDto.RegistrationToExhibitionId,
                registeringLitter,
                catRegistrationId,
                cancellationToken);

            if (day.ExhibitorsCage != null &&
                !cages.ExhibitorsCages.Select(x => x.CageId).Contains(day.ExhibitorsCage.Value))
            {
                return Result.Failure<int>(Errors.CageDoesNotBelongToExhibitionForExhibitorError);
            }
        }

        return Result.Success();
    }

    private async Task<Result> Validations(CreateCatRegistrationDto createCatRegistrationDto,
        bool isExhibitionOrganizer,
        CancellationToken cancellationToken)
    {
        if (createCatRegistrationDto.ExhibitedCat == null && createCatRegistrationDto.Litter == null)
        {
            return Result.Failure<int>(Errors.ExhibitedCatOrLitterMustBeFilledInError);
        }

        if (createCatRegistrationDto is { ExhibitedCat: not null, Litter: not null })
        {
            return Result.Failure<int>(Errors.ExhibitedCatAndLitterCannotBeFilledAtTheSameTimeError);
        }

        Domain.Entities.RegistrationToExhibition registrationToExhibition =
            await GetRegistrationToExhibition(createCatRegistrationDto, cancellationToken);
        HashSet<int> daysInExhibitionIds =
            registrationToExhibition.Exhibition.Days.Select(x => x.Id).ToHashSet();

        if (createCatRegistrationDto.CatDays.Distinct().Count() != createCatRegistrationDto.CatDays.Count)
        {
            return Result.Failure<int>(Errors.CatDaysAreNotUniqueError);
        }

        if (createCatRegistrationDto.CatDays.Count > daysInExhibitionIds.Count)
        {
            return Result.Failure<int>(Errors.ThereAremMoreCatDaysInExhibitionError);
        }

        if (!createCatRegistrationDto.CatDays.TrueForAll(catDay =>
                daysInExhibitionIds.Contains(catDay.ExhibitionDayId)))
        {
            return Result.Failure<int>(Errors.CatDayDoesNotBelongToExhibitionError);
        }

        if (createCatRegistrationDto.CatDays.Exists(
                catDay => catDay is { ExhibitorsCage: null, RentedCageTypeId: null, Cage: null }))
        {
            return Result.Failure<int>(Errors.CatDayHasNeitherCageNorRentedCageTypeError);
        }

        if (createCatRegistrationDto.CatDays.Exists(
                catDay => catDay is { ExhibitorsCage: not null, RentedCageTypeId: not null } or
                    { ExhibitorsCage: not null, Cage: not null } or { RentedCageTypeId: not null, Cage: not null }))
        {
            return Result.Failure<int>(Errors.CatDayHasBothCageAndRentedCageTypeError);
        }

        if (RegistrationToExhibitionService.IsPaid(registrationToExhibition.PaymentInfo) &&
            !isExhibitionOrganizer)
        {
            return Result.Failure<int>(Errors.CannotUpdateRegistrationThatHasAlreadyBeenPaidError);
        }

        ExhibitorAndUserDto? exhibitorDto = await exhibitorService.GetExhibitorById(
            registrationToExhibition.ExhibitorId, cancellationToken);
        if (exhibitorDto == null)
        {
            throw new InvalidDatabaseStateException("Exhibitor not found");
        }

        foreach (CreateCatDayDto day in createCatRegistrationDto.CatDays)
        {
            if (day.GroupsIds.Count == 0)
            {
                return Result.Failure<int>(Errors.CatDayHasNoGroupsError);
            }

            HashSet<string> availableGroups = (await exhibitionService.GetGroupsCatRegistrationCanBeRegisteredIn(
                new LitterOrExhibitedCatDto
                {
                    LitterDto = createCatRegistrationDto.Litter,
                    ExhibitedCat = createCatRegistrationDto.ExhibitedCat,
                    ExhibitorDto = exhibitorDto
                },
                day.ExhibitionDayId)).Select(x => x.GroupId).ToHashSet();

            if (createCatRegistrationDto.Litter != null && day.Cage != null)
            {
                RentedType cageType = CagesService.GetCageTypeForCage(new CreateCageDto
                {
                    Height = day.Cage.Height, Length = day.Cage.Length, Width = day.Cage.Width
                });

                if (cageType == RentedType.Single)
                {
                    return Result.Failure<int>(Errors.CatDayHasGroupsThatAreNotAvailable);
                }
            }

            if (!day.GroupsIds.TrueForAll(group =>
                    availableGroups.Contains(group)))
            {
                return Result.Failure<int>(Errors.CatDayHasGroupsThatAreNotAvailable);
            }
        }

        HashSet<int> exhibitionDaysIdsCatIsRegisteredInto =
            createCatRegistrationDto.CatDays.Select(x => x.ExhibitionDayId).ToHashSet();
        HashSet<int> allDaysInExhibition = registrationToExhibition.Exhibition.Days.Select(x => x.Id).ToHashSet();
        if (exhibitionDaysIdsCatIsRegisteredInto.Except(allDaysInExhibition).Any())
        {
            return Result.Failure<int>(Errors.CatIsRegisteredIntoDAysThatAreNotInExhibitionError);
        }

        ListComparer<int> intComparer = new();
        List<Price> allPrices = appContext.Prices
            .Include(x => x.Groups)
            .Include(z => z.ExhibitionDays)
            .ToList();

        HashSet<string> prices = allPrices
            .Where(x => intComparer.Equals(x.ExhibitionDays.Select(exhibitionDay => exhibitionDay.Id).ToList(),
                exhibitionDaysIdsCatIsRegisteredInto.ToList()))
            .SelectMany(x => x.Groups)
            .Select(x => x.GroupId)
            .ToHashSet();

        List<HashSet<string>> groups = createCatRegistrationDto.CatDays
            .Select(x => x.GroupsIds.ToHashSet())
            .ToList();

        bool selectedDaysHaveRegisteredGroups = groups.All(x => x.All(prices.Contains));
        if (!selectedDaysHaveRegisteredGroups)
        {
            return Result.Failure<int>(Errors.CatDayHasNoGroupsError);
        }

        switch (createCatRegistrationDto.RegistrationType)
        {
            case RegistrationType.NonHomeExhibitedCat when createCatRegistrationDto.ExhibitedCat!.Father == null ||
                                                           createCatRegistrationDto.ExhibitedCat!.Mother == null:
                return Result.Failure<int>(Errors.NonHomeCatMustHaveParents);
            case RegistrationType.NonHomeExhibitedCat:
                {
                    FatherDto? father = createCatRegistrationDto.ExhibitedCat!.Father;
                    MotherDto? mother = createCatRegistrationDto.ExhibitedCat!.Mother;

                    if (father!.PedigreeNumber == mother!.PedigreeNumber)
                    {
                        return Result.Failure<int>(Errors.FatherAndMotherCannotHaveSamePedigreeNumberError);
                    }

                    if (createCatRegistrationDto.ExhibitedCat.PedigreeNumber == father.PedigreeNumber ||
                        createCatRegistrationDto.ExhibitedCat.PedigreeNumber == mother.PedigreeNumber)
                    {
                        return Result.Failure<int>(Errors.CatCannotHaveTheSamePedigreeNumberAdFatherOrMotherError);
                    }

                    break;
                }
            case RegistrationType.HomeExhibitedCat when createCatRegistrationDto.ExhibitedCat!.Father != null ||
                                                        createCatRegistrationDto.ExhibitedCat!.Mother != null:
                return Result.Failure<int>(Errors.HomeCatMustNotHaveParentsError);
            case RegistrationType.Litter:
                {
                    FatherDto father = createCatRegistrationDto.Litter!.Father;
                    MotherDto mother = createCatRegistrationDto.Litter!.Mother;

                    if (father.PedigreeNumber == mother.PedigreeNumber)
                    {
                        return Result.Failure<int>(Errors.FatherAndMotherCannotHaveSamePedigreeNumberError);
                    }

                    break;
                }
        }

        ICollection<ExhibitionDay> exhibitionDays = registrationToExhibition.Exhibition.Days;
        IEnumerable<int> exhibitionDaysThatAreAttended =
            createCatRegistrationDto.CatDays.Select(x => x.ExhibitionDayId);
        DateOnly firstExhibitionDayDate =
            exhibitionDays.Where(x => exhibitionDaysThatAreAttended.Contains(x.Id)).Min(x => x.Date);
        if (createCatRegistrationDto is { ExhibitedCat: null, Litter: not null })
        {
            if (createCatRegistrationDto.Litter.BirthDate.AddMonths(10) <=
                firstExhibitionDayDate)
            {
                return Result.Failure<int>(Errors.LitterMustBeYoungerThan10MonthsError);
            }

            if (createCatRegistrationDto.Litter.BirthDate.AddMonths(4) >=
                firstExhibitionDayDate)
            {
                return Result.Failure<int>(Errors.LitterMustBeOlderThan4MonthsError);
            }
        }

        if (createCatRegistrationDto is { ExhibitedCat: not null, Litter: null })
        {
            if (createCatRegistrationDto.ExhibitedCat.BirthDate.AddMonths(4) >=
                firstExhibitionDayDate)
            {
                return Result.Failure<int>(Errors.CatMustBeOlderThan4MonthsError);
            }
        }

        if (createCatRegistrationDto.ExhibitedCat != null)
        {
            if (!VerifyEmsCode(createCatRegistrationDto.ExhibitedCat.Ems))
            {
                return Result.Failure<int>(Errors.InvalidEmsCodeError);
            }

            if (createCatRegistrationDto.ExhibitedCat.Father != null)
            {
                if (!VerifyEmsCode(createCatRegistrationDto.ExhibitedCat.Father.Ems))
                {
                    return Result.Failure<int>(Errors.InvalidEmsCodeError);
                }
            }

            if (createCatRegistrationDto.ExhibitedCat.Mother != null)
            {
                if (!VerifyEmsCode(createCatRegistrationDto.ExhibitedCat.Mother.Ems))
                {
                    return Result.Failure<int>(Errors.InvalidEmsCodeError);
                }
            }
        }

        if (createCatRegistrationDto.Litter != null)
        {
            if (!VerifyEmsCode(createCatRegistrationDto.Litter.Father.Ems))
            {
                return Result.Failure<int>(Errors.InvalidEmsCodeError);
            }

            if (!VerifyEmsCode(createCatRegistrationDto.Litter.Mother.Ems))
            {
                return Result.Failure<int>(Errors.InvalidEmsCodeError);
            }
        }

        return Result.Success();
    }

    private bool VerifyEmsCode(string ems)
    {
        Result<EmsCode> emsCode = EmsCode.Create(ems);
        if (emsCode.IsFailure)
        {
            return false;
        }

        EmsResult parsedEms = emsCode.Value.Parse();
        return !parsedEms.IsFatalFailure;
    }

    private async Task<Domain.Entities.RegistrationToExhibition> GetRegistrationToExhibition(
        CreateCatRegistrationDto createCatRegistrationDto,
        CancellationToken cancellationToken)
    {
        Domain.Entities.RegistrationToExhibition? registrationToExhibition = await appContext
            .RegistrationsToExhibition
            .Where(x => x.Id == createCatRegistrationDto.RegistrationToExhibitionId)
            .Include(x => x.Exhibition)
            .ThenInclude(x => x.Days)
            .ThenInclude(x => x.CagesForRent)
            .FirstOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(createCatRegistrationDto.RegistrationToExhibitionId, registrationToExhibition);
        return registrationToExhibition;
    }

    private void MapExhibitedCatOrLitter(CreateCatRegistrationDto createCatRegistrationDto,
        CatRegistration catRegistration)
    {
        if (createCatRegistrationDto.ExhibitedCat != null)
        {
            ExhibitedCat exhibitedCat =
                mapper.Map<ExhibitedCat>(createCatRegistrationDto.ExhibitedCat);
            catRegistration.ExhibitedCat = exhibitedCat;
        }
        else if (createCatRegistrationDto.Litter != null)
        {
            Litter litter = mapper.Map<Litter>(createCatRegistrationDto.Litter);
            catRegistration.Litter = litter;
        }
        else if (createCatRegistrationDto is { ExhibitedCat: not null, Litter: not null })
        {
            throw new InvalidDataContractException("ExhibitedCat and Litter cannot be filled at the same time");
        }
        else
        {
            throw new InvalidDataContractException(
                "ExhibitedCat or Litter must be filled but this should be handled above");
        }
    }

    private MultiCurrencyPrice ComputePrice(HashSet<Record> records, List<Price> prices)
    {
        ListComparer<int> comparer = new();
        decimal priceKc = 0;
        decimal priceEur = 0;
        foreach (Record record in records)
        {
            List<decimal> possiblePricesKc = [];
            List<decimal> possiblePricesEur = [];
            List<Price?> list = record.Groups
                .Select(group => prices
                    .Find(x => x.Groups.Exists(
                                   group1 => group1.GroupId == group) &&
                               comparer.Equals(
                                   x.ExhibitionDays.Select(y => y.Id).ToList(),
                                   record.ExhibitionDayIds.ToList()
                               )
                    )
                )
                .ToList();
            foreach (Price? priceRecord in list)
            {
                if (priceRecord == null)
                {
                    throw new InvalidDatabaseStateException("Price not found");
                }

                possiblePricesKc.Add(priceRecord.Amounts.Where(x => x.Currency == Currency.Czk).Single().Amount);
                possiblePricesEur.Add(priceRecord.Amounts.Where(x => x.Currency == Currency.Eur).Single().Amount);
            }

            priceKc += possiblePricesKc.Max();
            priceEur += possiblePricesEur.Max();
        }

        return new MultiCurrencyPrice(priceKc, priceEur);
    }

    private HashSet<Record> ComputeAttendance(CatRegistrationDto dto)
    {
        ListComparer<string> comparer = new();
        HashSet<Record> records = [];
        foreach (CatDayDto var in dto.CatDays)
        {
            if (records.Any(x => comparer.Equals(x.Groups, var.GroupsIds)))
            {
                Record record = records.Single(x => comparer.Equals(x.Groups, var.GroupsIds));
                record.ExhibitionDayIds.Add(var.ExhibitionDayId);
                record.Days++;
            }

            else if (records.Any(x => x.Groups.Any(y => var.GroupsIds.Contains(y))))
            {
                Record record = records.Single(x => x.Groups.Any(y => var.GroupsIds.Contains(y)));
                record.ExhibitionDayIds.Add(var.ExhibitionDayId);
                record.Groups.AddRange(var.GroupsIds);
                record.Groups = record.Groups.Distinct().ToList();
                record.Days++;
            }
            else
            {
                Record record = new() { Groups = var.GroupsIds, Days = 1, ExhibitionDayIds = [var.ExhibitionDayId] };
                records.Add(record);
            }
        }

        return records;
    }

    private record Record
    {
        public required List<string> Groups { get; set; } = [];
        public required HashSet<int> ExhibitionDayIds { get; init; } = [];
        public required int Days { get; set; }
    }
}
