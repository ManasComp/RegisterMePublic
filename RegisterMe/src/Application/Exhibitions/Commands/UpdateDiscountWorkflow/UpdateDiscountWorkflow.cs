#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Workflows;
using RegisterMe.Domain.Common;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.UpdateDiscountWorkflow;

// ReSharper disable always UnusedType.Global
public record UpdateDiscountWorkflowCommand : IRequest<Result<int>>
{
    public required Workflow Workflow { get; init; } = null!;
    public required int Id { get; init; }
}

public class UpdateDiscountWorkflowCommandValidator : AbstractValidator<UpdateDiscountWorkflowCommand>
{
    public UpdateDiscountWorkflowCommandValidator()
    {
        RuleFor(x => x.Workflow).NotNull();
        RuleFor(x => x.Id).ForeignKeyValidator();
    }
}

public class UpdateDiscountWorkflowCommandHandler(
    WorkflowService service,
    IAuthorizationService authorizationService,
    IUser user,
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<UpdateDiscountWorkflowCommand, Result<int>>
{
    public async Task<Result<int>> Handle(UpdateDiscountWorkflowCommand request, CancellationToken cancellationToken)
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

        Result<int> id = await service.UpdateDiscountWorkflow(request.Workflow, request.Id, cancellationToken);
        return id;
    }
}
