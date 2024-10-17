#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Enums;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetExhibitions;

// ReSharper disable always UnusedType.Global
public record GetExhibitionsQuery : IRequest<PaginatedList<ExhibitionDto>>
{
    public required int PageNumber { get; init; } = 1;
    public required int PageSize { get; init; } = 10;
    public required int? OrganizationId { get; init; }
    public required string? UserId { get; init; }
    public required string? SearchString { get; init; }

    public required OrganizationPublishStatus OrganizationPublishStatus { get; init; } =
        OrganizationPublishStatus.Published;

    public required ExhibitionRegistrationStatus ExhibitionStatus { get; init; } = ExhibitionRegistrationStatus.All;
}

public class GetExhibitionsQueryValidator : AbstractValidator<GetExhibitionsQuery>
{
    public GetExhibitionsQueryValidator()
    {
        RuleFor(e => e).ValidPagination(e => e.PageNumber, e => e.PageSize);
        RuleFor(x => x.OrganizationId).OptionalForeignKeyValidator();
        RuleFor(x => x.UserId).OptionalForeignKeyValidator();
        RuleFor(x => x.SearchString).MaximumLength(50);
        RuleFor(x => x.OrganizationPublishStatus).IsInEnum();
        RuleFor(x => x.ExhibitionStatus).IsInEnum();
    }
}

public class GetExhibitionsQueryHandler(
    IExhibitionService exhibitionService,
    IUser user,
    IAuthorizationService authorizationService)
    : IRequestHandler<GetExhibitionsQuery, PaginatedList<ExhibitionDto>>
{
    public async Task<PaginatedList<ExhibitionDto>> Handle(GetExhibitionsQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult? authorizationResult = null;
        if (request.OrganizationPublishStatus is OrganizationPublishStatus.Published)
        {
            if (request.UserId != null)
            {
                authorizationResult = await authorizationService
                    .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                        new AuthorizeOwnDataId(request.UserId), Operations.Read);
            }
        }
        else
        {
            if (request.OrganizationId != null)
            {
                authorizationResult = await authorizationService
                    .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                        new AuthorizeOrganizationId(request.OrganizationId),
                        Operations
                            .Update); // there is update because viewing unpublished exhibitions is considered as update
            }
            else
            {
                authorizationResult = await authorizationService
                    .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                        new AuthorizeExhibitionId(null),
                        Operations.DoSuperAdminStuff);
            }
        }

        if (authorizationResult != null)
        {
            Guard.Against.UnAuthorized(authorizationResult);
        }


        ExhibitionsFilterDto exhibitionsFilterDto = new()
        {
            OrganizationId = request.OrganizationId,
            UserId = request.UserId,
            SearchString = request.SearchString,
            ExhibitionRegistrationStatus = request.ExhibitionStatus,
            OrganizationPublishStatus = request.OrganizationPublishStatus
        };
        return await exhibitionService.GetExhibitions(request.PageNumber, request.PageSize, exhibitionsFilterDto);
    }
}
