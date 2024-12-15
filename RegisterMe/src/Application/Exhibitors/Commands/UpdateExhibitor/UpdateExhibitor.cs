#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Validators;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitors.Commands.UpdateExhibitor;

// ReSharper disable always UnusedType.Global
public record UpdateExhibitorCommand : IRequest<Result>
{
    public required UpsertExhibitorDto Exhibitor { get; init; } = null!;
    public required string AspNetUserId { get; init; } = null!;
}

public class UpdateExhibitorCommandValidator : BaseExhibitor.BaseExhibitorValidator<UpdateExhibitorCommand>
{
    public UpdateExhibitorCommandValidator()
    {
        AddCommonRules(v => v.Exhibitor);
        RuleFor(v => v.AspNetUserId).ForeignKeyValidator();
    }
}

public class UpdateExhibitorCommandHandler(
    IExhibitorService exhibitorService,
    IUser user,
    IAuthorizationService authorizationService) : IRequestHandler<UpdateExhibitorCommand, Result>
{
    public async Task<Result> Handle(UpdateExhibitorCommand request, CancellationToken cancellationToken)
    {
        ExhibitorAndUserDto? exhibitorId =
            await exhibitorService.GetExhibitorByUserId(request.AspNetUserId, cancellationToken);
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitorId(exhibitorId?.Id),
                Operations.Create);
        Guard.Against.UnAuthorized(authorizationResult);

        Result result =
            await exhibitorService.UpdateExhibitor(request.Exhibitor, request.AspNetUserId, cancellationToken);
        return result;
    }
}
