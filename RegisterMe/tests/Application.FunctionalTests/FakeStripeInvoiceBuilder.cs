#region

using RegisterMe.Application.Pricing;
using RegisterMe.Domain.Enums;
using Stripe.Checkout;

#endregion

namespace RegisterMe.Application.FunctionalTests;

public class FakeStripeInvoiceBuilder : IStripeInvoiceBuilder
{
    public Task<List<SessionLineItemOptions>> GetInvoice(int registrationToExhibitionId, Currency currency)
    {
        return Task.FromResult(new List<SessionLineItemOptions>());
    }
}
