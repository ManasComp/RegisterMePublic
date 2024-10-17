#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.Pricing.Queries.GetBeneficiaryMessage;

// ReSharper disable always UnusedType.Global
public record GetBeneficiaryMessageQuery : IRequest<string>
{
    public required int RegistrationToExhibitionId { get; init; }
}

public class GetBeneficiaryMessageQueryValidator : AbstractValidator<GetBeneficiaryMessageQuery>
{
    public GetBeneficiaryMessageQueryValidator()
    {
        RuleFor(v => v.RegistrationToExhibitionId).ForeignKeyValidator();
    }
}

public class GetBeneficiaryMessageQueryHandler(
    IPricingFacade pricingFacade,
    IUser user,
    IAuthorizationService authorizationService) : IRequestHandler<GetBeneficiaryMessageQuery, string>
{
    public async Task<string> Handle(GetBeneficiaryMessageQuery request, CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return await pricingFacade.GetBeneficiaryMessage(request.RegistrationToExhibitionId);
    }
}
