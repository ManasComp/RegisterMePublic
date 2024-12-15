#region

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Pricing.Enums;
using RegisterMe.Application.Pricing.Queries.GetAvailablePaymentTypes;
using RegisterMe.Application.Pricing.Queries.GetBeneficiaryMessage;
using RegisterMe.Application.Pricing.Queries.GetPrice;
using WebApi.Infrastructure;

#endregion

namespace WebApi.Endpoints;

public class Pricing : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(this, "pricing");
        group.MapGet(GetAvailablePaymentTypes, "available-payment-types/{registrationToExhibitionId}")
            .WithDescription("Get available payment types");
        group.MapGet(GetBeneficiaryMessageQuery, "beneficiary-message/{registrationToExhibitionId}")
            .WithDescription("Get beneficiary message");
        group.MapGet(GetPrice, "price/{registrationToExhibitionId}").WithDescription("Get price");
    }

    private static async Task<Ok<List<PaymentTypeWithCurrency>>> GetAvailablePaymentTypes([FromServices] ISender sender,
        [FromRoute] int registrationToExhibitionId)
    {
        GetAvailablePaymentTypesQuery query = new() { RegistrationToExhibitionId = registrationToExhibitionId };
        List<PaymentTypeWithCurrency> data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Ok<string>> GetBeneficiaryMessageQuery([FromServices] ISender sender,
        [FromRoute] int registrationToExhibitionId)
    {
        GetBeneficiaryMessageQuery query = new() { RegistrationToExhibitionId = registrationToExhibitionId };
        string data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Ok<RegistrationToExhibitionPrice>> GetPrice([FromServices] ISender sender,
        [FromRoute] int registrationToExhibitionId)
    {
        GetPriceQuery query = new() { RegistrationToExhibitionId = registrationToExhibitionId };
        RegistrationToExhibitionPrice data = await sender.Send(query);
        return TypedResults.Ok(data);
    }
}
