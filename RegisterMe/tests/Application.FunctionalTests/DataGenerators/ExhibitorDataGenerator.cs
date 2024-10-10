#region

using RegisterMe.Application.Exhibitors.Dtos;

#endregion

namespace RegisterMe.Application.FunctionalTests.DataGenerators;

public static class ExhibitorDataGenerator
{
    public static UpsertExhibitorDto GetExhibitorDto1()
    {
        UpsertExhibitorDto exhibitor = new()
        {
            Organization = "Testing Organization",
            MemberNumber = "123456",
            Country = "CZ",
            City = "New York",
            Street = "Wall Street",
            HouseNumber = "1",
            ZipCode = "10005",
            IsPartOfCsch = true,
            EmailToOrganization = "email@example.com",
            IsPartOfFife = true
        };

        return exhibitor;
    }

    public static UpsertExhibitorDto GetExhibitorDto2()
    {
        UpsertExhibitorDto exhibitor = new()
        {
            Organization = "1Testing Organization",
            MemberNumber = "1123456",
            Country = "SK",
            City = "1New York",
            Street = "1Wall Street",
            HouseNumber = "111",
            ZipCode = "110005",
            IsPartOfCsch = false,
            EmailToOrganization = "1email@example.com",
            IsPartOfFife = false
        };

        return exhibitor;
    }
}
