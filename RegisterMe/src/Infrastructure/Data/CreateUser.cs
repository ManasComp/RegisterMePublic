#region

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data;

public static class CreateUser
{
    public static async Task<ApplicationUser> CreateSuperAdministrator(IConfiguration configuration,
        UserManager<ApplicationUser> userManager, IdentityRole administratorRole)
    {
        string? superAdministratorMail = configuration.GetValue<string>("Email:Mail");
        ApplicationUser superAdministrator = new()
        {
            UserName = superAdministratorMail,
            Email = superAdministratorMail,
            FirstName = "Super",
            LastName = "Administrator",
            PhoneNumber = "+420222222222",
            EmailConfirmed = true,
            OrganizationId = null,
            DateOfBirth = new DateOnly(1990, 1, 1)
        };

        if (userManager.Users.All(u => u.UserName != superAdministrator.UserName))
        {
            await userManager.CreateAsync(superAdministrator, Defaults.Password);
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await userManager.AddToRolesAsync(superAdministrator, [administratorRole.Name]);
            }
        }

        return superAdministrator;
    }

    public static ApplicationUser CreateTestOrganizationAdministrator()
    {
        ApplicationUser organizationAdministrator1 = new()
        {
            UserName = "525025@muni.cz", // todo change this in the production
            Email = "525025@muni.cz", // todo change this in the production
            FirstName = "Organization",
            LastName = "Administrator",
            PhoneNumber = "+420111111111", // todo change this in the production
            EmailConfirmed = true,
            OrganizationId = null,
            DateOfBirth = new DateOnly(1990, 1, 1)
        };
        return organizationAdministrator1;
    }

    public static ApplicationUser CreateOrganizationAdministrator2()
    {
        ApplicationUser organizationAdministrator1 = new()
        {
            UserName = "brnokocky@gmail.com",
            Email = "brnokocky@gmail.com",
            FirstName = "Brno",
            LastName = "Kocky",
            PhoneNumber = "+420333333333",
            EmailConfirmed = true,
            OrganizationId = null,
            DateOfBirth = new DateOnly(1990, 1, 1)
        };
        return organizationAdministrator1;
    }
}
