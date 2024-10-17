#region

using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Pricing.Enums;

public record PaymentTypeWithCurrency(PaymentType PaymentType, Currency Currency);
