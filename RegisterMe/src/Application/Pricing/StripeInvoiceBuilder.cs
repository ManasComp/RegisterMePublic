#region

using RegisterMe.Application.Exhibitions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Domain.Enums;
using Stripe.Checkout;

#endregion

namespace RegisterMe.Application.Pricing;

public class StripeInvoiceBuilder(
    IPricingFacade pricingFacade,
    IExhibitionService exhibitionService,
    IRegistrationToExhibitionService registrationToExhibitionService) : IStripeInvoiceBuilder
{
    public async Task<List<SessionLineItemOptions>> GetInvoice(int registrationToExhibitionId,
        Currency currency)
    {
        RegistrationToExhibitionDto registrationToExhibition =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(registrationToExhibitionId);
        BriefExhibitionDto briefExhibition =
            await exhibitionService.GetExhibitionById(registrationToExhibition.ExhibitionId);
        RegistrationToExhibitionPrice price = await pricingFacade.GetPrice(registrationToExhibitionId);

        List<SessionLineItemOptions> sessionPrice = [];

        int finalExhibitionAmount = (int)(price.GExhibitorPrice.GetPriceForCurrency(currency) * 100);

        SessionLineItemOptions exhibitionPrice = new()
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = finalExhibitionAmount,
                Currency = currency.ToString().ToUpper(),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = "Registrace na výstavu" + briefExhibition.Name
                }
            },
            Quantity = 1
        };
        sessionPrice.Add(exhibitionPrice);

        sessionPrice.AddRange(price.CatRegistrationPrices.Select(item => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (int)(item.GetPrice().GetPriceForCurrency(currency) * 100),
                Currency = currency.ToString().ToUpper(),
                ProductData =
                    new SessionLineItemPriceDataProductDataOptions { Name = "Registrace kočky" + item.CatName }
            },
            Quantity = 1
        }));

        return sessionPrice;
    }
}
