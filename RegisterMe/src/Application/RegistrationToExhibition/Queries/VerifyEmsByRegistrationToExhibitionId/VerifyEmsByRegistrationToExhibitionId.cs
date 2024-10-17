#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationsToExhibitionByExhibitorId;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Queries.VerifyEmsByRegistrationToExhibitionId;

// ReSharper disable always UnusedType.Global
public record HasValidEmsQuery : IRequest<bool>
{
    public required int RegistrationToExhibitionId { get; init; }
}

public class
    HasValidEmsQueryValidator : AbstractValidator<
    GetRegistrationsToExhibitionByExhibitorIdQuery>
{
    public HasValidEmsQueryValidator()
    {
        RuleFor(v => v.ExhibitorId).ForeignKeyValidator();
    }
}

public class HasValidEmsQueryHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user) : IRequestHandler<HasValidEmsQuery, bool>
{
    public async Task<bool> Handle(
        HasValidEmsQuery request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult1 = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId),
                Operations.Read);
        Guard.Against.UnAuthorized(authorizationResult1);

        return await registrationToExhibitionService.HasValidEms(request.RegistrationToExhibitionId,
            cancellationToken);
    }
}
