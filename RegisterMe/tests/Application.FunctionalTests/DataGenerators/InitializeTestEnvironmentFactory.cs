#region

using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;
using RegisterMe.Application.Exhibitions.Commands.PublishExhibition;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetPrices;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.RegistrationToExhibition.Commands.CreateRegistrationToExhibition;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.FunctionalTests.DataGenerators;

#region

using static Testing;

#endregion

public static class InitializeTestEnvironmentFactory
{
    public static async Task<(List<ExhibitionDayDto> exhibitionDays, Result<int> registrationToExhibitionId)>
        InitializeTestEnvironment()
    {
        string ondrejId = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(ondrejId)
        })).Value;

        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await RunAsOndrejAsync();

        int exhibition1Id = (await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        })).Value;
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition1Id });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "16"],
            ExhibitionId = exhibition1Id,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = exhibitionDays.Select(x => x.Id).ToList(),
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "16"],
            ExhibitionId = exhibition1Id,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = [exhibitionDays.Select(x => x.Id).ToList()[0]],
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        Result<string> result12 = await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "16"],
            ExhibitionId = exhibition1Id,
            PriceDays =
            [
                new PriceDays
                {
                    ExhibitionDayIds = [exhibitionDays.Select(x => x.Id).ToList()[1]],
                    Price = new MultiCurrencyPrice(100, 3)
                }
            ]
        });
        result12.IsSuccess.Should().BeTrue();
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id
        });
        Result<string> result = await SendAsync(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 5,
                ExhibitionDaysId = exhibitionDays.Select(x => x.Id).ToList(),
                Height = 120,
                Width = 120,
                Length = 120,
                RentedCageTypes = [RentedType.Double, RentedType.Single]
            }
        });

        result.IsSuccess.Should().BeTrue();

        await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id });


        string vojtaId = await RunAsVojtaAsync();
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = vojtaId, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;

        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);


        await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition1Id });
        await SendAsync(new GetPricesQuery { ExhibitionId = exhibition1Id });
        return (exhibitionDays, registrationToExhibitionId);
    }
}
