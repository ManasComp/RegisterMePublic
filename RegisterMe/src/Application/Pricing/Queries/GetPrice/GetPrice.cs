#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Pricing.Dtos;

#endregion

namespace RegisterMe.Application.Pricing.Queries.GetPrice;

// ReSharper disable always UnusedType.Global
public record GetPriceQuery : IRequest<RegistrationToExhibitionPrice>
{
    public required int RegistrationToExhibitionId { get; init; }
}

public class GetPriceQueryValidator : AbstractValidator<GetPriceQuery>
{
    public GetPriceQueryValidator()
    {
        RuleFor(v => v.RegistrationToExhibitionId).ForeignKeyValidator();
    }
}

public class GetPriceQueryHandler(IPricingFacade pricingFacade, IAuthorizationService authorizationService, IUser user)
    : IRequestHandler<GetPriceQuery, RegistrationToExhibitionPrice>
{
    public async Task<RegistrationToExhibitionPrice> Handle(GetPriceQuery request, CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return await pricingFacade.GetPrice(request.RegistrationToExhibitionId, cancellationToken);
    }
}
