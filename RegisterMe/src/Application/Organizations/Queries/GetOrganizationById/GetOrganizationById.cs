#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Organizations.Dtos;

#endregion

namespace RegisterMe.Application.Organizations.Queries.GetOrganizationById;

// ReSharper disable always UnusedType.Global
public record GetOrganizationByIdQuery : IRequest<OrganizationDto>
{
    public required int OrganizationId { get; init; }
}

public class GetOrganizationByIdQueryValidator : AbstractValidator<GetOrganizationByIdQuery>
{
    public GetOrganizationByIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId).ForeignKeyValidator();
    }
}

public class GetOrganizationByIdQueryHandler(
    IOrganizationService organizationService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetOrganizationByIdQuery, OrganizationDto>
{
    public async Task<OrganizationDto> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        OrganizationDto organization =
            await organizationService.GetOrganizationByIdAsync(request.OrganizationId, cancellationToken);
        if (user.Id is null)
        {
            if (!organization.IsConfirmed)
            {
                throw new ForbiddenAccessException();
            }
        }
        else
        {
            AuthorizationResult result = await authorizationService.AuthorizeAsync(
                AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeOrganizationId(request.OrganizationId), Operations.Read);
            Guard.Against.UnAuthorized(result);
        }

        return organization;
    }
}
