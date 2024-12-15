#region

using Microsoft.AspNetCore.Identity;
using RegisterMe.Application.Services.Groups;
using RegisterMe.Domain.Constants;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data;

public static class Helpers
{
    public static async Task<List<Group>> SeedGroups(ApplicationDbContext context, GroupInitializer groupInitializer)
    {
        List<Group> groups;
        if (!context.Groups.Any())
        {
            groups = groupInitializer.GetGroups()
                .Select(x => new Group { GroupId = x.GroupId, Name = x.Name })
                .ToList();
            context.Groups.AddRange(groups);
            await context.SaveChangesAsync();
        }
        else
        {
            groups = context.Groups.ToList();
        }

        return groups;
    }

    public static async Task<(IdentityRole administratorRole, IdentityRole organizatorAdministratorRole )> SeedRoles(
        RoleManager<IdentityRole> roleManager)
    {
        IdentityRole administratorRole = new(Roles.Administrator);
        IdentityRole organiserAdministratorRole = new(Roles.OrganizationAdministrator);
        IdentityRole exhibitorRole = new(Roles.Exhibitor);

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }

        if (roleManager.Roles.All(r => r.Name != exhibitorRole.Name))
        {
            await roleManager.CreateAsync(exhibitorRole);
        }

        if (roleManager.Roles.All(r => r.Name != organiserAdministratorRole.Name))
        {
            await roleManager.CreateAsync(organiserAdministratorRole);
        }

        return (administratorRole, organiserAdministratorRole);
    }
}
