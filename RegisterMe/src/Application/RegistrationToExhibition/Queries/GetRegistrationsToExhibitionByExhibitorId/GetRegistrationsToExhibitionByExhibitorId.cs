#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationsToExhibitionByExhibitorId;

// ReSharper disable always UnusedType.Global
public record GetRegistrationsToExhibitionByExhibitorIdQuery : IRequest<List<RegistrationToExhibitionDto>>
{
    public required int ExhibitorId { get; init; }
}

public class
    GetRegistrationsToExhibitionByExhibitorIdQueryValidator : AbstractValidator<
    GetRegistrationsToExhibitionByExhibitorIdQuery>
{
    public GetRegistrationsToExhibitionByExhibitorIdQueryValidator()
    {
        RuleFor(v => v.ExhibitorId).ForeignKeyValidator();
    }
}

public class GetRegistrationsToExhibitionByExhibitorIdQueryHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user) : IRequestHandler<GetRegistrationsToExhibitionByExhibitorIdQuery, List<RegistrationToExhibitionDto>>
{
    public async Task<List<RegistrationToExhibitionDto>> Handle(
        GetRegistrationsToExhibitionByExhibitorIdQuery request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitorId(request.ExhibitorId),
                Operations.OnlyOwnerCanDo);
        Guard.Against.UnAuthorized(authorizationResult);

        return await registrationToExhibitionService.GetRegistrationsToExhibitionByExhibitorId(request.ExhibitorId,
            cancellationToken);
    }
}
