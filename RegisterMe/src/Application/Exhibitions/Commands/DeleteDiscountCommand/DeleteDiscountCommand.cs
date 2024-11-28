#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Workflows;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.DeleteDiscountCommand;

// ReSharper disable always UnusedType.Global
public record DeleteDiscountCommand : IRequest<Result>
{
    public required int Id { get; init; }
}

public class DeleteDiscountCommandCommandValidator : AbstractValidator<DeleteDiscountCommand>
{
    public DeleteDiscountCommandCommandValidator()
    {
        RuleFor(x => x.Id).ForeignKeyValidator();
    }
}

public class DeleteDiscountCommandCommandHandler(
    WorkflowService workflowService,
    IAuthorizationService authorizationService,
    IUser user,
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<DeleteDiscountCommand, Result>
{
    public async Task<Result> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        int exhibitionId = await applicationDbContext.PriceAdjustmentWorkflows
            .Where(x => x.Id == request.Id)
            .Select(x => x.ExhibitionId)
            .FirstOrDefaultAsync(cancellationToken);

        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(exhibitionId),
                Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);

        Result result = await workflowService.DeleteDiscountCommand(request.Id, cancellationToken);
        return result;
    }
}
