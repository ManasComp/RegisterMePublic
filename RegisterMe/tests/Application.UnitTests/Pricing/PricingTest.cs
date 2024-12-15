#region

using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RegisterMe.Application.Pricing;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Services.Enums;
using RegisterMe.Application.Services.Workflows;
using RegisterMe.Application.ValueTypes;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.UnitTests.Pricing;

public class PricingTest
{
    [Test]
    [TestCase(1, 600, 20, 1000, 30, 3)]
    [TestCase(2, 900, 40, 1000, 30, 3)]
    [TestCase(1, 600, 20, 1000, 30, 4)]
    [TestCase(2, 900, 40, 1000, 30, 4)]
    [TestCase(1, 600, 20, 1000, 30, 150)]
    [TestCase(2, 900, 40, 1000, 30, 150)]
    [TestCase(1, 600, 20, 1000, 30, 2)]
    [TestCase(2, 900, 40, 1000, 30, 2)]
    public async Task MassDiscountShouldBeApplied(int numberOfVisitedDays, int finalPriceCzk, int finalPriceEur,
        int originalPriceCzk, int originalPriceEur, int sortedAscendingByPriceIndex)
    {
        List<Workflow>? workflow = await ReadWorkflowFromFile();
        RulesEngine.RulesEngine rulesEngine = new(workflow?.ToArray());


        CatRegistrationStructure catRegistration = new()
        {
            CatRegistrationId = 1,
            SortedAscendingByPriceIndex = sortedAscendingByPriceIndex,
            NumberOfVisitedDays = numberOfVisitedDays,
            OriginalPrice = new MultiCurrencyPrice(originalPriceCzk, originalPriceEur),
            CatName = "Cat1",
            RentedCageTypesIds = [],
            IsLitter = false,
            CountOfUsedCagesPerRentedCageType = new Dictionary<CagesAndCatsEnum, int>
            {
                { CagesAndCatsEnum.SingleCageSingleCat, 0 },
                { CagesAndCatsEnum.DoubleCageSingleCat, 0 },
                { CagesAndCatsEnum.DoubleCageMultipleCats, 0 }
            },
            OwnCages = []
        };

        WorkflowService workflowService = new(null!, new Utils());
        ICollection<MultiCurrencyPrice> result = await workflowService.ExecuteDiscountWorkflow(rulesEngine,
            catRegistration, workflow?.Single(x => x.WorkflowName == "MnozstevniSleva") ?? throw new Exception());

        result.Count.Should().Be(1);
        result.First()
            .Equals(new MultiCurrencyPrice(finalPriceCzk - originalPriceCzk, finalPriceEur - originalPriceEur)).Should()
            .BeTrue();
    }

    [Test]
    [TestCase(1, 1000, 30, 0)]
    [TestCase(2, 1000, 30, 0)]
    [TestCase(1, 1000, 30, 1)]
    [TestCase(2, 1000, 30, 1)]
    public async Task MassDiscountShouldNotBeApplied(int numberOfVisitedDays,
        int originalPriceCzk, int originalPriceEur, int sortedAscendingByPriceIndex)
    {
        List<Workflow>? workflow = await ReadWorkflowFromFile();
        RulesEngine.RulesEngine rulesEngine = new(workflow?.ToArray());


        CatRegistrationStructure catRegistration = new()
        {
            CatRegistrationId = 1,
            SortedAscendingByPriceIndex = sortedAscendingByPriceIndex,
            NumberOfVisitedDays = numberOfVisitedDays,
            OriginalPrice = new MultiCurrencyPrice(originalPriceCzk, originalPriceEur),
            CatName = "Cat1",
            RentedCageTypesIds = [],
            IsLitter = false,
            CountOfUsedCagesPerRentedCageType = new Dictionary<CagesAndCatsEnum, int>
            {
                { CagesAndCatsEnum.SingleCageSingleCat, 0 },
                { CagesAndCatsEnum.DoubleCageSingleCat, 0 },
                { CagesAndCatsEnum.DoubleCageMultipleCats, 0 }
            },
            OwnCages = []
        };

        WorkflowService workflowService = new(null!, new Utils());
        ICollection<MultiCurrencyPrice> result = await workflowService.ExecuteDiscountWorkflow(rulesEngine,
            catRegistration, workflow?.Single(x => x.WorkflowName == "MnozstevniSleva") ?? throw new Exception());

        result.Count.Should().Be(0);
    }

    [Test]
    [TestCase(1, 300, 12)]
    [TestCase(2, 600, 24)]
    public async Task RentedCageDiscountShouldBeApplied(int doubleCageSingleCat, int feeCzk, int feeEuro)
    {
        List<Workflow>? workflow = await ReadWorkflowFromFile();
        RulesEngine.RulesEngine rulesEngine = new(workflow?.ToArray());


        CatRegistrationStructure catRegistration = new()
        {
            CatRegistrationId = 1,
            SortedAscendingByPriceIndex = 1,
            NumberOfVisitedDays = 1,
            OriginalPrice = new MultiCurrencyPrice(1000, 30),
            CatName = "Cat1",
            RentedCageTypesIds = [],
            IsLitter = false,
            CountOfUsedCagesPerRentedCageType = new Dictionary<CagesAndCatsEnum, int>
            {
                { CagesAndCatsEnum.SingleCageSingleCat, 0 },
                { CagesAndCatsEnum.DoubleCageSingleCat, doubleCageSingleCat },
                { CagesAndCatsEnum.DoubleCageMultipleCats, 0 }
            },
            OwnCages = []
        };

        WorkflowService workflowService = new(null!, new Utils());
        ICollection<MultiCurrencyPrice> result = await workflowService.ExecuteDiscountWorkflow(rulesEngine,
            catRegistration, workflow?.Single(x => x.WorkflowName == "VelkaPujcenaKlec") ?? throw new Exception());

        result.Count.Should().Be(1);
        result.First().Equals(new MultiCurrencyPrice(feeCzk, feeEuro)).Should().BeTrue();
    }

    [Test]
    [TestCase(3, 0, 0)]
    [TestCase(0, 0, 0)]
    [TestCase(0, 1, 0)]
    [TestCase(0, 1, 1)]
    [TestCase(0, 0, 1)]
    [TestCase(1, 0, 0, true)]
    [TestCase(2, 0, 0, true)]
    public async Task RentedCageDiscountShouldNotBeApplied(int doubleCageSingleCat, int singleCageSingleCat,
        int doubleCageMultipleCats, bool isLitter = false)
    {
        List<Workflow>? workflow = await ReadWorkflowFromFile();
        RulesEngine.RulesEngine rulesEngine = new(workflow?.ToArray());


        CatRegistrationStructure catRegistration = new()
        {
            CatRegistrationId = 1,
            SortedAscendingByPriceIndex = 1,
            NumberOfVisitedDays = 1,
            OriginalPrice = new MultiCurrencyPrice(1000, 30),
            CatName = "Cat1",
            RentedCageTypesIds = [],
            IsLitter = isLitter,
            CountOfUsedCagesPerRentedCageType = new Dictionary<CagesAndCatsEnum, int>
            {
                { CagesAndCatsEnum.SingleCageSingleCat, singleCageSingleCat },
                { CagesAndCatsEnum.DoubleCageSingleCat, doubleCageSingleCat },
                { CagesAndCatsEnum.DoubleCageMultipleCats, doubleCageMultipleCats }
            },
            OwnCages = []
        };

        WorkflowService workflowService = new(null!, new Utils());
        ICollection<MultiCurrencyPrice> result = await workflowService.ExecuteDiscountWorkflow(rulesEngine,
            catRegistration, workflow?.Single(x => x.WorkflowName == "VelkaPujcenaKlec") ?? throw new Exception());

        result.Count.Should().Be(0);
    }

    [Test]
    [TestCase(61, 61, 61, 1, 300, 12)]
    [TestCase(61, 61, 61, 2, 600, 24)]
    public async Task OwnCageShouldBeApplied(int length, int width, int height, int numberOfDays, int feeCzk,
        int feeEuro)
    {
        List<Workflow>? workflow = await ReadWorkflowFromFile();
        RulesEngine.RulesEngine rulesEngine = new(workflow?.ToArray());


        CatRegistrationStructure catRegistration = new()
        {
            CatRegistrationId = 1,
            SortedAscendingByPriceIndex = 1,
            NumberOfVisitedDays = 1,
            OriginalPrice = new MultiCurrencyPrice(1000, 30),
            CatName = "Cat1",
            RentedCageTypesIds = [],
            IsLitter = false,
            CountOfUsedCagesPerRentedCageType = new Dictionary<CagesAndCatsEnum, int>
            {
                { CagesAndCatsEnum.SingleCageSingleCat, 0 },
                { CagesAndCatsEnum.DoubleCageSingleCat, 0 },
                { CagesAndCatsEnum.DoubleCageMultipleCats, 0 }
            },
            OwnCages =
            [
                new PricingCage
                {
                    NumberOfDays = numberOfDays,
                    Length = length,
                    Width = width,
                    Height = height,
                    Type = OwnCageEnum.SingleCat
                }
            ]
        };

        WorkflowService workflowService = new(null!, new Utils());
        ICollection<MultiCurrencyPrice> result = await workflowService.ExecuteDiscountWorkflow(rulesEngine,
            catRegistration, workflow?.Single(x => x.WorkflowName == "VelkaVlastniKlec") ?? throw new Exception());

        result.Count.Should().Be(1);
        result.First().Equals(new MultiCurrencyPrice(feeCzk, feeEuro)).Should().BeTrue();
    }

    [Test]
    [TestCase(61, 61, 61, 0)]
    [TestCase(61, 61, 61, 3)]
    [TestCase(50, 50, 50, 1)]
    [TestCase(50, 50, 50, 2)]
    [TestCase(61, 61, 61, 1, true)]
    [TestCase(61, 61, 61, 2, true)]
    public async Task OwnCageShouldNotBeApplied(int length, int width, int height, int numberOfDays,
        bool isLitter = false)
    {
        List<Workflow>? workflow = await ReadWorkflowFromFile();
        RulesEngine.RulesEngine rulesEngine = new(workflow?.ToArray());


        CatRegistrationStructure catRegistration = new()
        {
            CatRegistrationId = 1,
            SortedAscendingByPriceIndex = 1,
            NumberOfVisitedDays = 1,
            OriginalPrice = new MultiCurrencyPrice(1000, 30),
            CatName = "Cat1",
            RentedCageTypesIds = [],
            IsLitter = isLitter,
            CountOfUsedCagesPerRentedCageType = new Dictionary<CagesAndCatsEnum, int>
            {
                { CagesAndCatsEnum.SingleCageSingleCat, 0 },
                { CagesAndCatsEnum.DoubleCageSingleCat, 0 },
                { CagesAndCatsEnum.DoubleCageMultipleCats, 0 }
            },
            OwnCages =
            [
                new PricingCage
                {
                    NumberOfDays = numberOfDays,
                    Length = length,
                    Width = width,
                    Height = height,
                    Type = OwnCageEnum.SingleCat
                }
            ]
        };

        WorkflowService workflowService = new(null!, new Utils());
        ICollection<MultiCurrencyPrice> result = await workflowService.ExecuteDiscountWorkflow(rulesEngine,
            catRegistration, workflow?.Single(x => x.WorkflowName == "VelkaVlastniKlec") ?? throw new Exception());

        result.Count.Should().Be(0);
    }

    private async Task<List<Workflow>?> ReadWorkflowFromFile(string fileName = "CatRegistrationWorkflow.json")
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
