#region

using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Pricing;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Pricing.Enums;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.Services.Enums;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Entities.RulesEngine;
using RegisterMe.Domain.Enums;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.Services.Workflows;

public class WorkflowService(IApplicationDbContext appContext, Utils utils)
{
    private const string PaymentWorkflowName = "Payment";

    public async Task<Result<int>> CreateWorkFlow(Workflow workflow, int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        Exhibition? exhibition = await appContext.Exhibitions.FindAsync([exhibitionId], cancellationToken);
        Guard.Against.Null(exhibition);

        Result validationResult = await validateWorkflows(workflow);
        if (!validationResult.IsSuccess)
        {
            return Result.Failure<int>(validationResult.Error);
        }

        PriceAdjustmentWorkflow priceAdjustmentWorkflow = new(workflow, exhibitionId);
        appContext.PriceAdjustmentWorkflows.Add(priceAdjustmentWorkflow);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success(priceAdjustmentWorkflow.Id);
    }

    public async Task<List<WorkflowDto>> GetDiscountsByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        List<PriceAdjustmentWorkflow> workflows = await appContext.PriceAdjustmentWorkflows
            .Where(x => x.ExhibitionId == exhibitionId)
            .Include(i => i.Rules)
            .ThenInclude(i => i.Rules)
            .Include(x => x.GlobalParams)
            .ToListAsync(cancellationToken);
        List<WorkflowDto> workflowsDto = workflows.Select(x => x.GetAsWorkflowDto()).ToList();
        return workflowsDto;
    }

    public async Task<Workflow> GetDiscountById(int id, CancellationToken cancellationToken = default)
    {
        PriceAdjustmentWorkflow? workflow = await appContext.PriceAdjustmentWorkflows
            .Include(i => i.Rules)
            .ThenInclude(i => i.Rules)
            .Include(x => x.GlobalParams)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (workflow == null)
        {
            throw new NotFoundException(nameof(PriceAdjustmentWorkflow), id.ToString());
        }

        Workflow data = workflow.GetAsWorkflow();
        return data;
    }

    public async Task<Result> CreatePaymentWorkflow(PriceTypeWorkflow paymentWorkflow,
        CancellationToken cancellationToken = default)
    {
        Exhibition? exhibition =
            await appContext.Exhibitions.FindAsync([paymentWorkflow.ExhibitionId], cancellationToken);
        if (exhibition == null)
        {
            throw new Exception("Exhibition not found");
        }

        exhibition.PaymentTypesWorkflow = paymentWorkflow;
        appContext.Exhibitions.Update(exhibition);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UpdatePaymentWorkflow(PriceTypeWorkflow paymentWorkflow,
        CancellationToken cancellationToken = default)
    {
        Exhibition exhibition = await appContext.Exhibitions.Include(x => x.PaymentTypesWorkflow)
            .SingleAsync(x => x.Id == paymentWorkflow.ExhibitionId, cancellationToken);
        PriceTypeWorkflow workflow = exhibition.PaymentTypesWorkflow;

        Result validationResult = await validateWorkflows(paymentWorkflow.GetAsWorkflow());
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        exhibition.PaymentTypesWorkflow = paymentWorkflow;
        appContext.Exhibitions.Update(exhibition);
        appContext.PriceTypeWorkflows.Remove(workflow);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> validateWorkflows(Workflow workflow)
    {
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
                    NumberOfDays = 1,
                    Length = 1,
                    Width = 1,
                    Height = 1,
                    Type = OwnCageEnum.SingleCat
                }
            ]
        };

        RulesEngine.RulesEngine rulesEngine = new([workflow]);

        List<RuleResultTree>? ruleResultList =
            await rulesEngine.ExecuteAllRulesAsync(workflow.WorkflowName,
                new RuleParameter("registrationToExhibition", registrationToExhibition),
                new RuleParameter("catRegistration", catRegistration),
                new RuleParameter("utils", utils),
                new RuleParameter(nameof(OwnCageEnum.SingleCat), OwnCageEnum.SingleCat),
                new RuleParameter(nameof(OwnCageEnum.MultipleCats), OwnCageEnum.MultipleCats),
                new RuleParameter(nameof(CagesAndCatsEnum.SingleCageSingleCat),
                    CagesAndCatsEnum.SingleCageSingleCat),
                new RuleParameter(nameof(CagesAndCatsEnum.DoubleCageSingleCat),
                    CagesAndCatsEnum.DoubleCageSingleCat),
                new RuleParameter(nameof(CagesAndCatsEnum.DoubleCageMultipleCats),
                    CagesAndCatsEnum.DoubleCageMultipleCats)
            );

        List<string> failures = ruleResultList.Where(x => x.ExceptionMessage != string.Empty)
            .Select(x => x.ExceptionMessage).ToList();

        if (workflow.WorkflowName == PaymentWorkflowName)
        {
            List<string> requiredRulesNames =
            [
                "PayByBankTransfer_CZK", "PayOnlineByCard_CZK", "PayInPlaceByCache_CZK", "PayByBankTransfer_EUR",
                "PayOnlineByCard_EUR", "PayInPlaceByCache_EUR"
            ];
            List<string> ruleNames = ruleResultList.Select(x => x.Rule.RuleName).ToList();
            if (requiredRulesNames.Count != ruleNames.Count)
            {
                failures.Add("Different number of rules");
            }
            else if (!requiredRulesNames.All(x => ruleNames.Any(y => y == x)))
            {
                failures.Add("Some rules are missing" + string.Join(",", requiredRulesNames.Except(ruleNames)));
            }
        }
 
        return failures.Count != 0
            ? failures.Count != 0
                ? Result.Failure(new Error("Failures found", string.Join(",", failures)))
                : Result.Success()
            : Result.Success();
    }

    public async Task<Result<int>> UpdateDiscountWorkflow(Workflow workflow, int discountId,
        CancellationToken cancellationToken = default)
    {
        PriceAdjustmentWorkflow original = await appContext.PriceAdjustmentWorkflows
            .Include(i => i.Rules)
            .ThenInclude(i => i.Rules)
            .Include(x => x.GlobalParams)
            .SingleAsync(x => x.Id == discountId, cancellationToken);

        Result validationResult = await validateWorkflows(workflow);
        if (!validationResult.IsSuccess)
        {
            return Result.Failure<int>(validationResult.Error);
        }

        appContext.PriceAdjustmentWorkflows.Remove(original);
        PriceAdjustmentWorkflow priceAdjustmentWorkflow = new(workflow, original.ExhibitionId);
        await appContext.PriceAdjustmentWorkflows.AddAsync(priceAdjustmentWorkflow, cancellationToken);
        await appContext.SaveChangesAsync(cancellationToken);

        return Result.Success(priceAdjustmentWorkflow.Id);
    }

    public async Task<Result> DeleteDiscountCommand(int discountId, CancellationToken cancellationToken = default)
    {
        PriceAdjustmentWorkflow? discount =
            await appContext.PriceAdjustmentWorkflows.FindAsync([discountId], cancellationToken);
        Guard.Against.Null(discount);
        appContext.PriceAdjustmentWorkflows.Remove(discount);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Workflow> GetPaymentTypesByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        PriceTypeWorkflow workflows = await appContext.PriceTypeWorkflows
            .Where(x => x.ExhibitionId == exhibitionId)
            .Include(i => i.Rules)
            .ThenInclude(i => i.Rules)
            .Include(x => x.GlobalParams)
            .SingleAsync(cancellationToken);
        return workflows.GetAsWorkflow();
    }

    public async Task<List<PaymentTypeWithCurrency>> ExecutePaymentTypesByExhibition(
        RegistrationToExhibitionDto registrationToExhibitionDto,
        Workflow workflow, bool allowPaymentByCard)
    {
        RulesEngine.RulesEngine rulesEngine = new([workflow]);

        List<RuleResultTree>? ruleResultList =
            await rulesEngine.ExecuteAllRulesAsync(PaymentWorkflowName,
                new RuleParameter("registrationToExhibition", registrationToExhibitionDto));
        Guard.Against.Null(ruleResultList);

        Dictionary<PaymentTypeWithCurrency, bool> adminWorkflows = new()
        {
            { new PaymentTypeWithCurrency(PaymentType.PayByBankTransfer, Currency.Czk), true },
            { new PaymentTypeWithCurrency(PaymentType.PayOnlineByCard, Currency.Czk), allowPaymentByCard },
            { new PaymentTypeWithCurrency(PaymentType.PayInPlaceByCache, Currency.Czk), true },
            { new PaymentTypeWithCurrency(PaymentType.PayByBankTransfer, Currency.Eur), true },
            { new PaymentTypeWithCurrency(PaymentType.PayOnlineByCard, Currency.Eur), allowPaymentByCard },
            { new PaymentTypeWithCurrency(PaymentType.PayInPlaceByCache, Currency.Eur), true }
        };

        List<PaymentTypeWithCurrency> availablePaymentTypes = [];
        foreach (RuleResultTree ruleResult in ruleResultList.Where(ruleResult => ruleResult.IsSuccess))
        {
            switch (ruleResult.Rule.RuleName)
            {
                case "PayByBankTransfer_CZK":
                    PaymentTypeWithCurrency payment1 = new(PaymentType.PayByBankTransfer,
                        Currency.Czk);
                    if (adminWorkflows.ContainsKey(payment1))
                    {
                        availablePaymentTypes.Add(payment1);
                    }

                    break;
                case "PayOnlineByCard_CZK":
                    PaymentTypeWithCurrency payment2 = new(PaymentType.PayOnlineByCard, Currency.Czk);
                    if (adminWorkflows.ContainsKey(payment2))
                    {
                        availablePaymentTypes.Add(payment2);
                    }

                    break;
                case "PayInPlaceByCache_CZK":
                    PaymentTypeWithCurrency payment3 = new(PaymentType.PayInPlaceByCache, Currency.Czk);
                    if (adminWorkflows.ContainsKey(payment3))
                    {
                        availablePaymentTypes.Add(payment3);
                    }

                    break;
                case "PayByBankTransfer_EUR":
                    PaymentTypeWithCurrency payment4 = new(PaymentType.PayByBankTransfer, Currency.Eur);
                    if (adminWorkflows.ContainsKey(payment4))
                    {
                        availablePaymentTypes.Add(payment4);
                    }

                    break;
                case "PayOnlineByCard_EUR":
                    PaymentTypeWithCurrency payment5 = new(PaymentType.PayOnlineByCard, Currency.Eur);
                    if (adminWorkflows.ContainsKey(payment5))
                    {
                        availablePaymentTypes.Add(payment5);
                    }

                    break;
                case "PayInPlaceByCache_EUR":
                    PaymentTypeWithCurrency payment6 = new(PaymentType.PayInPlaceByCache, Currency.Eur);
                    if (adminWorkflows.ContainsKey(payment6))
                    {
                        availablePaymentTypes.Add(payment6);
                    }

                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        return availablePaymentTypes;
    }

    public async Task<ICollection<MultiCurrencyPrice>> ExecuteDiscountWorkflow(RulesEngine.RulesEngine rulesEngine,
        CatRegistrationStructure structure, Workflow workflow)
    {
        List<RuleResultTree>? ruleResultList =
                await rulesEngine.ExecuteAllRulesAsync(workflow.WorkflowName,
                    new RuleParameter("catRegistration", structure),
                    new RuleParameter("utils", utils),
                    new RuleParameter(nameof(OwnCageEnum.SingleCat), OwnCageEnum.SingleCat),
                    new RuleParameter(nameof(OwnCageEnum.MultipleCats), OwnCageEnum.MultipleCats),
                    new RuleParameter(nameof(CagesAndCatsEnum.SingleCageSingleCat),
                        CagesAndCatsEnum.SingleCageSingleCat),
                    new RuleParameter(nameof(CagesAndCatsEnum.DoubleCageSingleCat),
                        CagesAndCatsEnum.DoubleCageSingleCat),
                    new RuleParameter(nameof(CagesAndCatsEnum.DoubleCageMultipleCats),
                        CagesAndCatsEnum.DoubleCageMultipleCats)
                )
            ;

        HashSet<MultiCurrencyPrice> prices = [];
        foreach (RuleResultTree? ruleResult in ruleResultList)
        {
            string? price = null;
            if (ruleResult.ActionResult.Output != null)
            {
                price = ruleResult.ActionResult.Output.ToString() ??
                        throw new InvalidOperationException();
            }
            else if (ruleResult.IsSuccess)
            {
                price = ruleResult.Rule.SuccessEvent ??
                        throw ruleResult.ActionResult.Exception;
            }

            if (price != null)
            {
                prices.Add(new MultiCurrencyPrice(price));
            }
        }

        return prices;
    }
}
