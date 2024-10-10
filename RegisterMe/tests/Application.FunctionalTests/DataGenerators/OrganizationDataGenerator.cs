#region

using RegisterMe.Application.Organizations.Dtos;

#endregion

namespace RegisterMe.Application.FunctionalTests.DataGenerators;

public static class OrganizationDataGenerator
{
    public static CreateOrganizationDto GetOrganizationDto1(string organizationAdminId)
    {
        CreateOrganizationDto organization =
            new()
            {
                Address = "Botanická 68A, 602 00 Brno-Královo Pole-Ponava",
                Name = "Big Organization",
                Email = "info@testorg.com",
                Ico = "12345678",
                TelNumber = "+1234567890",
                Website = "www.google.com",
                AdminId = organizationAdminId
            };
        return organization;
    }

    public static CreateOrganizationDto GetOrganizationDto2(string organizationAdminId)
    {
        CreateOrganizationDto organization =
            new()
            {
                Address = "1Botanická 68A, 602 00 Brno-Královo Pole-Ponava",
                Name = "1test Organization",
                Email = "1info@testorg.com",
                Ico = "112345678",
                TelNumber = "1+1234567890",
                Website = "www.testorg1.com",
                AdminId = organizationAdminId
            };
        return organization;
    }
}
