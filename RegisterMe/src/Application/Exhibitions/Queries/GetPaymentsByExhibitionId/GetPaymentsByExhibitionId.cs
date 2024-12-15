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

namespace RegisterMe.Application.Exhibitions.Queries.GetPaymentsByExhibitionId;

// ReSharper disable always UnusedType.Global
public record GetPaymentsByExhibitionIdQuery : IRequest<Workflow>
{
    public required int ExhibitionId { get; init; }
}

public class GetPaymentsByExhibitionIdQueryValidator : AbstractValidator<GetPaymentsByExhibitionIdQuery>
{
    public GetPaymentsByExhibitionIdQueryValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class GetPaymentsByExhibitionIdQueryHandler(
    WorkflowService workflowService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetPaymentsByExhibitionIdQuery, Workflow>
{
    public async Task<Workflow> Handle(GetPaymentsByExhibitionIdQuery request, CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.ExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        Workflow workflow =
            await workflowService.GetPaymentTypesByExhibitionId(request.ExhibitionId, cancellationToken);
        return workflow;
    }
}
