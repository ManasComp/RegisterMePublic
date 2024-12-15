#region

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Users.Command;
using RegisterMe.Domain.Common;
using WebApi.Infrastructure;

#endregion

namespace WebApi.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(this, "users");
        group.MapIdentityUserApi();
        group.MapDelete(DeletePersonalDataCommand, "delete-personal-data/{userId}")
            .WithDescription("Delete personal data");
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeletePersonalDataCommand([FromServices] ISender sender,
        [FromRoute] string userId)
    {
        DeletePersonalDataCommand query = new() { UserId = userId };
        Result result = await sender.Send(query);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }
}
