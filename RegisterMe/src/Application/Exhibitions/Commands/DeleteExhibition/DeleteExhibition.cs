#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.DeleteExhibition;

// ReSharper disable always UnusedType.Global
public record DeleteUnpublishedExhibitionCommand : IRequest<Result>
{
    public required int ExhibitionId { get; init; }
}

public class DeleteExhibitionCommandValidator : AbstractValidator<DeleteUnpublishedExhibitionCommand>
{
    public DeleteExhibitionCommandValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class DeleteExhibitionCommandHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user) : IRequestHandler<DeleteUnpublishedExhibitionCommand, Result>
{
    public async Task<Result> Handle(DeleteUnpublishedExhibitionCommand request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.ExhibitionId), Operations.Delete);
        Guard.Against.UnAuthorized(authResult);

        Result result = await exhibitionService.DeleteUnpublishedExhibition(request.ExhibitionId, cancellationToken);
        return result;
    }
}
