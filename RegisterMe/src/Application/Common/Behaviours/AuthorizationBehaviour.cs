#region

using System.Reflection;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Security;

#endregion

namespace RegisterMe.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse>(
    IUser user,
    IIdentityService identityService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        IEnumerable<AuthorizeAttribute> authorizeAttributes =
            request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        IEnumerable<AuthorizeAttribute> attributes = authorizeAttributes.ToList();
        if (!attributes.Any())
        {
            return await next();
        }

        if (user.Id == null)
        {
            throw new UnauthorizedAccessException();
        }

        await RoleBasedAuthorization(attributes);
        await PolicyBasedAuthorization(attributes);

        return await next();
    }

    private async Task PolicyBasedAuthorization(IEnumerable<AuthorizeAttribute> authorizeAttributes)
    {
        IEnumerable<AuthorizeAttribute> authorizeAttributesWithPolicies =
            authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
        IEnumerable<AuthorizeAttribute> attributesWithPolicies = authorizeAttributesWithPolicies.ToList();
        if (attributesWithPolicies.Any())
        {
            foreach (string policy in attributesWithPolicies.Select(a => a.Policy))
            {
                bool authorized = await identityService.AuthorizeAsync(user.Id!, policy);

                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }
        }
    }

    private async Task RoleBasedAuthorization(IEnumerable<AuthorizeAttribute> authorizeAttributes)
    {
        IEnumerable<AuthorizeAttribute> authorizeAttributesWithRoles =
            authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

        IEnumerable<AuthorizeAttribute> attributesWithRoles = authorizeAttributesWithRoles.ToList();
        if (!attributesWithRoles.Any())
        {
            return;
        }

        bool authorized = false;

        foreach (string role in attributesWithRoles.Select(a => a.Roles.Split(',')).SelectMany(x => x))
        {
            bool isInRole = await identityService.IsInRoleAsync(user.Id!, role.Trim());
            if (!isInRole)
            {
                continue;
            }

            authorized = true;
            break;
        }

        if (!authorized)
        {
            throw new ForbiddenAccessException();
        }
    }
}
