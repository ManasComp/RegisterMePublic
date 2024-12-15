#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;

// ReSharper disable always UnusedType.Global
public record GetDaysByExhibitionIdQuery : IRequest<List<ExhibitionDayDto>>
{
    public required int ExhibitionId { get; init; }
}

public class GetDaysByExhibitionIdQueryValidator : AbstractValidator<GetDaysByExhibitionIdQuery>
{
    public GetDaysByExhibitionIdQueryValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class GetDaysByExhibitionIdQueryHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetDaysByExhibitionIdQuery, List<ExhibitionDayDto>>
{
    public async Task<List<ExhibitionDayDto>> Handle(GetDaysByExhibitionIdQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.ExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return await exhibitionService.GetExhibitionDayByExhibitionId(request.ExhibitionId, cancellationToken);
    }
}
