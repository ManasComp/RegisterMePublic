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

namespace RegisterMe.Application.Exhibitions.Queries.GetCagesStatistics;

// ReSharper disable always UnusedType.Global
public record GetCagesStatisticsQuery : IRequest<List<CatRegistrationStatistics>>
{
    public required int RegistrationToExhibitionId { get; init; }
}

public class GetCagesStatisticsQueryValidator : AbstractValidator<GetCagesStatisticsQuery>
{
    public GetCagesStatisticsQueryValidator()
    {
        RuleFor(v => v.RegistrationToExhibitionId).ForeignKeyValidator();
    }
}

public class GetCagesStatisticsQueryHandler(
    ICagesService cagesService,
    IAuthorizationService authorizationService,
    IUser user) : IRequestHandler<GetCagesStatisticsQuery, List<CatRegistrationStatistics>>
{
    public async Task<List<CatRegistrationStatistics>> Handle(GetCagesStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return await cagesService.GetStatisticsAboutRegistrationToExhibition(request.RegistrationToExhibitionId,
            cancellationToken);
    }
}
