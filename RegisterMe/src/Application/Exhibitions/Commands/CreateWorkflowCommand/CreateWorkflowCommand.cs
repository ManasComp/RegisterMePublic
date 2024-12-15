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

namespace RegisterMe.Application.Exhibitions.Commands.CreateWorkflowCommand;

// ReSharper disable always UnusedType.Global
public record CreateWorkflowCommandCommand : IRequest<Result<int>>
{
    public required Workflow Workflow { get; init; } = null!;
    public required int ExhibitionId { get; init; }
}

public class CreateWorkflowCommandCommandValidator : AbstractValidator<CreateWorkflowCommandCommand>
{
    public CreateWorkflowCommandCommandValidator()
    {
        RuleFor(x => x.Workflow).NotNull();
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();

        RuleFor(x => x.Workflow.WorkflowName).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Workflow.Rules).NotEmpty();
        RuleForEach(x => x.Workflow.Rules).Must(x => x.RuleName != null).WithMessage("RuleName is required");
    }
}

public class CreateWorkflowCommandCommandHandler(
    WorkflowService workflowService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<CreateWorkflowCommandCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateWorkflowCommandCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.ExhibitionId),
                Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);

        Result<int> data =
            await workflowService.CreateWorkFlow(request.Workflow, request.ExhibitionId, cancellationToken);
        return data;
    }
}
