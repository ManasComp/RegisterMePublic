#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetPrices;

// ReSharper disable always UnusedType.Global
public record GetPricesQuery : IRequest<List<BigPriceDto>>
{
    public required int ExhibitionId { get; init; }
}

public class GetPricesQueryValidator : AbstractValidator<GetPricesQuery>
{
    public GetPricesQueryValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class GetPricesQueryHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetPricesQuery, List<BigPriceDto>>
{
    public async Task<List<BigPriceDto>> Handle(GetPricesQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.ExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return await exhibitionService.GetPricesGroupsByExhibitionId(request.ExhibitionId, cancellationToken);
    }
}
