#region

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.Exhibitors.Commands.UpdateExhibitor;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorById;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorByUserId;
using RegisterMe.Domain.Common;
using WebApi.Infrastructure;

#endregion

namespace WebApi.Endpoints;

public class Exhibitiors : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(this, "exhibitors");
        group.MapPost(CreateExhibitor, "{userId}").WithDescription("Create exhibitor");
        group.MapGet(GetExhibitorById, "by-exhibitor-id/{exhibitorId}").WithDescription("Get exhibitor by id");
        group.MapGet(GetExhibitorByUserId, "{userId}").WithDescription("Get exhibitor by user id");
        group.MapPut(UpdateExhibitor, "{userId}").WithDescription("Update exhibitor");
    }

    private static async Task<Results<Created<int>, BadRequest<string>>> CreateExhibitor([FromServices] ISender sender,
        [FromBody] CreateExhibitorCommand createExhibitorCommand, [FromRoute] string userId)
    {
        if (createExhibitorCommand.UserId != userId)
        {
            return TypedResults.BadRequest("User id in the body does not match the user id in the route");
        }

        Result<int> result = await sender.Send(createExhibitorCommand);
        Results<Created<int>, BadRequest<string>> parsedResult = ResultHelper.ParseCreatedResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> UpdateExhibitor([FromServices] ISender sender,
        [FromRoute] string userId, [FromBody] UpdateExhibitorCommand command)
    {
        if (command.AspNetUserId != userId)
        {
            return TypedResults.BadRequest("User id in the body does not match the user id in the route");
        }

        Result result = await sender.Send(command);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Ok<ExhibitorAndUserDto>> GetExhibitorById([FromServices] ISender sender,
        [FromRoute] int exhibitorId)
    {
        GetExhibitorByIdQuery command = new() { ExhibitorId = exhibitorId };
        ExhibitorAndUserDto data = await sender.Send(command);
        return TypedResults.Ok(data);
    }

    private static async Task<Results<Ok, Ok<ExhibitorAndUserDto>>> GetExhibitorByUserId([FromServices] ISender sender,
        [FromRoute] string userId)
    {
        GetExhibitorByUserIdQuery command = new() { UserId = userId };
        ExhibitorAndUserDto? data = await sender.Send(command);
        if (data != null)
        {
            return TypedResults.Ok(data);
        }

        return TypedResults.Ok();
    }
}
