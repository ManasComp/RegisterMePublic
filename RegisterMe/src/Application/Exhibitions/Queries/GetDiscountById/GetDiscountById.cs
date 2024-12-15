#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Workflows;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetDiscountById;

// ReSharper disable always UnusedType.Global
public record GetDiscountByIdQuery : IRequest<Workflow>
{
    public required int WorkflowId { get; init; }
}

public class GetDiscountByIdQueryValidator : AbstractValidator<GetDiscountByIdQuery>
{
    public GetDiscountByIdQueryValidator()
    {
        RuleFor(x => x.WorkflowId).ForeignKeyValidator();
    }
}

public class GetDiscountByIdQueryHandler(
    WorkflowService workflowService,
    IAuthorizationService authorizationService,
    IUser user,
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetDiscountByIdQuery, Workflow>
{
    public async Task<Workflow> Handle(GetDiscountByIdQuery request, CancellationToken cancellationToken)
    {
        int exhibitionId = await applicationDbContext.PriceAdjustmentWorkflows.Where(x => x.Id == request.WorkflowId)
            .Select(x => x.ExhibitionId).FirstOrDefaultAsync(cancellationToken);
        if (exhibitionId == 0)
        {
            throw new NotFoundException("Discount not found", request.WorkflowId.ToString());
        }

        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(exhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        Workflow workflow = await workflowService.GetDiscountById(request.WorkflowId, cancellationToken);
        return workflow;
    }
}
