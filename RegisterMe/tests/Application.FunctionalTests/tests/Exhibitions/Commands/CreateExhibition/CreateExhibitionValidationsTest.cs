#region

using System.Diagnostics.CodeAnalysis;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Services.Workflows;

#endregion

namespace RegisterMe.Application.FunctionalTests.tests.Exhibitions.Commands.CreateExhibition;

#region

using static Testing;

#endregion

public class CreateExhibitionValidationsTest(DatabaseTypes databaseType) : BaseTestFixture(databaseType)
{
    [Test]
    [TestCase(null, null, null, null, "updatedexhibition", null, null, null, null, null, null, null, null, null, null)]
    [TestCase(null, null, null, null, null, null, null, null, null, "Updated Email", null, null, null, null, null)]
    [TestCase(null, null, null, null, null, null, null, null, null, null, "2022-01-11", "2022-01-01", null, null, null)]
    [TestCase("", null, null, null, null, null, null, null, null, null, "2022-01-11", "2022-01-01", null, null, null)]
    public async Task ShouldFailCreateExhibition(string? north, string? south, string? streetAddress, string? name,
        string? url,
        string? description, string? bankAccount, string? iban, string? phone, string? email,
        DateTime? registrationStart,
        DateTime? registrationEnd, DateTime? exhibitionStart, DateTime? exhibitionEnd, int? hours)
    {
        // Arrange
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await RunAsOndrejAsync();

        DefaultValues(ref north, ref south, ref streetAddress, ref name, ref url, ref description, ref bankAccount,
            ref iban, ref phone, ref email, ref registrationStart, ref registrationEnd, ref exhibitionStart,
            ref exhibitionEnd, ref hours, organization1);

        // Act
        Func<Task> act = async () => await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = new CreateExhibitionDto
            {
                Address = new AddressDto { Latitude = north, Longitude = south, StreetAddress = streetAddress },
                Name = name,
                Url = url,
                Description = description,
                BankAccount = bankAccount,
                Iban = iban,
                Phone = phone,
                Email = email,
                RegistrationStart = DateOnly.FromDateTime(registrationStart.Value),
                RegistrationEnd = DateOnly.FromDateTime(registrationEnd.Value),
                ExhibitionStart = DateOnly.FromDateTime(exhibitionStart.Value),
                ExhibitionEnd = DateOnly.FromDateTime(exhibitionEnd.Value),
                OrganizationId = organization1,
                DeleteNotFinishedRegistrationsAfterHours = hours!.Value
            }
        });

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    private static void DefaultValues([AllowNull] ref string north, [AllowNull] ref string south,
        [AllowNull] ref string streetAddress,
        [AllowNull] ref string name, [AllowNull] ref string url, [AllowNull] ref string description,
        [AllowNull] ref string bankAccount, [AllowNull] ref string iban, [AllowNull] ref string phone,
        [AllowNull] ref string email, [NotNull] ref DateTime? registrationStart,
        [NotNull] ref DateTime? registrationEnd,
        [NotNull] ref DateTime? exhibitionStart, [NotNull] ref DateTime? exhibitionEnd, ref int? hours,
        int organization1)
    {
        CreateExhibitionDto exhibition = ExhibitionDataGenerator.Exhibition1(organization1);

        north ??= exhibition.Address.Latitude;
        south ??= exhibition.Address.Longitude;
        streetAddress ??= exhibition.Address.StreetAddress;
        name ??= exhibition.Name;
        url ??= exhibition.Url;
        description ??= exhibition.Description;
        bankAccount ??= exhibition.BankAccount;
        iban ??= exhibition.Iban;
        phone ??= exhibition.Phone;
        email ??= exhibition.Email;
        registrationStart ??= exhibition.RegistrationStart.ToDateTime(TimeOnly.MaxValue);
        registrationEnd ??= exhibition.RegistrationEnd.ToDateTime(TimeOnly.MaxValue);
        exhibitionStart ??= exhibition.ExhibitionStart.ToDateTime(TimeOnly.MaxValue);
        exhibitionEnd ??= exhibition.ExhibitionEnd.ToDateTime(TimeOnly.MaxValue);
        hours ??= exhibition.DeleteNotFinishedRegistrationsAfterHours;
    }

    [Test]
    [TestCase(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)]
    public async Task ShouldCreateExhibition(string? north, string? south, string? streetAddress, string? name,
        string? url,
        string? description, string? bankAccount, string? iban, string? phone, string? email,
        DateTime? registrationStart,
        DateTime? registrationEnd, DateTime? exhibitionStart, DateTime? exhibitionEnd, int? hours)
    {
        // Arrange
        string user1 = await RunAsOndrejAsync();
        int organization1 = (await SendAsync(new CreateOrganizationCommand
        {
            CreateOrganizationDto = OrganizationDataGenerator.GetOrganizationDto1(user1)
        })).Value;
        await RunAsAdministratorAsync();
        await SendAsync(new ConfirmOrganizationCommand { OrganizationId = organization1 });
        await RunAsOndrejAsync();

        DefaultValues(ref north, ref south, ref streetAddress, ref name, ref url, ref description, ref bankAccount,
            ref iban, ref phone, ref email, ref registrationStart, ref registrationEnd, ref exhibitionStart,
            ref exhibitionEnd, ref hours, organization1);

        // Act
        Func<Task> act = async () => await SendAsync(new CreateExhibitionCommand
        {
            CreateExhibitionDto = new CreateExhibitionDto
            {
                Address = new AddressDto { Latitude = north, Longitude = south, StreetAddress = streetAddress },
                Name = name,
                Url = url,
                Description = description,
                BankAccount = bankAccount,
                Iban = iban,
                Phone = phone,
                Email = email,
                RegistrationStart = DateOnly.FromDateTime(registrationStart.Value),
                RegistrationEnd = DateOnly.FromDateTime(registrationEnd.Value),
                ExhibitionStart = DateOnly.FromDateTime(exhibitionStart.Value),
                ExhibitionEnd = DateOnly.FromDateTime(exhibitionEnd.Value),
                OrganizationId = organization1,
                DeleteNotFinishedRegistrationsAfterHours = hours!.Value
            }
        });

        // Assert
        await act.Should().NotThrowAsync();
    }
}
