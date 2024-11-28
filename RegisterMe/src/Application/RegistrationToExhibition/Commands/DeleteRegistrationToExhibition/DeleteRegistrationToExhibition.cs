#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Commands.DeleteRegistrationToExhibition;

// ReSharper disable always UnusedType.Global
public record DeleteRegistrationToExhibitionCommand : IRequest<Result>
{
    public required int RegistrationToExhibitionId { get; init; }
}

public class DeleteRegistrationToExhibitionCommandValidator : AbstractValidator<DeleteRegistrationToExhibitionCommand>
{
    public DeleteRegistrationToExhibitionCommandValidator()
    {
        RuleFor(v => v.RegistrationToExhibitionId).ForeignKeyValidator();
    }
}

public class DeleteRegistrationToExhibitionCommandHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    IUser user) : IRequestHandler<DeleteRegistrationToExhibitionCommand, Result>
{
    public async Task<Result> Handle(DeleteRegistrationToExhibitionCommand request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId),
                Operations.Delete);
        Guard.Against.UnAuthorized(authorizationResult);
        Result result =
            await registrationToExhibitionService.DeleteRegistration(request.RegistrationToExhibitionId,
                cancellationToken);
        return result;
    }
}
