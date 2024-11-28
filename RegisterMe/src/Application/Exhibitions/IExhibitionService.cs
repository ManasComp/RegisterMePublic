#region

using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Services.Groups;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions;

public interface IExhibitionService
{
    /// <summary>
    ///     Updates exhibition
    /// </summary>
    /// <param name="briefExhibitionDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> UpdateExhibition(UpdateExhibitionDto briefExhibitionDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes unpublished exhibition
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> DeleteUnpublishedExhibition(int exhibitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Publish exhibition
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> PublishExhibition(int exhibitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create exhibition
    /// </summary>
    /// <param name="exhibitionDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result<int>> CreateExhibition(CreateExhibitionDto exhibitionDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get exhibitions
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    Task<PaginatedList<ExhibitionDto>> GetExhibitions(int pageNumber, int pageSize,
        ExhibitionsFilterDto? parameters = null);

    /// <summary>
    ///     Get exhibition by id
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BriefExhibitionDto> GetExhibitionById(int exhibitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get advertisement price
    /// </summary>
    /// <param name="advertisementId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MultiCurrencyPrice> GetAdvertisementPrice(int advertisementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Cancel exhibition
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> CancelExhibition(int exhibitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get groups can be registered in
    /// </summary>
    /// <param name="catRegistration"></param>
    /// <param name="exhibitionDayId"></param>
    /// <returns></returns>
    public Task<List<DatabaseGroupDto>> GetGroupsCatRegistrationCanBeRegisteredIn(
        LitterOrExhibitedCatDto catRegistration,
        int exhibitionDayId);

    /// <summary>
    ///     Get advertisement by exhibition id
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<AdvertisementDto>> GetAdvertisementsByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get exhibition days by exhibition id
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<ExhibitionDayDto>> GetExhibitionDayByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create advertisement
    /// </summary>
    /// <param name="advertisementDto"></param>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result<int>> CreateAdvertisement(UpsertAdvertisementDto advertisementDto, int exhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete advertisement
    /// </summary>
    /// <param name="advertisementId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> DeleteAdvertisement(int advertisementId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update advertisement
    /// </summary>
    /// <param name="advertisementDto"></param>
    /// <param name="advertisementId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> UpdateAdvertisement(UpsertAdvertisementDto advertisementDto, int advertisementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get advertisement by id
    /// </summary>
    /// <param name="advertisementId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<AdvertisementDto> GetAdvertisementById(int advertisementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create price group
    /// </summary>
    /// <param name="groupsIds"></param>
    /// <param name="exhibitionId"></param>
    /// <param name="priceDays"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result<string>> CreatePrices(List<string> groupsIds, int exhibitionId, List<PriceDays> priceDays,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get exhibition groups that are not fully registered
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <returns></returns>
    public Task<List<DatabaseGroupDto>> GetExhibitionGroupsThatAreNotFullyRegistered(int exhibitionId);

    /// <summary>
    ///     Delete price group
    /// </summary>
    /// <param name="priceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> DeletePriceGroup(string priceId, CancellationToken cancellationToken);

    /// <summary>
    ///     Get prices groups by exhibition id
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<BigPriceDto>> GetPricesGroupsByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get prices groups by id
    /// </summary>
    /// <param name="groupsId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result<BigPriceDto>> GetPricesGroupsById(string groupsId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get groups by group id
    /// </summary>
    /// <param name="priceGroupId"></param>
    /// <returns></returns>
    public Task<List<DatabaseGroupDto>> GetGroupsByGroupId(string priceGroupId);

    /// <summary>
    ///     Update price group
    /// </summary>
    /// <param name="originalPricesIds"></param>
    /// <param name="groupsIds"></param>
    /// <param name="priceDays"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string> UpdatePriceGroup(string originalPricesIds, List<string> groupsIds,
        List<PriceDays> priceDays,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get days by group id
    /// </summary>
    /// <param name="priceGroupIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<ExhibitionDayDto>> GetDaysByGroupId(string priceGroupIds,
        CancellationToken cancellationToken = default);
}
