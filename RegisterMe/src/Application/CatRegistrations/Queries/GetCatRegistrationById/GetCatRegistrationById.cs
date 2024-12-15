#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.CatRegistrations.Queries.GetCatRegistrationById;

// ReSharper disable always UnusedType.Global
public record GetCatRegistrationByIdQuery : IRequest<CatRegistrationDto>
{
    public required int Id { get; init; }
}

public class GetCatRegistrationByIdQueryValidator : AbstractValidator<GetCatRegistrationByIdQuery>
{
    public GetCatRegistrationByIdQueryValidator()
    {
        RuleFor(v => v.Id).ForeignKeyValidator();
    }
}

public class GetCatRegistrationByIdQueryHandler(
    ICatRegistrationService catRegistrationService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetCatRegistrationByIdQuery, CatRegistrationDto>
{
    public async Task<CatRegistrationDto> Handle(GetCatRegistrationByIdQuery request,
        CancellationToken cancellationToken)
    {
        CatRegistrationDto catRegistrationById =
            await catRegistrationService.GetCatRegistrationById(request.Id, cancellationToken);

        int registrationToExhibitionId = catRegistrationById.RegistrationToExhibitionId;
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
                Operations.Read);
        Guard.Against.UnAuthorized(authorizationResult);

        return catRegistrationById;
    }
}
