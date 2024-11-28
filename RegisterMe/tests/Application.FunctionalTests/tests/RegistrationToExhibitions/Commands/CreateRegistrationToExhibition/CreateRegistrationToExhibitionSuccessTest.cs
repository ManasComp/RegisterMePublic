#region

using RegisterMe.Application.Exhibitions.Commands.CancelExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.PublishExhibition;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.RegistrationToExhibition.Commands.CreateRegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;
using static RegisterMe.Application.FunctionalTests.Testing;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.RegistrationToExhibitions.Commands.
    CreateRegistrationToExhibition;

public class CreateRegistrationToExhibitionSuccessTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    public async Task ShouldCreateRegistrationToExhibition()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
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
            GroupsIds = ["1", "2"],
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
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id
        });
        await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id });
        CreateExhibitorCommand commandToCreateExhbiitor = new()
        {
            UserId = user, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
        };
        int createExhibitorCommand =
            (await SendAsync(commandToCreateExhbiitor)).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        // Act
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);

        // Assert
        registrationToExhibitionId.IsSuccess.Should().BeTrue();
        registrationToExhibitionId.Value.Should().BeGreaterThan(0);
        RegistrationToExhibitionDto registrationToExhibition =
            await SendAsync(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId.Value
            });
        CompareUtils.Equals(createRegistrationToExhibitionCommand.RegistrationToExhibition, registrationToExhibition)
            .Should().BeTrue();

        UpsertExhibitorDto exhibitor = commandToCreateExhbiitor.Exhibitor;
        ApplicationUser? getUser = await FindAsync<ApplicationUser>(user);
        getUser.Should().NotBeNull();
        registrationToExhibition.PersonRegistration.HouseNumber.Should().Be(exhibitor.HouseNumber);
        registrationToExhibition.PersonRegistration.City.Should().Be(exhibitor.City);
        registrationToExhibition.PersonRegistration.Street.Should().Be(exhibitor.Street);
        registrationToExhibition.PersonRegistration.Country.Should().Be(exhibitor.Country);
        registrationToExhibition.PersonRegistration.ZipCode.Should().Be(exhibitor.ZipCode);
        registrationToExhibition.PersonRegistration.DateOfBirth.Should().Be(getUser!.DateOfBirth);
        registrationToExhibition.PersonRegistration.Organization.Should().Be(exhibitor.Organization);
        registrationToExhibition.PersonRegistration.MemberNumber.Should().Be(exhibitor.MemberNumber);
        registrationToExhibition.PersonRegistration.FirstName.Should().Be(getUser.FirstName);
        registrationToExhibition.PersonRegistration.Email.Should().Be(getUser.Email);
        registrationToExhibition.PersonRegistration.LastName.Should().Be(getUser.LastName);
        registrationToExhibition.PersonRegistration.Country.Should().Be(exhibitor.Country);
        registrationToExhibition.PersonRegistration.PhoneNumber.Should().Be(getUser.PhoneNumber);
        registrationToExhibition.PersonRegistration.DateOfBirth.Should().Be(getUser.DateOfBirth);
    }

    [Test]
    public async Task ShouldFailCreateRegistrationToExhibitionForUnpublishedExhibition()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        })).Value;
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await RunAsOndrejAsync();
        int exhibition1Id = (await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.Exhibition1(organization1)
        })).Value;
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id
        });

        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = user, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        // Act
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);

        // Assert
        registrationToExhibitionId.IsSuccess.Should().BeFalse();
    }

    [Test]
    public async Task ShouldFailCreateRegistrationToExhibitionForCanceledExhibition()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
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
            GroupsIds = ["1", "2"],
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
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id
        });
        await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id });
        await SendAsync(new CancelExhibitionCommand { ExhibitionId = exhibition1Id });
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = user, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        // Act
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);

        // Assert
        registrationToExhibitionId.IsSuccess.Should().BeFalse();
    }

    [Test]
    [TestCase(RunAsSpecificUser.RunAsOndrej)]
    [TestCase(RunAsSpecificUser.RunAsAdministratorAsync)]
    public async Task ShouldFailCreateRegistrationToExhibitionForExhibitionOutOfRegistrationTime(
        RunAsSpecificUser runAsSpecificUser)
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
        })).Value;
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await RunAsOndrejAsync();
        int exhibition1Id = (await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = ExhibitionDataGenerator.OutdatedRegistration(organization1)
        })).Value;
        List<ExhibitionDayDto> exhibitionDays =
            await SendAsync(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibition1Id });
        await SendAsync(new CreatePriceGroupCommand
        {
            GroupsIds = ["1", "2"],
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
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id
        });
        await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id });
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = user, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        // Act
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);

        // Assert
        registrationToExhibitionId.IsSuccess.Should().BeFalse();
    }


    [Test]
    public async Task ShouldFailCreateMoreRegistrationsTOxhibitionsToOneExhibition()
    {
        // Arrange
        string user = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user)
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
            GroupsIds = ["1", "2"],
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
        await SendAsync(new CreateAdvertisementCommand
        {
            Advertisement = AdvertisementDataGenerator.GetAdvertisementDto1(), ExhibitionId = exhibition1Id
        });
        await SendAsync(new PublishExhibitionCommand { ExhibitionId = exhibition1Id });
        int createExhibitorCommand =
            (await SendAsync(new CreateExhibitorCommand
            {
                UserId = user, Exhibitor = ExhibitorDataGenerator.GetExhibitorDto1()
            })).Value;
        List<AdvertisementDto> advertisements =
            await SendAsync(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibition1Id });

        // Act
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Result<int> registrationToExhibitionId = await SendAsync(createRegistrationToExhibitionCommand);
        CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand2 =
            new()
            {
                RegistrationToExhibition = RegistrationToExhibitionDataGenerator.Normal(exhibition1Id,
                    createExhibitorCommand, advertisements.First().Id)
            };
        Result<int> registrationToExhibitionId2 = await SendAsync(createRegistrationToExhibitionCommand2);

        // Assert
        registrationToExhibitionId.IsSuccess.Should().BeTrue();
        registrationToExhibitionId2.IsSuccess.Should().BeFalse();
    }
}
