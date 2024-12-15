#region

using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Enums;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Organizations;

public interface IOrganizationService
{
    /// <summary>
    ///     Create organization
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<int>> CreateOrganization(CreateOrganizationDto model,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update organization
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> UpdateOrganization(UpdateOrganizationDto model, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get organization by id
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<OrganizationDto> GetOrganizationByIdAsync(int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get organizations
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="isConfirmed"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    Task<PaginatedList<OrganizationDto>> GetOrganizations(int pageNumber, int pageSize,
        OrganizationConfirmationStatus isConfirmed,
        GetOrganizationsModel? parameters = null);

    /// <summary>
    ///     Confirm organization
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> ConfirmOrganization(int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete organization
    /// </summary>
    /// <param name="organizationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> DeleteOrganization(int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Is organization administrator
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    public Task<bool> IsOrganizationAdministrator(string userId, int organizationId);
}
