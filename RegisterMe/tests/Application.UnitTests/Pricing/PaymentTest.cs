#region

using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RegisterMe.Application.Pricing;
using RegisterMe.Application.Pricing.Enums;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.Services.Workflows;
using RegisterMe.Domain.Enums;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.UnitTests.Pricing;

public class PaymentTest
{
    [Test]
    public async Task PaymentsShouldWorkForSchk()
    {
        Workflow workflow = (await ReadWorkflowFromFile() ?? throw new InvalidOperationException()).First();

        RegistrationToExhibitionDto registrationToExhibition = new()
        {
            CatRegistrationIds = [1, 3],
            Id = 5,
            PersonRegistration = new PersonRegistrationDto
            {
                City = "Brno",
                Street = "Br",
                HouseNumber = "Brno",
                ZipCode = "DF",
                Organization = "ORG",
                MemberNumber = "DF",
                DateOfBirth = DateOnly.MaxValue,
                Country = "Czech Republic",
                Email = "f",
                FirstName = "f",
                LastName = "f",
                IsPartOfCsch = true,
                PhoneNumber = "f",
                EmailToOrganization = "email@organization.com",
                IsPartOfFife = true
            },
            PaymentInfo = null,
            ExhibitionId = 1,
            ExhibitorId = 1,
            AdvertisementId = 1
        };

        WorkflowService workflowService = new(null!, new Utils());
        List<PaymentTypeWithCurrency> result =
            await workflowService.ExecutePaymentTypesByExhibition(registrationToExhibition, workflow, true);

        result.Count.Should().Be(2);
        result.Should().Contain(new PaymentTypeWithCurrency(PaymentType.PayByBankTransfer, Currency.Czk));
        result.Should().Contain(new PaymentTypeWithCurrency(PaymentType.PayInPlaceByCache, Currency.Czk));
    }

    [Test]
    public async Task PaymentsShouldWorkForNonSchk()
    {
        Workflow workflow = (await ReadWorkflowFromFile() ?? throw new InvalidOperationException()).First();

        RegistrationToExhibitionDto registrationToExhibition = new()
        {
            CatRegistrationIds = [1, 3],
            Id = 5,
            PersonRegistration = new PersonRegistrationDto
            {
                City = "Brno",
                Street = "Br",
                HouseNumber = "Brno",
                ZipCode = "DF",
                Organization = "ORG",
                MemberNumber = "DF",
                DateOfBirth = DateOnly.MaxValue,
                Country = "Czech Republic",
                Email = "f",
                FirstName = "f",
                LastName = "f",
                IsPartOfCsch = false,
                PhoneNumber = "f",
                EmailToOrganization = "email@organization.com",
                IsPartOfFife = true
            },
            PaymentInfo = null,
            ExhibitionId = 1,
            ExhibitorId = 1,
            AdvertisementId = 1
        };

        WorkflowService workflowService = new(null!, new Utils());
        List<PaymentTypeWithCurrency> result =
            await workflowService.ExecutePaymentTypesByExhibition(registrationToExhibition, workflow, true);

        result.Count.Should().Be(2);
        result.Should().Contain(new PaymentTypeWithCurrency(PaymentType.PayByBankTransfer, Currency.Eur));
        result.Should().Contain(new PaymentTypeWithCurrency(PaymentType.PayInPlaceByCache, Currency.Eur));
    }

    private async Task<List<Workflow>?> ReadWorkflowFromFile(string fileName = "Payments.json")
    {
        string currentDir = Directory.GetCurrentDirectory();
        string desiredDir =
            Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDir)!.FullName)!.FullName)!.FullName;
        string[] files = Directory.GetFiles(desiredDir, fileName, SearchOption.AllDirectories);
        files.Length.Should().BeGreaterThan(0);

        string fileData = await File.ReadAllTextAsync(files[0]);
        List<Workflow>? workflow = JsonConvert.DeserializeObject<List<Workflow>>(fileData);
        workflow.Should().NotBeNull();

        return workflow;
    }
}
