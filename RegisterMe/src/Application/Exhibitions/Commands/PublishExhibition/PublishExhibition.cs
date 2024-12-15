#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.PublishExhibition;

// ReSharper disable always UnusedType.Global
public record PublishExhibitionCommand : IRequest<Result>
{
    public required int ExhibitionId { get; init; }
}

public class PublishExhibitionCommandValidator : AbstractValidator<PublishExhibitionCommand>
{
    public PublishExhibitionCommandValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class PublishExhibitionCommandHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<PublishExhibitionCommand, Result>
{
    public async Task<Result> Handle(PublishExhibitionCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.ExhibitionId),
                Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);
        Result result = await exhibitionService.PublishExhibition(request.ExhibitionId, cancellationToken);
        return result;
    }
}
