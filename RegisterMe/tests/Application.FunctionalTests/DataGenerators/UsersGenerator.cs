#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.FunctionalTests.DataGenerators;

public class UsersGenerator
{
    public static ApplicationUser Ondrej()
    {
        ApplicationUser user = new()
        {
            UserName = "ondrej@mofies.com",
            Email = "ondrej@mofies.com",
            FirstName = "ondrej",
            LastName = "ondrej",
            PhoneNumber = "333333333",
            EmailConfirmed = true,
            OrganizationId = null,
            DateOfBirth = new DateOnly(DateTime.UtcNow.AddYears(-18).Year, 1, 1)
        };
        return user;
    }

    public static ApplicationUser Vojta()
    {
        ApplicationUser user = new()
        {
            UserName = "vojta@mofies.com",
            Email = "vojta@mofies.com",
            FirstName = "vojta",
            LastName = "vojta",
            PhoneNumber = "987654321",
            EmailConfirmed = true,
            OrganizationId = null,
            DateOfBirth = new DateOnly(DateTime.UtcNow.AddYears(-18).Year, 1, 1)
        };
        return user;
    }

    public static ApplicationUser Admin()
    {
        ApplicationUser user = new()
        {
            UserName = "adminr@admin.com",
            Email = "admin@admin.com",
            FirstName = "Administrator",
            LastName = "Administrator",
            PhoneNumber = "1111111111",
            EmailConfirmed = true,
            OrganizationId = null,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-18))
        };
        return user;
    }

    public static ApplicationUser Sabrina()
    {
        ApplicationUser user = new()
        {
            UserName = "sabrina@mofies.com",
            Email = "sabrina@mofies.com",
            FirstName = "sabrina",
            LastName = "sabrina",
            PhoneNumber = "9999999999",
            EmailConfirmed = true,
            OrganizationId = null,
            DateOfBirth = new DateOnly(DateTime.UtcNow.AddYears(-18).Year, 1, 1)
        };
        return user;
    }

    public static ApplicationUser Admin1()
    {
        ApplicationUser user = new()
        {
            UserName = "admin1@admin.com",
            Email = "admin1@admin.com",
            FirstName = "Administrator",
            LastName = "Administrator",
            PhoneNumber = "123456789",
            EmailConfirmed = true,
            OrganizationId = null,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-18))
        };
        return user;
    }
}
