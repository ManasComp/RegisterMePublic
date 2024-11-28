#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.Cages.Queries.GetPersonCageById;

// ReSharper disable always UnusedType.Global
public record GetPersonCageByIdQuery : IRequest<CageDto>
{
    public required int PersonCageId { get; init; }
}

public class GetPersonCageByIdQueryValidator : AbstractValidator<GetPersonCageByIdQuery>
{
    public GetPersonCageByIdQueryValidator()
    {
        RuleFor(x => x.PersonCageId).ForeignKeyValidator();
    }
}

public class GetPersonCageByIdQueryHandler(
    ICagesService cagesService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetPersonCageByIdQuery, CageDto>
{
    public async Task<CageDto> Handle(GetPersonCageByIdQuery request, CancellationToken cancellationToken)
    {
        int registrationToExhibitionId =
            await cagesService.GetRegistrationToExhibitionIdByCageId(request.PersonCageId, cancellationToken);
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId), Operations.Read);

        Guard.Against.UnAuthorized(authResult);

        CageDto result = await cagesService.GetPersonCageById(request.PersonCageId, cancellationToken);
        return result;
    }
}
