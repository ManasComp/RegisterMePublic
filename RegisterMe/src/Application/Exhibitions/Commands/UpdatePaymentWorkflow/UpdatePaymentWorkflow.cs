#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Workflows;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities.RulesEngine;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.UpdatePaymentWorkflow;

// ReSharper disable always UnusedType.Global
public record UpdatePaymentWorkflowCommand : IRequest<Result>
{
    public required Workflow PaymentWorkflow { get; init; } = null!;
    public required int ExhibitionId { get; init; }
}

public class UpdatePaymentWorkflowCommandValidator : AbstractValidator<UpdatePaymentWorkflowCommand>
{
    public UpdatePaymentWorkflowCommandValidator()
    {
        RuleFor(x => x.PaymentWorkflow).NotNull();
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class UpdatePaymentWorkflowCommandHandler(
    WorkflowService workflowService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<UpdatePaymentWorkflowCommand, Result>
{
    public async Task<Result> Handle(UpdatePaymentWorkflowCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.ExhibitionId),
                Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);

        Result result = await workflowService.UpdatePaymentWorkflow(
            new PriceTypeWorkflow(request.PaymentWorkflow, request.ExhibitionId), cancellationToken);
        return result;
    }
}
