#region

using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Validators;

public class PriceDaysValidator : AbstractValidator<PriceDays>
{
    public PriceDaysValidator()
    {
        RuleFor(x => x.ExhibitionDayIds.Count).GreaterThan(0);
        RuleFor(x => x.Price.PriceCzk).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Price.PriceEur).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Price).Must(x => x.PriceCzk >= x.PriceEur);
        RuleFor(x => x.ExhibitionDayIds.Count).GreaterThan(0);
        RuleFor(x => x.ExhibitionDayIds).Must(x => x.Distinct().Count() == x.Count);
    }
}
