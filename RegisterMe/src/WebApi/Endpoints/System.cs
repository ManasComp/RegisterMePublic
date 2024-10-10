#region

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Services.Ems;
using RegisterMe.Application.System.Dtos;
using RegisterMe.Application.System.Queries.GetAllSupportedCats;
using RegisterMe.Application.System.Queries.GetSupportedCountriesQuery;
using RegisterMe.Application.System.Queries.ParseEms;
using RegisterMe.Application.System.Queries.PaymentByCardIsConfigured;
using RegisterMe.Application.System.Queries.RequiresGroup;
using RegisterMe.Domain.Common;
using WebApi.Dtos;
using WebApi.Infrastructure;

#endregion

namespace WebApi.Endpoints;

public class System : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(this, "system");
        group.MapGet(GetAllSupportedCatsQuery, "cats").WithDescription("Get all supported cats");
        group.MapGet(GetSupportedCountriesQuery, "countries").WithDescription("Get supported countries");
        group.MapPost(ParseEms, "parse-ems").WithDescription("Parse ems");
        group.MapGet(PaymentByCardIsConfiguredQuery, "payment-by-card-is-configured")
            .WithDescription("Check if payment by card is configured");
        group.MapGet(RequiresGroupQuery, "requires-group/{ems}").WithDescription("Check if requires group");
    }

    private static async Task<Ok<List<TypeOfCat>>> GetAllSupportedCatsQuery([FromServices] ISender sender)
    {
        GetAllSupportedCatsQuery query = new();
        List<TypeOfCat> result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<CountryDto>>> GetSupportedCountriesQuery([FromServices] ISender sender)
    {
        GetSupportedCountriesQuery query = new();
        List<CountryDto> result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok, BadRequest<string>>> ParseEms([FromServices] ISender sender,
        [AsParameters] EmsModelRequest modelRequest)
    {
        ParseEmsQuery query = new()
        {
            Breed = modelRequest.Breed, Colour = modelRequest.Colour, Ems = modelRequest.Ems
        };
        Result result = await sender.Send(query);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Ok<bool>> PaymentByCardIsConfiguredQuery([FromServices] ISender sender)
    {
        PaymentByCardIsConfiguredQuery query = new();
        bool result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<bool>> RequiresGroupQuery([FromServices] ISender sender,
        [FromRoute] string ems)
    {
        RequiresGroupQuery query = new() { Ems = ems };
        bool result = await sender.Send(query);
        return TypedResults.Ok(result);
    }
}
