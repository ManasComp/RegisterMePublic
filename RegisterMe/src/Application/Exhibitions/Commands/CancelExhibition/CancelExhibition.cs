#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.CancelExhibition;

// ReSharper disable always UnusedType.Global
public record CancelExhibitionCommand : IRequest<Result>
{
    public required int ExhibitionId { get; init; }
}

public class CancelExhibitionCommandValidator : AbstractValidator<CancelExhibitionCommand>
{
    public CancelExhibitionCommandValidator()
    {
        RuleFor(v => v.ExhibitionId).ForeignKeyValidator();
    }
}

public class CancelExhibitionCommandHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user) : IRequestHandler<CancelExhibitionCommand, Result>
{
    public async Task<Result> Handle(CancelExhibitionCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.ExhibitionId),
                Operations.Delete);
        Guard.Against.UnAuthorized(authorizationResult);
        Result result = await exhibitionService.CancelExhibition(request.ExhibitionId, cancellationToken);

        return result;
    }
}
