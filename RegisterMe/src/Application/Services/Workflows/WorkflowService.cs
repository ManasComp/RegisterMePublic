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

        MyWorkflow myWorkflow = new(workflow, exhibitionId);
        appContext.MyWorkflows.Add(myWorkflow);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success(myWorkflow.Id);
    }

    public async Task<List<WorkflowDto>> GetDiscountsByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        List<MyWorkflow> workflows = await appContext.MyWorkflows
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
        MyWorkflow? workflow = await appContext.MyWorkflows
            .Include(i => i.Rules)
            .ThenInclude(i => i.Rules)
            .Include(x => x.GlobalParams)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (workflow == null)
        {
            throw new NotFoundException(nameof(MyWorkflow), id.ToString());
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
        exhibition.PaymentTypesWorkflow = paymentWorkflow;
        appContext.Exhibitions.Update(exhibition);
        appContext.PriceTypeWorkflows.Remove(workflow);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<int>> UpdateDiscountWorkflow(Workflow workflow, int discountId,
        CancellationToken cancellationToken = default)
    {
        MyWorkflow original = await appContext.MyWorkflows
            .Include(i => i.Rules)
            .ThenInclude(i => i.Rules)
            .Include(x => x.GlobalParams)
            .SingleAsync(x => x.Id == discountId, cancellationToken);
        appContext.MyWorkflows.Remove(original);
        MyWorkflow myWorkflow = new(workflow, original.ExhibitionId);
        await appContext.MyWorkflows.AddAsync(myWorkflow, cancellationToken);
        await appContext.SaveChangesAsync(cancellationToken);

        return Result.Success(myWorkflow.Id);
    }

    public async Task<Result> DeleteDiscountCommand(int discountId, CancellationToken cancellationToken = default)
    {
        MyWorkflow? discount = await appContext.MyWorkflows.FindAsync([discountId], cancellationToken);
        Guard.Against.Null(discount);
        appContext.MyWorkflows.Remove(discount);
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
