#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.CatRegistrations.Queries.GetUserCatsNotInExhibition;

// ReSharper disable always UnusedType.Global
public record GetUserCatsNotInExhibitionQuery : IRequest<List<CatModelP>>
{
    public required int RegistrationToExhibitionId { get; init; }
    public required CatRegistrationType Type { get; init; }
}

public class GetUserCatsNotInExhibitionQueryValidator : AbstractValidator<GetUserCatsNotInExhibitionQuery>
{
    public GetUserCatsNotInExhibitionQueryValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.RegistrationToExhibitionId).ForeignKeyValidator();
    }
}

public class GetUserCatsNotInExhibitionQueryHandler(
    ICatRegistrationService catService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetUserCatsNotInExhibitionQuery, List<CatModelP>>
{
    public async Task<List<CatModelP>> Handle(GetUserCatsNotInExhibitionQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(authResult);

        List<CatModelP> result = request.Type switch
        {
            CatRegistrationType.Litter => await catService.GetUserLittersNotInExhibition(request
                .RegistrationToExhibitionId),
            CatRegistrationType.Cat => await catService.GetUserCatsNotInExhibition(request.RegistrationToExhibitionId),
            _ => throw new NotImplementedException()
        };

        return result;
    }
}
