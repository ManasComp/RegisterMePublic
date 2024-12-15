#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Workflows;
using RegisterMe.Domain.Entities.RulesEngine;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetDiscountsByExhibitionId;

// ReSharper disable always UnusedType.Global
public record GetDiscountsByExhibitionIdQuery : IRequest<List<WorkflowDto>>
{
    public required int ExhibitionId { get; init; }
}

public class GetDiscountsByExhibitionIdQueryValidator : AbstractValidator<GetDiscountsByExhibitionIdQuery>
{
    public GetDiscountsByExhibitionIdQueryValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class GetDiscountsByExhibitionIdQueryHandler(
    WorkflowService workflowService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetDiscountsByExhibitionIdQuery, List<WorkflowDto>>
{
    public async Task<List<WorkflowDto>> Handle(GetDiscountsByExhibitionIdQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.ExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        List<WorkflowDto> workflows =
            await workflowService.GetDiscountsByExhibitionId(request.ExhibitionId, cancellationToken);
        return workflows;
    }
}
