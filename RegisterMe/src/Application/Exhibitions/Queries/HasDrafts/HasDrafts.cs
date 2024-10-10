#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.RegistrationToExhibition;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.HasDrafts;

// ReSharper disable always UnusedType.Global
public class HasDraftsQuery : IRequest<bool>
{
    public required int ExhibitionId { get; init; }
}

public class HasDraftsQueryValidator : AbstractValidator<HasDraftsQuery>
{
    public HasDraftsQueryValidator()
    {
        RuleFor(v => v.ExhibitionId).ForeignKeyValidator();
    }
}

public class HasDraftsQueryHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<HasDraftsQuery, bool>
{
    public async Task<bool> Handle(HasDraftsQuery request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.ExhibitionId), Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(authorizationResult);

        return await registrationToExhibitionService.HasDrafts(request.ExhibitionId, cancellationToken);
    }
}
