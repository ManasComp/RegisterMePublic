#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionByExhibitionId;

// ReSharper disable always UnusedType.Global
public record GetRegistrationsToExhibitionByExhibitionIdQuery : IRequest<List<RegistrationToExhibitionDto>>
{
    public required int ExhibitionId { get; init; }
}

public class
    GetRegistrationToExhibitionByExhibitionIdQueryValidator : AbstractValidator<
    GetRegistrationsToExhibitionByExhibitionIdQuery>
{
    public GetRegistrationToExhibitionByExhibitionIdQueryValidator()
    {
        RuleFor(v => v.ExhibitionId).ForeignKeyValidator();
    }
}

public class GetRegistrationToExhibitionByExhibitionIdQueryHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user
) : IRequestHandler<GetRegistrationsToExhibitionByExhibitionIdQuery, List<RegistrationToExhibitionDto>>
{
    public async Task<List<RegistrationToExhibitionDto>> Handle(GetRegistrationsToExhibitionByExhibitionIdQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.ExhibitionId),
                Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(authorizationResult);

        return await registrationToExhibitionService.GetRegistrationsToExhibitionByExhibitionId(request.ExhibitionId,
            cancellationToken);
    }
}
