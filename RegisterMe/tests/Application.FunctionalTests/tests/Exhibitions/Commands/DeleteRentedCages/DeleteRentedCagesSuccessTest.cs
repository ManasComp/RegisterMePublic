#region

using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;
using RegisterMe.Application.Exhibitions.Commands.DeleteRentedCages;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.DeleteRentedCages;

#region

using static Testing;

#endregion

public class DeleteRentedCagesPricesSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldDeleteRentedCages()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        CreateOrganizationCommand createOrganizationCommand = new()
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        };
        int organizationId = (await SendAsync(createOrganizationCommand)).Value;
        Result<int> exhibitionId = await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organizationId)
        });
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId.Value });

        Result<string> ids = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 10,
                Height = 120,
                Length = 120,
                Width = 120,
                RentedCageTypes =
                [
                    RentedType.Single,
                    RentedType.Double
                ],
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList()
            }
        });

        // Act
        Result result = await SendAsync(new DeleteRentedCagesCommand { CagesId = ids.Value });

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
