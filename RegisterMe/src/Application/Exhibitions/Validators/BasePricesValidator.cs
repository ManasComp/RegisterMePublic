#region

using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Validators;

public class BasePricesValidator
{
    public abstract class BasePriceCommandValidator<T> : AbstractValidator<T>
    {
        protected void AddCommonRules(Func<T, BasePriceValidatedDto> dtoSelector)
        {
            RuleFor(x => dtoSelector(x).GroupsIds.Count).GreaterThan(0);
            RuleFor(x => dtoSelector(x).PriceDays.Count).GreaterThan(0);
            RuleForEach(x => dtoSelector(x).PriceDays).SetValidator(new PriceDaysValidator());
        }
    }
}
