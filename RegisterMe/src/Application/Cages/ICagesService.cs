#region

using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Cages.Dtos.Combination;
using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Cages;

/// <summary>
///     Manages group cages and normal cages and provide helper methods for them
/// </summary>
public interface ICagesService
{
    /// <summary>
    ///     Get rented cage by id
    /// </summary>
    /// <param name="rentedCageId"></param>
    /// <returns></returns>
    public Task<BriefCageDto> GetRentedCageByGroupId(string rentedCageId);

    /// <summary>
    ///     Delete rented cages
    /// </summary>
    /// <param name="rentedCagesId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> DeleteRentedCages(string rentedCagesId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get rented cages by exhibition id
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <returns></returns>
    public Task<List<BriefCageDto>> GetRentedCagesByExhibitionId(int exhibitionId);

    /// <summary>
    ///     Get cage has is registered to
    /// </summary>
    /// <param name="catRegistrationDayId"></param>
    /// <param name="exhibitionDayId"></param>
    /// <param name="rentedTypeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<RentedCageGroup?> GetCageHashIsAreRegisteredTo(int catRegistrationDayId, int exhibitionDayId,
        int? rentedTypeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get person cage by id
    /// </summary>
    /// <param name="cageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<CageDto> GetPersonCageById(int cageId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update cage group
    /// </summary>
    /// <param name="rentedCageIds"></param>
    /// <param name="rentedCage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result<string>> UpdateCageGroup(string rentedCageIds, CreateRentedRentedCageDto rentedCage,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Record with given exhibition day id and cat registration id exist with cage id
    /// </summary>
    /// <param name="cageId"></param>
    /// <param name="exhibitionDayId"></param>
    /// <param name="catRegistrationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageId(int cageId, int exhibitionDayId,
        int catRegistrationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get person cages by registration to exhibition id
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<PersonCageWithCatDays>> GetPersonCagesByRegistrationToExhibitionId(
        int registrationToExhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete unused person cages
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task DeleteUnusedPersonCages(int registrationToExhibitionId, CancellationToken cancellationToken = default);


    /// <summary>
    ///     Get cat registration id by cage id
    /// </summary>
    /// <param name="cageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<int> GetRegistrationToExhibitionIdByCageId(int cageId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create rented cage
    /// </summary>
    /// <param name="rentedCage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<string>> CreateRentedCages(CreateRentedRentedCageDto rentedCage,
        CancellationToken cancellationToken = default);


    /// <summary>
    ///     Get person cages by exhibition day
    /// </summary>
    /// <param name="exhibitionDayId"></param>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<CageDto>> GetPersonCagesByExhibitionDay(int exhibitionDayId,
        int? registrationToExhibitionId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get available cage group types and own cages
    /// </summary>
    /// <param name="exhibitionDayId"></param>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="isLitter"></param>
    /// <param name="doNotIncludeCatRegistrationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<CagesPerDayDto> GetAvailableCageGroupTypesAndOwnCages(int exhibitionDayId,
        int registrationToExhibitionId, bool isLitter, int? doNotIncludeCatRegistrationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get exhibition cages info
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="exhibitionDayId"></param>
    /// <param name="doNotIncludeCatRegistrationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<ExhibitionCagesInfo>> GetExhibitionCagesInfo(int exhibitionId,
        int? exhibitionDayId = null, int? doNotIncludeCatRegistrationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get random rented cage type id from hash
    /// </summary>
    /// <param name="cageHash"></param>
    /// <param name="exhibitionDayId"></param>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result<int?>> GetRandomRentedCageTypeIdFromHash(RentedCageGroup? cageHash, int exhibitionDayId,
        int registrationToExhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get statistics about registration to exhibition
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<CatRegistrationStatistics>> GetStatisticsAboutRegistrationToExhibition(
        int registrationToExhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get rented types
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<RentedTypeEntity>> GetRentedTypes(int exhibitionId, int registrationToExhibitionId,
        CancellationToken cancellationToken = default);
}
