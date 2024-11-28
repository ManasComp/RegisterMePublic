#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetExhibitionCagesInfo;

// ReSharper disable always UnusedType.Global
public record GetExhibitionCagesInfoQuery : IRequest<List<ExhibitionCagesInfo>>
{
    public required int ExhibitionId { get; init; }
    public int? ExhibitionDayId { get; init; }
    public int? DoNotIncludeCatRegistrationId { get; init; }
}

public class GetExhibitionCagesInfoQueryValidator : AbstractValidator<GetExhibitionCagesInfoQuery>
{
    public GetExhibitionCagesInfoQueryValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
        RuleFor(x => x.ExhibitionDayId).OptionalForeignKeyValidator();
        RuleFor(x => x.DoNotIncludeCatRegistrationId).OptionalForeignKeyValidator();
    }
}

public class GetExhibitionCagesInfoQueryHandler(
    ICagesService cagesService,
    IAuthorizationService authorizationService,
    IUser user
)
    : IRequestHandler<GetExhibitionCagesInfoQuery, List<ExhibitionCagesInfo>>
{
    public async Task<List<ExhibitionCagesInfo>> Handle(GetExhibitionCagesInfoQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.ExhibitionId), Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(result);

        // other ids are validated inside
        return await cagesService.GetExhibitionCagesInfo(request.ExhibitionId, request.ExhibitionDayId,
            request.DoNotIncludeCatRegistrationId, cancellationToken);
    }
}
