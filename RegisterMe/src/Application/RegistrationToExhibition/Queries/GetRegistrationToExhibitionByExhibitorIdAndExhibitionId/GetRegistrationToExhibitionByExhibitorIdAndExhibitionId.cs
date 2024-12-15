#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Queries.
    GetRegistrationToExhibitionByExhibitorIdAndExhibitionId;

// ReSharper disable always UnusedType.Global
public record GetRegistrationToExhibitionByExhibitorIdAndExhibitionIdQuery : IRequest<RegistrationToExhibitionDto?>
{
    public required int ExhibitorId { get; init; }
    public required int ExhibitionId { get; init; }
}

public class
    GetRegistrationToExhibitionByExhibitorIdAndExhibitionIdQueryValidator : AbstractValidator<
    GetRegistrationToExhibitionByExhibitorIdAndExhibitionIdQuery>
{
    public GetRegistrationToExhibitionByExhibitorIdAndExhibitionIdQueryValidator()
    {
        RuleFor(v => v.ExhibitorId).ForeignKeyValidator();
        RuleFor(v => v.ExhibitionId).ForeignKeyValidator();
    }
}

public class GetRegistrationToExhibitionByExhibitorIdAndExhibitionIdQueryHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetRegistrationToExhibitionByExhibitorIdAndExhibitionIdQuery, RegistrationToExhibitionDto?>
{
    public async Task<RegistrationToExhibitionDto?> Handle(
        GetRegistrationToExhibitionByExhibitorIdAndExhibitionIdQuery request, CancellationToken cancellationToken)
    {
        RegistrationToExhibitionDto? resource =
            await registrationToExhibitionService.GetRegistrationToExhibitionByExhibitorIdAndExhibitionId(
                request.ExhibitionId, request.ExhibitorId, cancellationToken);
        if (resource == null)
        {
            return null;
        }

        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeRegistrationToExhibitionId(resource.Id), Operations.Read);
        Guard.Against.UnAuthorized(authorizationResult);

        AuthorizationResult authorizationResult1 = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(resource.ExhibitionId),
                Operations.Read);
        Guard.Against.UnAuthorized(authorizationResult1);

        return resource;
    }
}
