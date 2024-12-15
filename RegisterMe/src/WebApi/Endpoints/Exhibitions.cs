#region

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Exhibitions.Commands.CancelExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;
using RegisterMe.Application.Exhibitions.Commands.CreateWorkflowCommand;
using RegisterMe.Application.Exhibitions.Commands.DeleteAdvertisementById;
using RegisterMe.Application.Exhibitions.Commands.DeleteDiscountCommand;
using RegisterMe.Application.Exhibitions.Commands.DeleteExhibition;
using RegisterMe.Application.Exhibitions.Commands.DeletePriceGroup;
using RegisterMe.Application.Exhibitions.Commands.DeleteRentedCages;
using RegisterMe.Application.Exhibitions.Commands.PublishExhibition;
using RegisterMe.Application.Exhibitions.Commands.UpdateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.UpdateDiscountWorkflow;
using RegisterMe.Application.Exhibitions.Commands.UpdateExhibition;
using RegisterMe.Application.Exhibitions.Commands.UpdatePriceGroup;
using RegisterMe.Application.Exhibitions.Commands.UpdateRentedCageGroupDto;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementById;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetAllExhibitionGroupsThatAreNotFullyRegistered;
using RegisterMe.Application.Exhibitions.Queries.GetCagesStatistics;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByGroupId;
using RegisterMe.Application.Exhibitions.Queries.GetDiscountById;
using RegisterMe.Application.Exhibitions.Queries.GetDiscountsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionCagesInfo;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionGroupById;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitions;
using RegisterMe.Application.Exhibitions.Queries.GetGroupsByPriceGroupId;
using RegisterMe.Application.Exhibitions.Queries.GetGroupsCanBeRegisteredIn;
using RegisterMe.Application.Exhibitions.Queries.GetPaymentsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetPrices;
using RegisterMe.Application.Exhibitions.Queries.GetRentedCagesByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.HasDrafts;
using RegisterMe.Application.Services.Groups;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities.RulesEngine;
using RulesEngine.Models;
using WebApi.Infrastructure;

#endregion

namespace WebApi.Endpoints;

public class Exhibitions : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(this, "exhibitions");
        group.MapPost(CreateExhibition).WithDescription("Create exhibition").WithDescription("Create exhibition");
        group.MapGet(GetExhibitionById, "{exhibitionId}").WithDescription("Get exhibition by id");
        group.MapGet(GetExhibitions).WithDescription("Get exhibitions");
        group.MapPut(PublishExhibition, "{exhibitionId}/publish").WithDescription("Publish exhibition");
        group.MapDelete(DeleteUnpublishedExhibition, "{exhibitionId}").WithDescription("Delete unpublished exhibition");
        group.MapPut(CancelExhibition, "{exhibitionId}/cancel").WithDescription("Cancel exhibition");
        group.MapPut(UpdateExhibition, "{exhibitionId}").WithDescription("Update exhibition");
        group.MapGet(GetPayments, "{exhibitionId}/payments").WithDescription("Get payments");
        group.MapGet(GetPrices, "{exhibitionId}/prices").WithDescription("Get prices");
        group.MapDelete(DeleteAdvertisementById, "advertisements/{advertisementId}")
            .WithDescription("Delete advertisement by id");
        group.MapPost(CreateAdvertisement, "{exhibitionId}/advertisements").WithDescription("Create advertisement");
        group.MapPost(CreatePrices, "{exhibitionId}/prices").WithDescription("Create prices");
        group.MapDelete(DeletePrices, "prices/{pricesId}").WithDescription("Delete prices");
        group.MapPut(UpdateAdvertisement, "advertisement/{advertisementId}").WithDescription("Update advertisement");
        group.MapPost(CreateRentedCage, "rented-cages").WithDescription("Create rented cage");
        group.MapPost(CreateWorkflowCommand, "{exhibitionId}/workflow").WithDescription("Create workflow command");
        group.MapDelete(DeleteRentedCages, "rented-cages/{cagesId}").WithDescription("Delete rented cages");
        group.MapPut(UpdatePaymentWorkflow, "payments/{discountId}").WithDescription("Update payment workflow");
        group.MapPut(UpdatePriceGroup, "prices/{pricesId}").WithDescription("Update price group");
        group.MapPut(UpdateRentedCage, "rented-cages/{cagesId}").WithDescription("Update rented cage");
        group.MapGet(GetAdvertisementById, "advertisements/{advertisementId}")
            .WithDescription("Get advertisement by id");
        group.MapGet(GetAdvertisementsByExhibitionId, "{exhibitionId}/advertisements")
            .WithDescription("Get advertisements by exhibition id");
        group.MapGet(GetAllDaysGroupsCanBeRegisteredTo, "{exhibitionId}/days")
            .WithDescription("Get all days groups can be registered to");
        group.MapGet(GetGroupsCanBeRegisteredTo, "groups-can-be-registered/{exhibitionDayId}")
            .WithDescription("Get groups can be registered to");

        group.MapGet(GetGroupsByPriceGroupIdQuery, "groups/{groupsId}/groups")
            .WithDescription("Get groups by price group id");
        group.MapGet(GetExhibitionGroupByIdQuery, "groups/{groupsId}")
            .WithDescription("Get exhibition price group by id");

        group.MapGet(GetHasDraftsQuery, "{exhibitionId}/has-drafts").WithDescription("Get has drafts query");
        group.MapGet(GetCagesStatistics, "{registrationToExhibitionId}/cages-statistics")
            .WithDescription("Get cages statistics");
        group.MapGet(GetDaysByGroupId, "days/{priceGroupIds}").WithDescription("Get days by group id");

        group.MapPut(UpdateDiscountWorkflow, "discounts/{discountId}").WithDescription("Update discount workflow");
        group.MapDelete(DeleteDiscountCommandCommand, "discounts/{discountId}")
            .WithDescription("Delete discount command");
        group.MapGet(GetDiscountById, "discounts/{discountId}").WithDescription("Get discount by id");

        group.MapGet(GetDiscountsByExhibitionId, "{exhibitionId}/discounts")
            .WithDescription("Get discounts by exhibition id");
        group.MapGet(GetExhibitionCagesInfoQuery, "{exhibitionId}/cages-info")
            .WithDescription("Get exhibition cages info query");

        group.MapGet(GetRentedCagesByExhibitionIdQuery, "{exhibitionId}/rented-cages")
            .WithDescription("Get rented cages by exhibition id");
        group.MapGet(GetAllGroupsThatAreNotFullyRegistered, "{exhibitionId}/groups")
            .WithDescription("Get all groups that are not fully registered");
    }

    private static async Task<Results<Created<int>, BadRequest<string>>> CreateExhibition(ISender sender,
        [FromBody] CreateExhibitionDto createExhibition)
    {
        Result<int> result = await sender.Send(new CreateExhibitionCommand { CreateExhibitionDto = createExhibition });

        Results<Created<int>, BadRequest<string>> parsedResult = ResultHelper.ParseCreatedResult(result);
        return parsedResult;
    }

    private static async Task<Ok<BriefExhibitionDto>> GetExhibitionById([FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        GetExhibitionByIdQuery query = new() { ExhibitionId = exhibitionId };
        BriefExhibitionDto data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Ok<PaginatedList<ExhibitionDto>>> GetExhibitions([FromServices] ISender sender,
        [AsParameters] GetExhibitionsQuery query)
    {
        PaginatedList<ExhibitionDto> data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Results<Ok, BadRequest<string>>> PublishExhibition([FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        Result result = await sender.Send(new PublishExhibitionCommand { ExhibitionId = exhibitionId });
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> UpdateExhibition([FromServices] ISender sender,
        [FromRoute] int exhibitionId, [FromBody] UpdateExhibitionCommand command)
    {
        if (exhibitionId != command.UpdateExhibitionDto.Id)
        {
            return TypedResults.BadRequest("Id in the route does not match the id in the body");
        }

        Result result = await sender.Send(command);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteUnpublishedExhibition(
        [FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        Result result = await sender.Send(new DeleteUnpublishedExhibitionCommand { ExhibitionId = exhibitionId });
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> CancelExhibition([FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        Result result = await sender.Send(new CancelExhibitionCommand { ExhibitionId = exhibitionId });
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Ok<Workflow>> GetPayments([FromServices] ISender sender, [FromRoute] int exhibitionId)
    {
        GetPaymentsByExhibitionIdQuery query = new() { ExhibitionId = exhibitionId };
        Workflow data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Ok<List<BigPriceDto>>> GetPrices([FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        GetPricesQuery query = new() { ExhibitionId = exhibitionId };
        return TypedResults.Ok(await sender.Send(query));
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteAdvertisementById([FromServices] ISender sender,
        [FromRoute] int advertisementId)
    {
        Result result = await sender.Send(new DeleteAdvertisementCommand { AdvertisementId = advertisementId });
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Created<int>, BadRequest<string>>> CreateAdvertisement(
        [FromServices] ISender sender, [FromRoute] int exhibitionId,
        [FromBody] CreateAdvertisementCommand command)
    {
        if (exhibitionId != command.ExhibitionId)
        {
            return TypedResults.BadRequest("ExhibitionId in the route does not match the ExhibitionId in the body");
        }

        Result<int> id = await sender.Send(command);
        Results<Created<int>, BadRequest<string>> parsedResult = ResultHelper.ParseCreatedResult(id);
        return parsedResult;
    }

    private static async Task<Results<Created<string>, BadRequest<string>>> CreatePrices([FromServices] ISender sender,
        [FromRoute] int exhibitionId,
        [FromBody] CreatePriceGroupCommand command)
    {
        if (exhibitionId != command.ExhibitionId)
        {
            return TypedResults.BadRequest("ExhibitionId in the route does not match the ExhibitionId in the body");
        }

        Result<string> data = await sender.Send(command);
        return TypedResults.Created($"{data}", data.Value);
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeletePrices([FromServices] ISender sender,
        [FromRoute] string pricesId)
    {
        Result result = await sender.Send(new DeletePriceGroupCommand { PriceIds = pricesId });
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> UpdateAdvertisement([FromServices] ISender sender,
        [FromBody] UpdateAdvertisementCommand command, [FromRoute] int advertisementId)
    {
        if (command.AdvertisementId != advertisementId)
        {
            return TypedResults.BadRequest("Id mismatch");
        }

        Result result = await sender.Send(command);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok<string>, BadRequest<string>>> CreateRentedCage([FromServices] ISender sender,
        [FromBody] CreateRentedRentedCageDto command)
    {
        Result<string> result =
            await sender.Send(new AddNewRentedCageGroupToExhibitionCommand { CreateRentedRentedCageDto = command });
        Results<Ok<string>, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok<int>, BadRequest<string>>> CreateWorkflowCommand([FromServices] ISender sender,
        [FromBody] CreateWorkflowCommandCommand command, int exhibitionId)
    {
        if (command.ExhibitionId != exhibitionId)
        {
            return TypedResults.BadRequest("ExhibitionId in the route does not match the ExhibitionId in the body");
        }

        Result<int> result = await sender.Send(command);
        Results<Ok<int>, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteDiscountCommandCommand(
        [FromServices] ISender sender, [FromRoute] int discountId)
    {
        DeleteDiscountCommand command = new() { Id = discountId };
        Result result = await sender.Send(command);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteRentedCages([FromServices] ISender sender,
        [FromRoute] string cagesId)
    {
        DeleteRentedCagesCommand command = new() { CagesId = cagesId };
        Result result = await sender.Send(command);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok<int>, BadRequest<string>>> UpdateDiscountWorkflow(
        [FromServices] ISender sender, [FromBody] UpdateDiscountWorkflowCommand updateDiscountWorkflow,
        [FromRoute] int discountId)
    {
        if (updateDiscountWorkflow.Id != discountId)
        {
            return TypedResults.BadRequest("Id mismatch");
        }

        Result<int> result = await sender.Send(updateDiscountWorkflow);
        Results<Ok<int>, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok<int>, BadRequest<string>>> UpdatePaymentWorkflow([FromServices] ISender sender,
        [FromBody] UpdateDiscountWorkflowCommand updateDiscountWorkflow, [FromRoute] int discountId)
    {
        if (updateDiscountWorkflow.Id != discountId)
        {
            return TypedResults.BadRequest("Id mismatch");
        }

        Result<int> result = await sender.Send(updateDiscountWorkflow);
        Results<Ok<int>, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }


    private static async Task<Results<Ok<string>, BadRequest<string>>> UpdatePriceGroup([FromServices] ISender sender,
        [FromBody] UpdatePriceGroupCommand updateDiscountWorkflow, [FromRoute] string pricesId)
    {
        if (updateDiscountWorkflow.OriginalPricesId != pricesId)
        {
            return TypedResults.BadRequest("Id mismatch");
        }

        Result<string> result = await sender.Send(updateDiscountWorkflow);
        Results<Ok<string>, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok<string>, BadRequest<string>>> UpdateRentedCage([FromServices] ISender sender,
        [FromBody] UpdateRentedCageGroupCommand updateDiscountWorkflow, [FromRoute] string cagesId)
    {
        if (updateDiscountWorkflow.CagesId != cagesId)
        {
            return TypedResults.BadRequest("Id mismatch");
        }

        Result<string> result = await sender.Send(updateDiscountWorkflow);
        Results<Ok<string>, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Ok<AdvertisementDto>> GetAdvertisementById([FromServices] ISender sender,
        [FromRoute] int advertisementId)
    {
        GetAdvertisementByIdQuery query = new() { AdvertisementId = advertisementId };
        AdvertisementDto result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<AdvertisementDto>>> GetAdvertisementsByExhibitionId([FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        GetAdvertisementsByExhibitionIdQuery query = new() { ExhibitionId = exhibitionId };
        List<AdvertisementDto> result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<ExhibitionDayDto>>> GetAllDaysGroupsCanBeRegisteredTo(
        [FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        GetDaysByExhibitionIdQuery query = new() { ExhibitionId = exhibitionId };
        List<ExhibitionDayDto> result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<DatabaseGroupDto>>> GetAllGroupsThatAreNotFullyRegistered(
        [FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        List<DatabaseGroupDto> result =
            await sender.Send(new GetAllExhibitionGroupsThatAreNotFullyRegisteredQuery { ExhibitionId = exhibitionId });
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<CatRegistrationStatistics>>> GetCagesStatistics([FromServices] ISender sender,
        [FromRoute] int registrationToExhibitionId)
    {
        List<CatRegistrationStatistics> result =
            await sender.Send(new GetCagesStatisticsQuery { RegistrationToExhibitionId = registrationToExhibitionId });
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<ExhibitionDayDto>>> GetDaysByGroupId([FromServices] ISender sender,
        [FromRoute] string priceGroupIds)
    {
        List<ExhibitionDayDto> result = await sender.Send(new GetDaysByGroupIdQuery { PriceGroupIds = priceGroupIds });
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<Workflow>> GetDiscountById([FromServices] ISender sender,
        [FromRoute] int discountId)
    {
        Workflow result = await sender.Send(new GetDiscountByIdQuery { WorkflowId = discountId });
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<WorkflowDto>>> GetDiscountsByExhibitionId([FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        List<WorkflowDto> result =
            await sender.Send(new GetDiscountsByExhibitionIdQuery { ExhibitionId = exhibitionId });
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<ExhibitionCagesInfo>>> GetExhibitionCagesInfoQuery([FromServices] ISender sender,
        [FromRoute] int exhibitionId,
        [FromQuery] int? exhibitionDayId,
        [FromQuery] int? doNotIncludeCatRegistrationId)
    {
        List<ExhibitionCagesInfo> result = await sender.Send(new GetExhibitionCagesInfoQuery
        {
            ExhibitionId = exhibitionId,
            ExhibitionDayId = exhibitionDayId,
            DoNotIncludeCatRegistrationId = doNotIncludeCatRegistrationId
        });
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<BigPriceDto>, BadRequest<string>>> GetExhibitionGroupByIdQuery(
        [FromServices] ISender sender,
        [FromRoute] string groupsId)
    {
        Result<BigPriceDto> result = await sender.Send(new GetExhibitionGroupByIdQuery { GroupsId = groupsId });
        Results<Ok<BigPriceDto>, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Ok<List<DatabaseGroupDto>>> GetGroupsByPriceGroupIdQuery([FromServices] ISender sender,
        [FromRoute] string groupsId)
    {
        GetGroupsByPriceGroupIdQuery query = new() { GroupId = groupsId };
        List<DatabaseGroupDto> result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<List<DatabaseGroupDto>>, BadRequest<string>>> GetGroupsCanBeRegisteredTo(
        [FromServices] ISender sender,
        [FromBody] GetGroupsCanBeRegisteredInQuery query, [FromRoute] int exhibitionDayId)
    {
        if (query.ExhibitionDayId != exhibitionDayId)
        {
            return TypedResults.BadRequest(
                "ExhibitionDayId in the route does not match the ExhibitionDayId in the body");
        }

        List<DatabaseGroupDto> result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<BriefCageDto>>> GetRentedCagesByExhibitionIdQuery([FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        GetRentedCagesByExhibitionIdQuery query = new() { ExhibitionId = exhibitionId };
        return TypedResults.Ok(await sender.Send(query));
    }

    private static async Task<Ok<bool>> GetHasDraftsQuery([FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        HasDraftsQuery query = new() { ExhibitionId = exhibitionId };
        bool result = await sender.Send(query);
        return TypedResults.Ok(result);
    }
}
