#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitors.Validators;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Users.Command;

// ReSharper disable always UnusedType.Global
public record DeletePersonalDataCommand : IRequest<Result>
{
    public required string UserId { get; init; } = null!;
}

public class
    DeletePersonalDataCommandValidator : BaseExhibitor.BaseExhibitorValidator<DeletePersonalDataCommand>
{
    public DeletePersonalDataCommandValidator(UserManager<ApplicationUser> userManager)
    {
        RuleFor(v => v.UserId).ForeignKeyValidator()
            .MustAsync(async (userId, _) =>
            {
                ApplicationUser? user = await userManager.FindByIdAsync(userId);
                return user != null;
            }).WithMessage("User does not exist");
    }
}

public class DeletePersonalDataCommandCommandHandler(
    IUserService userService,
    IUser user,
    IAuthorizationService authorizationService)
    : IRequestHandler<DeletePersonalDataCommand, Result>
{
    public async Task<Result> Handle(DeletePersonalDataCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeOwnDataId(request.UserId),
                Operations.Delete);
        Guard.Against.UnAuthorized(authorizationResult);

        Result result = await userService.DeletePersonalDataAsync(request.UserId, cancellationToken);
        return result;
    }
}
