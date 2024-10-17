#region

using RegisterMe.Domain.Enums;
using Stripe.Checkout;

#endregion

namespace RegisterMe.Application.Pricing;

public interface IStripeInvoiceBuilder
{
    Task<List<SessionLineItemOptions>> GetInvoice(int registrationToExhibitionId,
        Currency currency);
}
