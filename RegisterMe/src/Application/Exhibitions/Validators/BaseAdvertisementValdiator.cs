#region

using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Validators;

public class BaseAdvertisement
{
    public abstract class BaseAdvertisementCommandValidator<T> : AbstractValidator<T>
    {
        protected void AddCommonRules(Func<T, UpsertAdvertisementDto> dtoSelector)
        {
            RuleFor(x => dtoSelector(x).Description).NotEmpty().MaximumLength(75);
            RuleFor(x => dtoSelector(x).Price.PriceCzk).GreaterThanOrEqualTo(0);
            RuleFor(x => dtoSelector(x).IsDefault).NotNull();
            RuleFor(x => dtoSelector(x).Price.PriceEur).GreaterThanOrEqualTo(0);
        }
    }
}
