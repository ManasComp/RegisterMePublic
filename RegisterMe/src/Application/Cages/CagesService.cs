#region

using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Cages.Dtos.Combination;
using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Cages;

/// <inheritdoc />
public class CagesService(IApplicationDbContext appContext, IMapper mapper, CagesServiceHelper cagesServiceHelper)
    : ICagesService
{
    private const int SingleCageMinWidth = 50;
    private const int SingleCageMinLength = 50;
    private const int DoubleCageMinWidth = 50;
    private const int DoubleCageMinLength = 100;

    /// <inheritdoc />
    public async Task<Result<string>> UpdateCageGroup(string rentedCageIds, CreateRentedRentedCageDto rentedCage,
        CancellationToken cancellationToken = default)
    {
        Result<string> ids = await CreateRentedCages(rentedCage, cancellationToken);

        if (ids.IsSuccess)
        {
            Result result = await DeleteRentedCages(rentedCageIds, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<string>(result.Error);
            }
        }

        await appContext.SaveChangesAsync(cancellationToken);
        return ids;
    }

    /// <inheritdoc />
    public async Task<Result<string>> CreateRentedCages(CreateRentedRentedCageDto rentedCage,
        CancellationToken cancellationToken = default)
    {
        HashSet<int> exhibitionDayIds = rentedCage.ExhibitionDaysId.ToHashSet();
        int exhibitionId = await appContext.ExhibitionDays
            .Where(x => x.Id == exhibitionDayIds.First())
            .Select(x => x.ExhibitionId)
            .SingleAsync(cancellationToken);
        List<int> exhibitionDays = await appContext.ExhibitionDays
            .Where(x => x.ExhibitionId == exhibitionId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        bool inconsistent = exhibitionDayIds.Except(exhibitionDays).Any();
        if (inconsistent)
        {
            return Result.Failure<string>(Errors.ExhibitionDasAreNotConsistentWithExhibitionError);
        }

        if (rentedCage.RentedCageTypes.Contains(RentedType.Double))
        {
            if (rentedCage.Width < DoubleCageMinWidth || rentedCage.Length < DoubleCageMinLength)
            {
                return Result.Failure<string>(Errors.DoubleCageIsTooSmallError);
            }
        }
        else
        {
            if (rentedCage.Width < SingleCageMinWidth || rentedCage.Length < SingleCageMinLength)
            {
                return Result.Failure<string>(Errors.SingleCageIsTooSmallError);
            }
        }

        List<ExhibitionDay> days = await appContext.ExhibitionDays
            .Where(x => exhibitionDayIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        List<RentedCage> rentedCages = Enumerable.Range(0, rentedCage.Count)
            .Select(_ =>
            {
                RentedCage? newCage = mapper.Map<RentedCage>(rentedCage);
                newCage.ExhibitionDays = days;
                newCage.RentedTypes = rentedCage.RentedCageTypes.Select(x => new RentedTypeEntity { RentedType = x })
                    .ToList();
                return newCage;
            })
            .ToList();

        await appContext.RentedCages.AddRangeAsync(rentedCages, cancellationToken);
        await appContext.SaveChangesAsync(cancellationToken);
        return FromIdsToGroupIds(rentedCages.Select(x => x.Id).ToList());
    }

    /// <inheritdoc />
    public async Task<Result<int?>> GetRandomRentedCageTypeIdFromHash(RentedCageGroup? cageHash, int exhibitionDayId,
        int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        if (cageHash == null)
        {
            return Result.Success<int?>(null);
        }

        CagesPerDayDto data = await GetAvailableCageGroupTypesAndOwnCages(exhibitionDayId, registrationToExhibitionId,
            false, // we want all cages
            null,
            cancellationToken);

        CageGroupDtoDescription? rentedCage = data.AvailableCagesForRent
            .Find(x => cageHash.Equals(x.RentedTypeId));

        if (rentedCage != null)
        {
            return rentedCage.Ids.First();
        }

        CageGroupDtoDescription? ownCage = data.AvailableAlreadyRentedCagesByExhibitor
            .Find(x => cageHash.Equals(x.RentedTypeId));

        return ownCage != null ? ownCage.Ids.First() : Result.Failure<int?>(Errors.CageNotFoundError);
    }

    /// <inheritdoc />
    public async Task<CagesPerDayDto> GetAvailableCageGroupTypesAndOwnCages(int exhibitionDayId,
        int registrationToExhibitionId, bool isLitter, int? doNotIncludeCatRegistrationId = null,
        CancellationToken cancellationToken = default)
    {
        ExhibitionDay? exhibitionDay = await appContext.ExhibitionDays
            .Where(x => x.Id == exhibitionDayId)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(exhibitionDayId, exhibitionDay);

        List<ExhibitionCagesInfo> cages = await GetExhibitionCagesInfo(exhibitionDay.ExhibitionId, exhibitionDayId,
            doNotIncludeCatRegistrationId, cancellationToken);
        IEnumerable<CageGroupDtoDescription> newCages =
            await cagesServiceHelper.GetAvailableCageGroupTypesForGivenExhibitionDay(cages, isLitter,
                cancellationToken);

        IEnumerable<CageGroupDtoDescription> usedCages =
            await cagesServiceHelper.GetAlreadyUsedCagesPerGivenExhibitionDay(exhibitionDayId,
                registrationToExhibitionId,
                isLitter,
                doNotIncludeCatRegistrationId,
                cancellationToken);

        IEnumerable<CageDtoDescription> ownCages =
            await cagesServiceHelper.GetOwnFreeCages(exhibitionDayId, registrationToExhibitionId, isLitter,
                doNotIncludeCatRegistrationId,
                cancellationToken);

        return new CagesPerDayDto
        {
            AvailableCagesForRent = newCages.ToList(),
            AvailableAlreadyRentedCagesByExhibitor = usedCages.ToList(),
            ExhibitorsCages = ownCages.ToList()
        };
    }

    /// <inheritdoc />
    public async Task<Result> DeleteRentedCages(string rentedCagesId, CancellationToken cancellationToken = default)
    {
        List<int> ids = FromGroupIdToIds(rentedCagesId);
        List<RentedCage> cageGroup = await appContext.RentedCages
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        appContext.RentedCages.RemoveRange(cageGroup);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<RentedCageGroup?> GetCageHashIsAreRegisteredTo(int catRegistrationDayId, int exhibitionDayId,
        int? rentedTypeId,
        CancellationToken cancellationToken = default)
    {
        if (rentedTypeId == null)
        {
            return null;
        }

        RentedTypeEntity? rentedType = await appContext.RentedTypes
            .Where(x => x.Id == rentedTypeId)
            .Include(x => x.CatDays)
            .ThenInclude(x => x.CatRegistration)
            .ThenInclude(x => x.RegistrationToExhibition)
            .Include(x => x.RentedCage)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound((int)rentedTypeId, rentedType);
        List<CatDay> catDays = rentedType.CatDays.Where(x => x.ExhibitionDayId == exhibitionDayId).ToList();

        int registrationToExhibitionId = catDays.First(x => x.Id == catRegistrationDayId).CatRegistration
            .RegistrationToExhibitionId;
        RentingType rentingType;
        if (rentedType.RentedType == RentedType.Single ||
            (rentedType is { RentedType: RentedType.Double } && catDays.Count == 1))
        {
            rentingType = RentingType.RentedWithZeroOtherCats;
        }
        else
        {
            rentingType = catDays.Count switch
            {
                2 when catDays.TrueForAll(x =>
                        x.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId) =>
                    RentingType.RentedWithOneOtherCat,
                3 when catDays.TrueForAll(x =>
                        x.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId) =>
                    RentingType.RentedWithTwoOtherCats,
                _ => throw new InvalidDatabaseStateException("Cage is already rented")
            };
        }

        return new RentedCageGroup(rentedType.RentedCage.Height, rentedType.RentedCage.Length,
            rentedType.RentedCage.Width,
            rentedType.RentedType, rentingType);
    }

    /// <inheritdoc />
    public async Task<List<ExhibitionCagesInfo>> GetExhibitionCagesInfo(int exhibitionId,
        int? exhibitionDayId = null, int? doNotIncludeCatRegistrationId = null,
        CancellationToken cancellationToken = default)
    {
        if (doNotIncludeCatRegistrationId != null)
        {
            bool belongsToExhibition = appContext.Exhibitions
                .Where(x => x.Id == exhibitionId)
                .SelectMany(x => x.RegistrationsToExhibitions)
                .SelectMany(x => x.CatRegistrations)
                .Select(x => x.Id)
                .AsEnumerable()
                .Contains(doNotIncludeCatRegistrationId.Value);

            if (!belongsToExhibition)
            {
                throw new InvalidOperationException("Cat registration does not belong to this exhibition");
            }
        }

        if (exhibitionDayId != null)
        {
            bool dayBelongsToExhibition = appContext.ExhibitionDays
                .Where(x => x.ExhibitionId == exhibitionId)
                .Select(x => x.Id)
                .AsEnumerable()
                .Contains(exhibitionDayId.Value);

            if (!dayBelongsToExhibition)
            {
                throw new InvalidOperationException("Exhibition day does not belong to this exhibition");
            }
        }

        List<ExhibitionDay> exhibitionDay = await appContext.ExhibitionDays
            .Where(x => x.ExhibitionId == exhibitionId)
            .Where(x => exhibitionDayId == null || x.Id == exhibitionDayId)
            .Include(x => x.CatDays)
            .ThenInclude(x => x.Cage)
            .Include(x => x.CatDays)
            .ThenInclude(x => x.CatRegistration)
            .ThenInclude(x => x.RegistrationToExhibition)
            .Include(x => x.CagesForRent)
            .ThenInclude(x => x.RentedTypes)
            .ThenInclude(x => x.CatDays)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        List<ExhibitionCagesInfo> infos = [];
        infos.AddRange(from day in exhibitionDay
            let numberOfPersonCagesPerDay = cagesServiceHelper.GetNumberOfPersonCagesPerDay(day)
            let cageGroupInfos = cagesServiceHelper.CageGroupInfos(doNotIncludeCatRegistrationId, day)
            select new ExhibitionCagesInfo
            {
                ExhibitionDayId = day.Id,
                Date = day.Date.ToString("dd/MM/yyyy"),
                NumberOfPersonCages = numberOfPersonCagesPerDay,
                CageGroupInfos = cageGroupInfos
            });

        return infos;
    }

    /// <inheritdoc />
    public async Task<List<CageDto>> GetPersonCagesByExhibitionDay(int exhibitionDayId,
        int? registrationToExhibitionId = null,
        CancellationToken cancellationToken = default)
    {
        ExhibitionDay? exhibitionDay = await appContext.ExhibitionDays
            .Where(x => x.Id == exhibitionDayId)
            .Where(x => registrationToExhibitionId == null ||
                        x.CatDays.Any(day =>
                            day.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId))
            .Include(x => x.CatDays)
            .ThenInclude(x => x.Cage)
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(exhibitionDayId, exhibitionDay);

        List<CageDto> personCages = exhibitionDay.CatDays
            .Where(x => x.Cage != null)
            .Select(x => x.Cage)
            .Distinct()
            .Select(x => new CageDto
            {
                Height = x!.Height, Length = x.Length, Width = x.Width, NumberOfCatsInCage = x.CatDays.Count
            })
            .ToList();

        return personCages;
    }

    /// <inheritdoc />
    public async Task<List<CatRegistrationStatistics>> GetStatisticsAboutRegistrationToExhibition(
        int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        var exhibitionDays = await appContext.ExhibitionDays
            .Where(x => x.CatDays.Any(day =>
                day.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId))
            .Select(x => new { x.Id, x.Date })
            .ToListAsync(cancellationToken);

        return exhibitionDays
            .Select(day => new CatRegistrationStatistics
            {
                OwnCages = cagesServiceHelper.GetNumberOfOwnCages(day.Id, registrationToExhibitionId),
                RentedCages = cagesServiceHelper.GetNumberOfRentedCages(day.Id, registrationToExhibitionId),
                Date = day.Date,
                NumberOfCats = cagesServiceHelper.GetNumberOfCats(day.Id, registrationToExhibitionId)
            }).OrderBy(x => x.Date).ToList();
    }

    /// <inheritdoc />
    public async Task<List<RentedTypeEntity>> GetRentedTypes(int exhibitionId, int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        List<RentedTypeEntity> rentedTypes = await appContext.RentedTypes
            .Where(x => x.CatDays.Any(day =>
                day.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId &&
                day.ExhibitionDay.Exhibition.Id == exhibitionId))
            .Include(x => x.CatDays)
            .ToListAsync(cancellationToken);

        return rentedTypes;
    }

    /// <inheritdoc />
    public Task<CageDto> GetPersonCageById(int cageId, CancellationToken cancellationToken = default)
    {
        CageDto cage = appContext.Cages
            .Where(x => x.Id == cageId)
            .Select(x => new CageDto
            {
                Height = x.Height, Length = x.Length, Width = x.Width, NumberOfCatsInCage = x.CatDays.Count
            })
            .Single();

        return Task.FromResult(cage);
    }

    /// <inheritdoc />
    public Task<bool> RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageId(int cageId, int exhibitionDayId,
        int catRegistrationId,
        CancellationToken cancellationToken = default)
    {
        bool ids = appContext.CatDays
            .Where(x => x.ExhibitionDayId == exhibitionDayId)
            .Where(x => x.CatRegistrationId == catRegistrationId)
            .Select(x => x.ExhibitorsCage)
            .Distinct()
            .Any(x => x == cageId);

        return Task.FromResult(ids);
    }

    /// <inheritdoc />
    public Task<List<PersonCageWithCatDays>> GetPersonCagesByRegistrationToExhibitionId(
        int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        List<PersonCageWithCatDays> cages = appContext.Cages
            .Where(x => x.CatDays.Any(day =>
                day.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId))
            .Include(x => x.CatDays)
            .Select(x =>
                new PersonCageWithCatDays
                {
                    PersonCageWithId = new PersonCageWithId
                    {
                        PersonCage = new CageDto
                        {
                            Height = x.Height,
                            Length = x.Length,
                            Width = x.Width,
                            NumberOfCatsInCage = x.CatDays.Count
                        },
                        PersonCageId = x.Id
                    },
                    CatDays = x.CatDays
                })
            .ToList();

        return Task.FromResult(cages);
    }

    /// <inheritdoc />
    public async Task DeleteUnusedPersonCages(int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        List<PersonCage> cages = appContext.Cages
            .Where(x => x.RegistrationToExhibitionId == registrationToExhibitionId)
            .Include(x => x.CatDays)
            .ToList();

        List<PersonCage> personCages = cages.Where(x => x.CatDays.Count == 0).ToList();
        appContext.Cages.RemoveRange(personCages);
        await appContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> GetRegistrationToExhibitionIdByCageId(int cageId, CancellationToken cancellationToken = default)
    {
        int registrationToExhibitionId = appContext.Cages
            .Where(x => x.Id == cageId)
            .Select(x => x.RegistrationToExhibitionId)
            .Single();

        return Task.FromResult(registrationToExhibitionId);
    }

    /// <inheritdoc />
    public async Task<List<BriefCageDto>> GetRentedCagesByExhibitionId(int exhibitionId)
    {
        var data = await appContext.RentedCages
            .Where(x => x.ExhibitionDays.Any(day => day.ExhibitionId == exhibitionId))
            .Select(x => new
            {
                id = x.Id,
                width = x.Width,
                height = x.Height,
                length = x.Length,
                rentedType = x.RentedTypes.Select(typeEntity => typeEntity.RentedType).ToList(),
                exhibitionDay = x.ExhibitionDays.Select(y => new SmallExhibitionDayDto { Id = y.Id, Date = y.Date })
            })
            .AsNoTracking()
            .ToListAsync();
        List<BriefCageDto> cages = data
            .GroupBy(x => new ItemDto
            {
                Width = x.width,
                Height = x.height,
                Length = x.length,
                RentedTypes = x.rentedType,
                ExhibitionDays = x.exhibitionDay.ToList()
            })
            .Select(x => new BriefCageDto
            {
                Count = x.Count(),
                Ids = FromIdsToGroupIds(x.Select(y => y.id).ToList()),
                Width = x.Key.Width,
                Height = x.Key.Height,
                Length = x.Key.Length,
                RentedTypes = x.Key.RentedTypes,
                ExhibitionDays =
                    x.Key.ExhibitionDays.Select(y => new SmallExhibitionDayDto { Id = y.Id, Date = y.Date })
                        .ToList()
            }).ToList();

        return cages;
    }

    /// <inheritdoc />
    public async Task<BriefCageDto> GetRentedCageByGroupId(string rentedCageId)
    {
        List<int> ids = FromGroupIdToIds(rentedCageId);
        var data = await appContext.RentedCages
            .Where(x => ids.Contains(x.Id))
            .Select(x => new
            {
                exhibitionId = x.ExhibitionDays.Select(y => y.ExhibitionId).First(),
                id = x.Id,
                width = x.Width,
                height = x.Height,
                length = x.Length,
                rentedType = x.RentedTypes.Select(z => z.RentedType).ToList(),
                exhibitionDay = x.ExhibitionDays.Select(z => new SmallExhibitionDayDto { Id = z.Id, Date = z.Date })
            })
            .AsNoTracking()
            .ToListAsync();
        if (data.Count == 0)
        {
            throw new NotFoundException("RentedCage", rentedCageId);
        }

        if (data.Select(x => x.exhibitionId).Distinct().Count() > 1)
        {
            throw new InvalidDatabaseStateException("RentedCages are not from the same exhibition.");
        }

        BriefCageDto dto = new()
        {
            Count = data.Count,
            Ids = FromIdsToGroupIds(data.Select(x => x.id).ToList()),
            Width = data.First().width,
            Height = data.First().height,
            Length = data.First().length,
            RentedTypes = data.First().rentedType,
            ExhibitionDays = data.First().exhibitionDay.ToList()
        };

        return dto;
    }

    public static List<int> FromGroupIdToIds(string groupId)
    {
        return groupId.Split(",").Select(int.Parse).ToList();
    }

    private static string FromIdsToGroupIds(List<int> ids)
    {
        return string.Join(",", ids.OrderDescending());
    }

    public static RentedType GetCageTypeForCage(CreateCageDto cage)
    {
        RentedType rentedType = cage switch
        {
            { Width: >= DoubleCageMinWidth, Length: >= DoubleCageMinLength } => RentedType.Double,
            { Width: >= SingleCageMinWidth, Length: >= SingleCageMinLength } => RentedType.Single,
            _ => throw new InvalidDatabaseStateException("Cage is too small to be rented.")
        };
        return rentedType;
    }
}
