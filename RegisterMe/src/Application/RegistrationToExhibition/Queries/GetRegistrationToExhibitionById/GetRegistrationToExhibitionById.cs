#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;

// ReSharper disable always UnusedType.Global
public record GetRegistrationToExhibitionByIdQuery : IRequest<RegistrationToExhibitionDto>
{
    public required int RegistrationToExhibitionId { get; init; }
}

public class GetRegistrationToExhibitionByIdQueryValidator : AbstractValidator<GetRegistrationToExhibitionByIdQuery>
{
    public GetRegistrationToExhibitionByIdQueryValidator()
    {
        RuleFor(v => v.RegistrationToExhibitionId).ForeignKeyValidator();
    }
}

public class GetRegistrationToExhibitionByIdQueryHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user) : IRequestHandler<GetRegistrationToExhibitionByIdQuery, RegistrationToExhibitionDto>
{
    public async Task<RegistrationToExhibitionDto> Handle(GetRegistrationToExhibitionByIdQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId),
                Operations.Read);
        Guard.Against.UnAuthorized(authorizationResult);

        RegistrationToExhibitionDto registrationToExhibitionDto =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(request.RegistrationToExhibitionId,
                cancellationToken);
        return registrationToExhibitionDto;
    }
}
