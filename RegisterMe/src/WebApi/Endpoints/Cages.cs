#region

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Cages.Dtos.Combination;
using RegisterMe.Application.Cages.Queries.GetAvailableRentedCageTypes;
using RegisterMe.Application.Cages.Queries.GetPersonCageById;
using RegisterMe.Application.Cages.Queries.GetPersonCagesByExhibitionDay;
using RegisterMe.Application.Cages.Queries.GetRentedCageGroupById;
using RegisterMe.Application.Cages.Queries.RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageId;
using WebApi.Infrastructure;

#endregion

namespace WebApi.Endpoints;

// ReSharper disable always UnusedType.Global
public class Cages : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(this, "cages");
        group.MapGet(GetAvailableRentedCageTypes, "available").WithDescription("Get available rented cage types");
        group.MapGet(GetPersonCageById, "{PersonCageId}").WithDescription("Get person cage by id");
        group.MapGet(GetPersonCagesByExhibitionDay, "exhibition-day/{exhibitionDayId}")
            .WithDescription("Get person cages by exhibition day");
        group.MapGet(GetRentedCageGroupById, "rented-cage-group/{cagesId}")
            .WithDescription("Get rented cage group by id");
        group.MapGet(GetRecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQuery, "exists")
            .WithDescription("Check if record exists");
    }

    private static async Task<Ok<CagesPerDayDto>> GetAvailableRentedCageTypes([FromServices] ISender sender,
        [AsParameters] GetAvailableRentedCageTypesQuery getAvailableCages)
    {
        return TypedResults.Ok(await sender.Send(getAvailableCages));
    }

    private static async Task<Ok<CageDto>> GetPersonCageById([FromServices] ISender sender,
        [FromRoute] int personCageId)
    {
        GetPersonCageByIdQuery query = new() { PersonCageId = personCageId };
        return TypedResults.Ok(await sender.Send(query));
    }

    private static async Task<Ok<List<CageDto>>> GetPersonCagesByExhibitionDay([FromServices] ISender sender,
        [FromRoute] int exhibitionDayId)
    {
        GetPersonCagesByExhibitionDayQuery query = new() { ExhibitionDayId = exhibitionDayId };
        List<CageDto> result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<BriefCageDto>> GetRentedCageGroupById([FromServices] ISender sender,
        [FromRoute] string cagesId)
    {
        GetRentedCageGroupByIdQuery query = new() { CagesId = cagesId };
        BriefCageDto result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<bool>> GetRecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQuery(
        [FromServices] ISender sender,
        [AsParameters] RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQuery query)
    {
        bool result = await sender.Send(query);
        return TypedResults.Ok(result);
    }
}
