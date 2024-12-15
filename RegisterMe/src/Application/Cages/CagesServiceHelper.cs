#region

using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Cages;

public class CagesServiceHelper(IApplicationDbContext appContext, IMapper mapper)
{
    private const int SingleCageMaxRegistrationToExhibitionCount = 1;
    private const int DoubleCageMaxRegistrationToExhibitionCount = 3;

    public List<CageGroupInfo> CageGroupInfos(int? doNotIncludeCatRegistrationId, ExhibitionDay day)
    {
        var cagesGroupBySize = day.CagesForRent
            .GroupBy(
                x => new { x.Width, x.Height, x.Length })
            .ToList();

        var availabilityCages = cagesGroupBySize.Select(x => new
        {
            x.Key.Width,
            x.Key.Height,
            x.Key.Length,
            TotalNumberOfRentedCages = x.Count(),
            AvailableTypes = GetAvailability(day.Id, x, doNotIncludeCatRegistrationId)
        }).ToList();

        List<CageGroupInfo> final = availabilityCages.Select(x => new CageGroupInfo
        {
            Width = x.Width,
            Height = x.Height,
            Length = x.Length,
            TotalNumberOfRentedCages = x.TotalNumberOfRentedCages,
            NumberOfUsedCages = x.AvailableTypes.Item1,
            AvailableTypes = x.AvailableTypes.Item2
        }).ToList();

        return final;
    }

    private Tuple<CagesRec, Dictionary<RentedType, HashSet<int>>> GetAvailability(int exhibitionDayId,
        IGrouping<object, RentedCage> cages, int? doNotIncludeCatRegistrationId)
    {
        CagesRec cagesRecRecord = new() { NumberOfSingleCages = 0, NumberOfDoubleCages = 0 };
        Dictionary<RentedType, HashSet<int>> availableTypesIds = new();

        foreach (RentedCage? cage in cages)
        {
            CagesRec oneCageRecUsage = CalculateOneCageUsage(cage, exhibitionDayId, doNotIncludeCatRegistrationId);
            cagesRecRecord.NumberOfSingleCages += oneCageRecUsage.NumberOfSingleCages;
            cagesRecRecord.NumberOfDoubleCages += oneCageRecUsage.NumberOfDoubleCages;

            UpdateAvailableTypesIds(cage, oneCageRecUsage, availableTypesIds);
        }

        return new Tuple<CagesRec, Dictionary<RentedType, HashSet<int>>>(cagesRecRecord, availableTypesIds);
    }

    private bool GetCatDaysPerExhibitionDay(RentedTypeEntity rentedTypeEntity, int exhibitionDayId,
        int? catRegistrationIdToNotInclude = null)
    {
        List<CatDay> catDays = rentedTypeEntity.CatDays;
        List<CatDay> days = catDays.Where(x =>
            x.ExhibitionDayId == exhibitionDayId && x.CatRegistrationId != catRegistrationIdToNotInclude).ToList();
        return days.Count != 0;
    }

    private CagesRec CalculateOneCageUsage(RentedCage cage, int exhibitionDayId,
        int? doNotIncludeCatRegistrationId)
    {
        CagesRec cagesRec = new() { NumberOfSingleCages = 0, NumberOfDoubleCages = 0 };

        List<RentedTypeEntity> validRentedTypes = cage.RentedTypes
            .Where(rentedType => GetCatDaysPerExhibitionDay(rentedType, exhibitionDayId, doNotIncludeCatRegistrationId))
            .ToList();

        foreach (RentedTypeEntity? rentedType in validRentedTypes)
        {
            int count = rentedType.CatDays.Count(x => x.ExhibitionDayId == exhibitionDayId);
            if (count == 0)
            {
                throw new InvalidDatabaseStateException("Invalid rented cage state.");
            }

            if (rentedType.RentedType == RentedType.Double)
            {
                // multiple days in double cages are shared, we count only one
                cagesRec.NumberOfDoubleCages++;
            }
            else
            {
                // multiple days in single cages are not shared, we count all
                cagesRec.NumberOfSingleCages += count;
            }
        }

        if (cagesRec.NumberOfSingleCages > 2)
        {
            throw new InvalidDatabaseStateException("Invalid rented single cage state.");
        }

        if (cagesRec.NumberOfDoubleCages > 3)
        {
            throw new InvalidDatabaseStateException("Invalid rented double cage state.");
        }

        if (cagesRec is { NumberOfSingleCages: > 0, NumberOfDoubleCages: > 0 })
        {
            throw new InvalidDatabaseStateException("Invalid rented single and double cage state.");
        }

        return cagesRec;
    }

    private void UpdateAvailableTypesIds(RentedCage cage, CagesRec usedCagesRec,
        Dictionary<RentedType, HashSet<int>> availableTypesIds)
    {
        List<RentedTypeEntity> list = cage.RentedTypes
            .Where(rentedTypeEntity =>
                usedCagesRec.TotalNumber + (1 / (decimal)rentedTypeEntity.RentedType) <= 1).ToList();
        foreach (RentedTypeEntity? rentedTypeEntity in list)
        {
            if (!availableTypesIds.TryGetValue(rentedTypeEntity.RentedType, out _))
            {
                availableTypesIds[rentedTypeEntity.RentedType] = [];
            }

            availableTypesIds[rentedTypeEntity.RentedType].Add(rentedTypeEntity.Id);
        }
    }

    public int GetNumberOfPersonCagesPerDay(ExhibitionDay day)
    {
        int numberOfPersonCagesPerDay = day.CatDays
            .Where(x => x.ExhibitorsCage != null)
            .Select(x => x.ExhibitorsCage)
            .Distinct()
            .Count();
        return numberOfPersonCagesPerDay;
    }

    public Task<IEnumerable<CageGroupDtoDescription>> GetAvailableCageGroupTypesForGivenExhibitionDay(
        List<ExhibitionCagesInfo> cages, bool isLitter,
        CancellationToken cancellationToken = default)
    {
        List<CageGroupInfo> cageGroupInfos = cages
            .SelectMany(x => x.CageGroupInfos)
            .Where(x => x.AvailableTypes.Count > 0)
            .ToList();

        List<CageGroupDtoDescription> rentedTypesPerRegistrationToExhibition = [];
        rentedTypesPerRegistrationToExhibition.AddRange(from dictionary in cageGroupInfos
            from rentedType in dictionary.AvailableTypes.Keys
            where !isLitter || rentedType != RentedType.Single
            select new CageGroupDtoDescription
            {
                RentedTypeId =
                    new RentedCageGroup(dictionary.Height, dictionary.Length, dictionary.Width, rentedType,
                        RentingType.RentedWithZeroOtherCats),
                Ids = dictionary.AvailableTypes[rentedType]
            });

        return Task.FromResult<IEnumerable<CageGroupDtoDescription>>(rentedTypesPerRegistrationToExhibition);
    }

    public Task<IEnumerable<CageGroupDtoDescription>> GetAlreadyUsedCagesPerGivenExhibitionDay(
        int exhibitionDayId, int registrationToExhibitionId, bool isLitter, int? doNotIncludeCatRegistrationId,
        CancellationToken _ = default)
    {
        if (isLitter)
        {
            return Task.FromResult(Enumerable.Empty<CageGroupDtoDescription>());
        }

        List<CageGroupDtoDescription> rentedTypesPerRegistrationToExhibition = appContext
            .ExhibitionDays
            .Where(x => x.Id == exhibitionDayId)
            .SelectMany(x => x.CagesForRent)
            .SelectMany(x => x.RentedTypes)
            .Where(x => x.CatDays.Any(day =>
                    day.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId &&
                    day.ExhibitionDayId == exhibitionDayId
                    && day.CatRegistrationId != doNotIncludeCatRegistrationId
                )
            )
            .Distinct()
            .Include(x => x.CatDays)
            .ThenInclude(x => x.CatRegistration)
            .ThenInclude(x => x.Litter)
            .Include(x => x.RentedCage)
            .AsNoTracking()
            .AsEnumerable()
            .Select(x => new
            {
                rentedType = x.RentedType,
                days = x.CatDays.Count(
                    day => day.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId &&
                           day.ExhibitionDayId == exhibitionDayId &&
                           (doNotIncludeCatRegistrationId == null ||
                            day.CatRegistration.Id != doNotIncludeCatRegistrationId.Value)),
                hasLitter = x.CatDays.Where(day => day.ExhibitionDayId == exhibitionDayId)
                    .Where(catDay => catDay.CatRegistrationId != doNotIncludeCatRegistrationId)
                    .Any(catDay => catDay.CatRegistration.Litter != null),
                entity = x
            })
            .Where(x => IsAvailable(x.rentedType, x.days, x.hasLitter))
            .GroupBy(x => new
            {
                x.entity.RentedCage.Width,
                x.entity.RentedCage.Height,
                x.entity.RentedCage.Length,
                x.entity.RentedType,
                x.days
            })
            .Select(x =>
                new CageGroupDtoDescription
                {
                    RentedTypeId =
                        new RentedCageGroup(x.Key.Height, x.Key.Length, x.Key.Width, x.Key.RentedType,
                            x.Key.days switch
                            {
                                1 => RentingType.RentedWithOneOtherCat,
                                2 => RentingType.RentedWithTwoOtherCats,
                                _ => throw new InvalidDatabaseStateException("Cage is too small to be rented.")
                            }
                        ),
                    Ids = x.Select(y => y.entity.Id).ToHashSet()
                }
            )
            .ToList();

        return Task.FromResult<IEnumerable<CageGroupDtoDescription>>(rentedTypesPerRegistrationToExhibition);

        bool IsAvailable(RentedType cage, int catRegistrationsCount, bool cageHasLitter)
        {
            if (cageHasLitter)
            {
                return false;
            }

            bool isAvailable = (cage == RentedType.Single &&
                                catRegistrationsCount < SingleCageMaxRegistrationToExhibitionCount)
                               || (cage == RentedType.Double &&
                                   catRegistrationsCount < DoubleCageMaxRegistrationToExhibitionCount);

            return isAvailable;
        }
    }

    public async Task<IEnumerable<CageDtoDescription>> GetOwnFreeCages(int exhibitionDayId,
        int registrationToExhibitionId, bool isLitter, int? doNotIncludeCatRegistration,
        CancellationToken cancellationToken = default)
    {
        List<PersonCage> personCages = await appContext.Cages
            .Where(x => x.RegistrationToExhibitionId == registrationToExhibitionId &&
                        x.CatDays.Any(day => day.ExhibitionDayId == exhibitionDayId))
            .Include(x => x.CatDays)
            .ThenInclude(x => x.CatRegistration)
            .ThenInclude(x => x.Litter)
            .ToListAsync(cancellationToken);

        IEnumerable<CageDtoDescription> rentedTypesPerRegistrationToExhibition = personCages
            .Where(x => IsAvailable(x,
                x.CatDays.Count(day =>
                    day.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId &&
                    exhibitionDayId == day.ExhibitionDayId &&
                    (doNotIncludeCatRegistration == null ||
                     day.CatRegistration.Id != doNotIncludeCatRegistration.Value)
                ),
                x.CatDays.Where(day => day.ExhibitionDayId == exhibitionDayId)
                    .Where(catDay => catDay.CatRegistrationId != doNotIncludeCatRegistration)
                    .Any(day => day.CatRegistration.Litter != null)))
            .Select(x =>
                new CageDtoDescription { CageId = x.Id, Description = $"Moje klec {x.Length}x{x.Width}" })
            .ToList();

        return rentedTypesPerRegistrationToExhibition;


        bool IsAvailable(PersonCage cage, int catRegistrationsCount, bool cageHasLitter)
        {
            if (cageHasLitter)
            {
                return false;
            }

            RentedType rentedType = CagesService.GetCageTypeForCage(mapper.Map<CreateCageDto>(cage));
            if (isLitter)
            {
                return rentedType == RentedType.Double && catRegistrationsCount == 0;
            }

            bool result = !((rentedType == RentedType.Single &&
                             catRegistrationsCount >= SingleCageMaxRegistrationToExhibitionCount)
                            || (rentedType == RentedType.Double &&
                                catRegistrationsCount >= DoubleCageMaxRegistrationToExhibitionCount));

            return result;
        }
    }

    public int GetNumberOfCats(int exhibitionDayId, int registrationToExhibitionId)
    {
        return GetNumberOfCages(
            exhibitionDayId,
            registrationToExhibitionId,
            _ => true,
            x => x.CatRegistrationId);
    }


    public int GetNumberOfOwnCages(int exhibitionDayId, int registrationToExhibitionId)
    {
        return GetNumberOfCages(
            exhibitionDayId,
            registrationToExhibitionId,
            x => x.ExhibitorsCage != null,
            x => x.ExhibitorsCage);
    }

    public CagesRec GetNumberOfRentedCages(int exhibitionDayId, int registrationToExhibitionId)
    {
        CagesRec cagesRec = new() { NumberOfDoubleCages = 0, NumberOfSingleCages = 0 };
        IEnumerable<CatDay> catDays = appContext.CatDays
            .Where(x => x.ExhibitionDayId == exhibitionDayId)
            .Where(x => x.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId)
            .Where(x => x.RentedCageTypeId != null)
            .Include(x => x.RentedTypeEntity)
            .AsEnumerable();

        HashSet<int> ids = [];
        foreach (CatDay? day in catDays.Where(day =>
                     !ids.Contains((int)day.RentedCageTypeId!) ||
                     day.RentedTypeEntity!.RentedType != RentedType.Double))
        {
            ids.Add(day.RentedCageTypeId!.Value);
            if (day.RentedTypeEntity!.RentedType == RentedType.Single)
            {
                cagesRec.NumberOfSingleCages += 1;
            }
            else
            {
                cagesRec.NumberOfDoubleCages += 1;
            }
        }

        return cagesRec;
    }

    private int GetNumberOfCages(
        int exhibitionDayId,
        int registrationToExhibitionId,
        Func<CatDay, bool> wherePredicate,
        Func<CatDay, int?> selectPredicate)
    {
        return appContext.CatDays
            .Where(x => x.ExhibitionDayId == exhibitionDayId)
            .Where(x => x.CatRegistration.RegistrationToExhibitionId == registrationToExhibitionId)
            .AsEnumerable()
            .Where(wherePredicate)
            .Select(selectPredicate)
            .Distinct()
            .Count();
    }
}
