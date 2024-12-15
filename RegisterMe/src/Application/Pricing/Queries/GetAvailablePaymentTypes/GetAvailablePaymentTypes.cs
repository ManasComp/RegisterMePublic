#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Pricing.Enums;

#endregion

namespace RegisterMe.Application.Pricing.Queries.GetAvailablePaymentTypes;

// ReSharper disable always UnusedType.Global
public record GetAvailablePaymentTypesQuery : IRequest<List<PaymentTypeWithCurrency>>
{
    public required int RegistrationToExhibitionId { get; init; }
}

public class GetAvailablePaymentTypesQueryValidator : AbstractValidator<GetAvailablePaymentTypesQuery>
{
    public GetAvailablePaymentTypesQueryValidator()
    {
        RuleFor(v => v.RegistrationToExhibitionId).GreaterThan(0);
    }
}

public class GetAvailablePaymentTypesQueryHandler(
    IPricingFacade pricingFacade,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetAvailablePaymentTypesQuery, List<PaymentTypeWithCurrency>>
{
    public async Task<List<PaymentTypeWithCurrency>> Handle(GetAvailablePaymentTypesQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return await pricingFacade.GetAvailablePaymentTypes(request.RegistrationToExhibitionId);
    }
}
