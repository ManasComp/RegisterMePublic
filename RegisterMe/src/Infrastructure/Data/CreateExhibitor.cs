#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data;

public static class CreateExhibitor
{
    public static void CreateExhibitorFromUserId(ApplicationUser user, ApplicationDbContext context)
    {
        Exhibitor exhibitor = new()
        {
            Organization = "My organization",
            MemberNumber = "number",
            AspNetUserId = user.Id,
            City = "My city",
            Street = "My street",
            HouseNumber = "My house number",
            ZipCode = "My zip code",
            Country = "CZ",
            EmailToOrganization = "name@organization.cz",
            IsPartOfCsch = true,
            IsPartOfFife = true
        };

        context.Exhibitors.Add(exhibitor);
        context.SaveChanges();
    }
}
