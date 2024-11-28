#region

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Commands.DeleteOrganization;
using RegisterMe.Application.Organizations.Commands.UpdateOrganization;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Queries.GetOrganizationById;
using RegisterMe.Application.Organizations.Queries.GetOrganizations;
using RegisterMe.Application.Organizations.Queries.IsOrganizationAdministrator;
using RegisterMe.Domain.Common;
using WebApi.Infrastructure;

#endregion

namespace WebApi.Endpoints;

public class Organizations : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(this, "organizations");
        group.MapPost(CreateOrganization).WithDescription("Create organization");
        group.MapGet(GetOrganizationById, "{organizationId}").WithDescription("Get organization by id");
        group.MapGet(GetOrganizations).WithDescription("Get organizations");
        group.MapPut(UpdateOrganization, "{organizationId}").WithDescription("Update organization");
        group.MapPut(ConfirmOrganization, "{organizationId}/confirm").WithDescription("Confirm organization");
        group.MapDelete(DeleteOrganizationCommand, "{organizationId}").WithDescription("Delete organization");
        group.MapGet(IsOrganizationAdministratorQuery, "{organizationId}/is-administrator/{userId}")
            .WithDescription("Check if user is organization administrator");
    }

    private static async Task<Results<Created<int>, BadRequest<string>>> CreateOrganization(
        [FromServices] ISender sender,
        [FromBody] CreateOrganizationDto createOrganization)
    {
        CreateOrganizationCommand createOrganizationCommand = new() { CreateOrganizationDto = createOrganization };
        Result<int> data = await sender.Send(createOrganizationCommand);
        Results<Created<int>, BadRequest<string>> parsedResult = ResultHelper.ParseCreatedResult(data);
        return parsedResult;
    }

    private static async Task<Results<Ok<Result>, BadRequest<string>>> UpdateOrganization([FromServices] ISender sender,
        [FromRoute] int organizationId,
        [FromBody] UpdateOrganizationDto updateOrganization)
    {
        if (organizationId != updateOrganization.Id)
        {
            return TypedResults.BadRequest("Id mismatch");
        }

        UpdateOrganizationCommand command = new() { OrganizationDto = updateOrganization };
        Result result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<OrganizationDto>> GetOrganizationById([FromServices] ISender sender,
        [FromRoute] int organizationId)
    {
        GetOrganizationByIdQuery query = new() { OrganizationId = organizationId };
        OrganizationDto data = await sender.Send(query);
        return TypedResults.Ok(data);
    }


    private static async Task<Ok<PaginatedList<OrganizationDto>>> GetOrganizations(ISender sender,
        [AsParameters] GetOrganizationsQuery query)
    {
        PaginatedList<OrganizationDto> data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Results<Ok, BadRequest<string>>> ConfirmOrganization([FromServices] ISender sender,
        [FromRoute] int organizationId)
    {
        ConfirmOrganizationCommand command = new() { OrganizationId = organizationId };
        Result result = await sender.Send(command);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteOrganizationCommand([FromServices] ISender sender,
        [FromRoute] int organizationId)
    {
        DeleteOrganizationCommand command = new() { OrganizationId = organizationId };
        Result result = await sender.Send(command);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Ok<bool>> IsOrganizationAdministratorQuery([FromServices] ISender sender,
        [FromRoute] int organizationId, [FromRoute] string userId)
    {
        bool result =
            await sender.Send(
                new IsOrganizationAdministratorQuery { UserId = userId, OrganizationId = organizationId });
        return TypedResults.Ok(result);
    }
}
