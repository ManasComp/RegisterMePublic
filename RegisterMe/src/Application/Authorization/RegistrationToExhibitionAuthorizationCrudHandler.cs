#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.RegistrationToExhibition;

#endregion

namespace RegisterMe.Application.Authorization;

/// <summary>
///     Registration to exhibition authorization handler.
/// </summary>
/// <param name="serviceScopeFactory"></param>
/// <param name="methods"></param>
public class RegistrationToExhibitionAuthorizationCrudHandler(
    IServiceScopeFactory serviceScopeFactory,
    AuthorizationHelperMethods methods) :
    Authorization<AuthorizeRegistrationToExhibitionId>(serviceScopeFactory, methods)
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        AuthorizeRegistrationToExhibitionId resource)
    {
        if (resource.RegistrationToExhibitionId == null)
        {
            context.Succeed(requirement);
            return;
        }

        if (requirement == Operations.DoOrganizationAdminStuff)
        {
            await HandleOrganizationAdminStuff(context, requirement, resource);
        }

        if (requirement == Operations.OnlyOwnerCanDo)
        {
            await HandleOnlyOwnerCanDo(context, requirement, resource);
        }

        await base.HandleRequirementAsync(context, requirement, resource);
    }


    protected override async Task HandleDeleteRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeRegistrationToExhibitionId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    private Task HandleOrganizationAdminStuff(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeRegistrationToExhibitionId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        string userId = AuthorizationHelperMethods.GetUserIdPub(context);
        bool isOrganizationAdmin = applicationDbContext.RegistrationsToExhibition
            .Any(x => x.Exhibition.Organization.Administrator
                .Any(admin => admin.Id == userId) && x.Id == resource.RegistrationToExhibitionId);
        if (isOrganizationAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private Task HandleOnlyOwnerCanDo(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeRegistrationToExhibitionId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        string userId = AuthorizationHelperMethods.GetUserIdPub(context);
        bool isUser = applicationDbContext.RegistrationsToExhibition
            .Any(x => x.Exhibitor.AspNetUserId == userId && x.Id == resource.RegistrationToExhibitionId);
        if (isUser)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    protected override async Task HandleUpdateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeRegistrationToExhibitionId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleCreateRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeRegistrationToExhibitionId resource)
    {
        await DefaultRequirement(context, requirement, resource);
    }

    protected override async Task HandleReadRequirementAsync(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement, AuthorizeRegistrationToExhibitionId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        int registrationToExhibitionId = resource.RegistrationToExhibitionId!.Value;
        bool registration =
            await applicationDbContext.RegistrationsToExhibition.AnyAsync(x => x.Id == registrationToExhibitionId);

        if (!registration)
        {
            throw new NotFoundException(nameof(RegistrationToExhibition), registrationToExhibitionId.ToString());
        }

        string userId = AuthorizationHelperMethods.GetUserIdPub(context);

        bool isUser = applicationDbContext.RegistrationsToExhibition
            .Any(x => x.Exhibitor.AspNetUserId == userId && x.Id == registrationToExhibitionId);

        bool isOrganizationAdmin = applicationDbContext.RegistrationsToExhibition
            .Any(x => x.Exhibition.Organization.Administrator
                .Any(admin => admin.Id == userId) && x.Id == registrationToExhibitionId);

        bool paymentRequested = await applicationDbContext.RegistrationsToExhibition
            .Where(x => x.Id == registrationToExhibitionId)
            .Include(x => x.PaymentInfo)
            .Select(RegistrationToExhibitionService.WasPaid)
            .FirstOrDefaultAsync();

        if ((paymentRequested && isOrganizationAdmin) || isUser)
        {
            context.Succeed(requirement);
        }
    }

    private async Task DefaultRequirement(AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        AuthorizeRegistrationToExhibitionId resource)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        int registrationToExhibitionId = resource.RegistrationToExhibitionId!.Value;
        Domain.Entities.RegistrationToExhibition? registration =
            await applicationDbContext.RegistrationsToExhibition.Where(x => x.Id == registrationToExhibitionId)
                .Include(x => x.PaymentInfo).FirstOrDefaultAsync();

        if (registration == null)
        {
            throw new NotFoundException(nameof(RegistrationToExhibition), registrationToExhibitionId.ToString());
        }

        string userId = AuthorizationHelperMethods.GetUserIdPub(context);
        if (RegistrationToExhibitionService.IsNotPaid(registration.PaymentInfo))
        {
            bool isUser = applicationDbContext.RegistrationsToExhibition
                .Any(x => x.Exhibitor.AspNetUserId == userId && x.Id == registrationToExhibitionId);

            if (isUser)
            {
                context.Succeed(requirement);
            }
        }
        else
        {
            bool isOrganizationAdmin = applicationDbContext.RegistrationsToExhibition
                .Any(x => x.Exhibition.Organization.Administrator
                    .Any(admin => admin.Id == userId) && x.Id == registrationToExhibitionId);

            if (isOrganizationAdmin)
            {
                context.Succeed(requirement);
            }
        }
    }
}

public record AuthorizeRegistrationToExhibitionId(int? RegistrationToExhibitionId);
