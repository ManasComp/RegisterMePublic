#region

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Commands.DeleteCatRegistration;
using RegisterMe.Application.CatRegistrations.Commands.UpdateCatRegistrationCommand;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.CatRegistrations.Queries.GetCatRegistrationById;
using RegisterMe.Application.CatRegistrations.Queries.GetUserCatsNotInExhibition;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;
using WebApi.Dtos;
using WebApi.Infrastructure;

#endregion

namespace WebApi.Endpoints;

public class CatRegistrations : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(this, "cat-registrations");
        group.MapPost(CreateCatRegistrationWithCat, "cat").WithDescription("Create cat registration");
        group.MapPost(CreateCatRegistrationWithLitter, "litter").WithDescription("Create cat registration");
        group.MapDelete(DeleteCatRegistration, "{catRegistrationId}").WithDescription("Delete cat registration");
        group.MapPut(UpdateCatRegistration, "{catRegistrationId}").WithDescription("Update cat registration");
        group.MapGet(GetCatRegistrationById, "{catRegistrationId}").WithDescription("Get cat registration by id");
        group.MapGet(GetUserCatsNotInExhibition, "user-cats-not-in-exhibition/{registrationToExhibitionId}")
            .WithDescription("Get user cats not in exhibition");
    }

    private static async Task<Results<Created<int>, BadRequest<string>>> CreateCatRegistrationWithCat(
        [FromServices] ISender sender,
        [FromBody] CreteCatRequest creteCatRequest)
    {
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                CatDays = creteCatRequest.CatDay,
                Note = creteCatRequest.Note,
                ExhibitedCat = creteCatRequest.ExhibitedCat,
                Litter = null,
                RegistrationToExhibitionId = creteCatRequest.RegistrationToExhibitionId
            }
        };

        Result<int> result = await sender.Send(command);
        Results<Created<int>, BadRequest<string>> parsedResult = ResultHelper.ParseCreatedResult(result);
        return parsedResult;
    }

    private static async Task<Results<Created<int>, BadRequest<string>>> CreateCatRegistrationWithLitter(
        [FromServices] ISender sender,
        [FromBody] CreteLitterRequest creteCatRequest)
    {
        CreateCatRegistrationCommand command = new()
        {
            CatRegistration = new CreateCatRegistrationDto
            {
                CatDays = creteCatRequest.CatDay,
                Note = creteCatRequest.Note,
                ExhibitedCat = null,
                Litter = creteCatRequest.Litter,
                RegistrationToExhibitionId = creteCatRequest.RegistrationToExhibitionId
            }
        };

        Result<int> result = await sender.Send(command);
        Results<Created<int>, BadRequest<string>> parsedResult = ResultHelper.ParseCreatedResult(result);
        return parsedResult;
    }


    private static async Task<Results<Ok, BadRequest<string>>> DeleteCatRegistration([FromServices] ISender sender,
        [FromRoute] int catRegistrationId)
    {
        DeleteCatRegistrationCommand command = new() { CatRegistrationId = catRegistrationId };
        Result result = await sender.Send(command);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }


    private static async Task<Results<Ok<int>, BadRequest<string>>> UpdateCatRegistration([FromServices] ISender sender,
        [FromRoute] int catRegistrationId,
        [FromBody] UpdateCatRegistrationDto updateCatRegistration)
    {
        if (catRegistrationId != updateCatRegistration.Id)
        {
            return TypedResults.BadRequest("Id mismatch");
        }

        Result<int> result =
            await sender.Send(new UpdateCatRegistrationCommand { CatRegistration = updateCatRegistration });
        Results<Ok<int>, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Ok<CatRegistrationDto>> GetCatRegistrationById([FromServices] ISender sender,
        [FromRoute] int catRegistrationId)
    {
        GetCatRegistrationByIdQuery command = new() { Id = catRegistrationId };
        CatRegistrationDto catRegistration = await sender.Send(command);
        return TypedResults.Ok(catRegistration);
    }

    private static async Task<Ok<List<CatModelP>>> GetUserCatsNotInExhibition([FromServices] ISender sender,
        [FromRoute] int registrationToExhibitionId, [FromQuery] CatRegistrationType type)
    {
        GetUserCatsNotInExhibitionQuery query = new()
        {
            RegistrationToExhibitionId = registrationToExhibitionId, Type = type
        };
        List<CatModelP> catRegistration = await sender.Send(query);
        return TypedResults.Ok(catRegistration);
    }
}
