#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Queries.HasActiveRegistrations;

// ReSharper disable always UnusedType.Global
public class HasActiveRegistrationsQuery : IRequest<bool>
{
    public bool? Paid { get; init; }
    public required int ExhibitionId { get; init; }
    public required string UserId { get; init; } = null!;
}

public class HasActiveRegistrationsQueryValidator : AbstractValidator<HasActiveRegistrationsQuery>
{
    public HasActiveRegistrationsQueryValidator()
    {
        RuleFor(v => v.ExhibitionId).ForeignKeyValidator();
        RuleFor(v => v.UserId).ForeignKeyValidator();
    }
}

public class HasActiveRegistrationsQueryHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<HasActiveRegistrationsQuery, bool>
{
    public async Task<bool> Handle(HasActiveRegistrationsQuery request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.ExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(authorizationResult);

        AuthorizationResult authorizationResult1 = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeOwnDataId(request.UserId),
                Operations.Read);
        Guard.Against.UnAuthorized(authorizationResult1);

        return await registrationToExhibitionService.HasActiveRegistrations(request.ExhibitionId, request.UserId,
            request.Paid, cancellationToken);
    }
}
