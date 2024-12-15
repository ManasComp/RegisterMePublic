#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetExhibitionGroupById;

// ReSharper disable always UnusedType.Global
public record GetExhibitionGroupByIdQuery : IRequest<Result<BigPriceDto>>
{
    public string GroupsId { get; init; } = null!;
}

public class GetExhibitionGroupByIdQueryValidator : AbstractValidator<GetExhibitionGroupByIdQuery>
{
    public GetExhibitionGroupByIdQueryValidator()
    {
        RuleFor(x => x.GroupsId).NotNull();
    }
}

public class GetExhibitionGroupByIdQueryHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetExhibitionGroupByIdQuery, Result<BigPriceDto>>
{
    public async Task<Result<BigPriceDto>> Handle(GetExhibitionGroupByIdQuery request,
        CancellationToken cancellationToken)
    {
        Result<BigPriceDto> group = await exhibitionService.GetPricesGroupsById(request.GroupsId, cancellationToken);
        if (group.IsFailure)
        {
            return group;
        }

        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(group.Value.ExhibitionId), Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(result);

        return group;
    }
}
