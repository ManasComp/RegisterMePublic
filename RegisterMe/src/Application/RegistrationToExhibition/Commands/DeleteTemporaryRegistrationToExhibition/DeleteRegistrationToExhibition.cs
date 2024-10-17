#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.BackgroundWorkers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Commands.DeleteTemporaryRegistrationToExhibition;

// ReSharper disable always UnusedType.Global
public record DeleteTemporaryRegistrationToExhibitionCommand : IRequest<Result>
{
    public required int ExhibitionId { get; init; }
    public required string WebAddress { get; init; }
}

public class
    DeleteTemporaryRegistrationToExhibitionCommandValidator : AbstractValidator<
    DeleteTemporaryRegistrationToExhibitionCommand>
{
    public DeleteTemporaryRegistrationToExhibitionCommandValidator()
    {
        RuleFor(v => v.ExhibitionId).ForeignKeyValidator();
    }
}

public class DeleteTemporaryRegistrationToExhibitionCommandHandler(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IAuthorizationService authorizationService,
    NotifierService notifierService,
    IUser user) : IRequestHandler<DeleteTemporaryRegistrationToExhibitionCommand, Result>
{
    public async Task<Result> Handle(DeleteTemporaryRegistrationToExhibitionCommand request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.ExhibitionId),
                Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(authorizationResult);

        Result<List<TypedEmail>> result =
            await registrationToExhibitionService.DeleteTemporaryRegistrations(request.ExhibitionId,
                cancellationToken);

        if (result.IsSuccess)
        {
            await notifierService.NotifyRegistrations(request.WebAddress, result.Value, cancellationToken);
        }

        return result;
    }
}
