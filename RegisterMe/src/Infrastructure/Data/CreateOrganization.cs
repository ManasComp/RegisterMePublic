#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data;

public static class CreateOrganization
{
    public static int CreateKockyBrnoOrganization(ApplicationUser administrator, ApplicationDbContext context)
    {
        Organization organizaton = new()
        {
            Name = "SO Kočky Brno",
            Email = "kocky.brno@seznam.cz",
            Ico = "03841979",
            TelNumber = "+420 604 954 118",
            Website = "https://www.kockybrno.cz",
            Administrator = [administrator],
            Address = "Kabátníkova 575/9, 602 00 Brno-Královo Pole",
            IsConfirmed = true
        };
        context.Organizations.Add(organizaton);
        context.SaveChanges();

        return organizaton.Id;
    }

    public static int CreateTestOrganization(ApplicationUser administrator, ApplicationDbContext context)
    {
        Organization organizaton = new()
        {
            Name = "SO Test Test",
            Email = "test.test@test.test",
            Ico = "test",
            TelNumber = "+999 999 999 999",
            Website = "https://www.google.cz",
            Administrator = [administrator],
            Address = "Botanická 68A, 602 00 Brno-Královo Pole-Ponava",
            IsConfirmed = true
        };
        context.Organizations.Add(organizaton);
        context.SaveChanges();

        return organizaton.Id;
    }
}
