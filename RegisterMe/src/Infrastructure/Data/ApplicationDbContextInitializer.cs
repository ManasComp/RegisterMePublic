#region

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RegisterMe.Application.Services.Groups;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data;

public static class InitializerExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        ApplicationDbContextInitializer initializer =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initializer.InitialiseAsync();
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        try
        {
            await initializer.SeedAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
        }
    }
}

public class ApplicationDbContextInitializer(
    ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    TimeProvider dateTime,
    GroupInitializer groupInitializer,
    IConfiguration configuration)
{
    public async Task InitialiseAsync()
    {
        try
        {
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database");
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Default roles
        (IdentityRole administratorRole, IdentityRole organiserAdministratorRole) =
            await Helpers.SeedRoles(roleManager);

        // Default users
        await CreateUser.CreateSuperAdministrator(configuration, userManager, administratorRole);

        ApplicationUser organizationAdministrator1 = CreateUser.CreateTestOrganizationAdministrator();
        ApplicationUser organizationAdministrator2 = CreateUser.CreateOrganizationAdministrator2();

        await CreateLastOrganizationWithExhibition(organizationAdministrator1, organiserAdministratorRole, false);
        await CreateCurrentOrganizationWithExhibition(organizationAdministrator2, organiserAdministratorRole, true);
    }

    private async Task CreateLastOrganizationWithExhibition(ApplicationUser organizationAdministrator,
        IdentityRole organiserAdministratorRole, bool published)
    {
        List<Group> groups = await Helpers.SeedGroups(context, groupInitializer);
        if (userManager.Users.All(u => u.UserName != organizationAdministrator.UserName))
        {
            await userManager.CreateAsync(organizationAdministrator, Defaults.Password);
            if (!string.IsNullOrWhiteSpace(organiserAdministratorRole.Name))
            {
                await userManager.AddToRolesAsync(organizationAdministrator,
                    [organiserAdministratorRole.Name]);
            }

            int organizationId = CreateOrganization.CreateTestOrganization(organizationAdministrator, context);
            await CreateExhibition.CreateXvExhibition(context, dateTime, groups, published, organizationId);
        }
    }

    private async Task CreateCurrentOrganizationWithExhibition(ApplicationUser organizationAdministrator,
        IdentityRole organiserAdministratorRole, bool published)
    {
        List<Group> groups = await Helpers.SeedGroups(context, groupInitializer);
        if (userManager.Users.All(u => u.UserName != organizationAdministrator.UserName))
        {
            await userManager.CreateAsync(organizationAdministrator, Defaults.Password);
            if (!string.IsNullOrWhiteSpace(organiserAdministratorRole.Name))
            {
                await userManager.AddToRolesAsync(organizationAdministrator,
                    [organiserAdministratorRole.Name]);
            }

            CreateExhibitor.CreateExhibitorFromUserId(organizationAdministrator, context);
            int organizationId = CreateOrganization.CreateKockyBrnoOrganization(organizationAdministrator, context);
            await CreateExhibition.CreateXviiExhibition(context, groups, published, organizationId);
        }
    }

    public async Task SeedTestExhibition(ApplicationUser organizationAdmin, ApplicationDbContext dbContext,
        TimeProvider dateTimeProvider,
        bool published = false)
    {
        await Helpers.SeedRoles(roleManager);
        List<Group> groups = await Helpers.SeedGroups(dbContext, groupInitializer);

        CreateExhibitor.CreateExhibitorFromUserId(organizationAdmin, dbContext);
        await dbContext.SaveChangesAsync();
        int organizationId = CreateOrganization.CreateKockyBrnoOrganization(organizationAdmin, dbContext);
        await CreateExhibition.CreateXvExhibition(dbContext, dateTime, groups, published, organizationId);
    }
}
