#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Enums;

#endregion

namespace RegisterMe.Application.Organizations.Queries.GetOrganizations;

// ReSharper disable always UnusedType.Global
public record GetOrganizationsQuery : IRequest<PaginatedList<OrganizationDto>>
{
    public required int PageNumber { get; init; } = 1;
    public required int PageSize { get; init; } = 10;

    public required OrganizationConfirmationStatus OrganizationConfirmationStatus { get; init; } =
        OrganizationConfirmationStatus.Confirmed;

    public required string SearchString { get; init; } = "";
    public required HasExhibitions? HasExhibitions { get; init; }
}

public class GetOrganizationsQueryValidator : AbstractValidator<GetOrganizationsQuery>
{
    public GetOrganizationsQueryValidator()
    {
        RuleFor(x => x).ValidPagination(x => x.PageNumber, x => x.PageSize);
        RuleFor(x => x.SearchString).MaximumLength(50);
    }
}

public class GetOrganizationsQueryHandler(
    IOrganizationService organizationService,
    IUser user,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetOrganizationsQuery, PaginatedList<OrganizationDto>>
{
    public async Task<PaginatedList<OrganizationDto>> Handle(GetOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.OrganizationConfirmationStatus is not OrganizationConfirmationStatus.Confirmed)
        {
            AuthorizationResult authorizationResult2 = await authorizationService
                .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                    new AuthorizeOrganizationId(null),
                    Operations.DoSuperAdminStuff);
            Guard.Against.UnAuthorized(authorizationResult2);
        }

        return await organizationService.GetOrganizations(request.PageNumber, request.PageSize,
            request.OrganizationConfirmationStatus,
            new GetOrganizationsModel { SearchString = request.SearchString, HasExhibitions = request.HasExhibitions });
    }
}
