#region

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.RegistrationToExhibition.Commands.BalancePayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.ChangeAdvertisement;
using RegisterMe.Application.RegistrationToExhibition.Commands.CreateRegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Commands.DeleteRegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Commands.DeleteTemporaryRegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Commands.FinishDelayedPayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.FinishOnlinePayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.RequestDelayedPayment;
using RegisterMe.Application.RegistrationToExhibition.Commands.StartOnlinePayment;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.ExportAllRegistrationsToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Queries.ExportRegistrationToExhibitionByExhibitionToZip;
using RegisterMe.Application.RegistrationToExhibition.Queries.ExportRegistrationToExhibitionToZip;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationsToExhibitionByExhibitorId;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionByExhibitionId;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionByExhibitorIdAndExhibitionId;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Application.RegistrationToExhibition.Queries.HasActiveRegistrations;
using RegisterMe.Application.RegistrationToExhibition.Queries.VerifyEmsByRegistrationToExhibitionId;
using RegisterMe.Domain.Common;
using RegisterMe.Infrastructure;
using WebApi.Dtos;
using WebApi.Infrastructure;

#endregion

namespace WebApi.Endpoints;

public class RegistrationsToExhibition : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup(this, "registrations-to-exhibition");
        group.MapPost(CreateRegistrationToExhibition, "exhibition/{exhibitionId}")
            .WithDescription("Create registration to exhibition");
        group.MapPut(UpdateRegistrationToExhibition, "{registrationToExhibitionId}")
            .WithDescription("Update registration to exhibition");
        group.MapDelete(DeleteRegistrationToExhibition, "{registrationToExhibitionId}")
            .WithDescription("Delete registration to exhibition");
        group.MapPost(StartOnlinePayment, "{registrationToExhibitionId}/start-online-payment")
            .WithDescription("Start online payment");
        group.MapPost(RequestDelayedPayment, "{registrationToExhibitionId}/request-delayed-payment")
            .WithDescription("Request delayed payment");
        group.MapGet(GetRegistrationToExhibitionByExhibitorIdAndExhibitionId,
                "{exhibitionId}/exhibitor-and-user/{exhibitorId}")
            .WithDescription("Get registration to exhibition by exhibitor id and exhibition id");
        group.MapGet(HasActiveRegistrations, "{exhibitionId}/existing-cat-registrations/{userId}")
            .WithDescription("Has active registrations");
        group.MapGet(GetCatRegistrationToExhibitionByExhibitionId, "exhibitions/{exhibitionId}")
            .WithDescription("Get cat registration to exhibition by exhibition id");
        group.MapGet(GetRegistrationsToExhibitionByExhibitorId, "exhibitors/{exhibitorId}")
            .WithDescription("Get registrations to exhibition by exhibitor id");
        group.MapGet(GetRegistrationToExhibitionById, "{registrationToExhibitionId}")
            .WithDescription("Get registration to exhibition by id");
        group.MapGet(HasValidEmsQuery, "{registrationToExhibitionId}/ems").WithDescription("Has valid ems");
        group.MapPost(BalancePaymentCommand, "{registrationToExhibitionId}/balance-payment")
            .WithDescription("Balance payment");
        group.MapPost(FinishOnlinePaymentCommand, "{registrationToExhibitionId}/finish-online-payment").WithDescription(
            "Finish online payment");
        group.MapPost(FinishDelayedPaymentCommand, "{registrationToExhibitionId}/finish-delayed-payment")
            .WithDescription("Finish delayed payment");
        group.MapDelete(DeleteTemporaryRegistrationToExhibitionCommand, "{exhibitionId}/temporary-registration")
            .WithDescription("Delete temporary registration to exhibition");
        group.MapGet(ExportAllRegistrationsToExhibitionQuery, "export-all/{exhibitionId}").WithDescription(
            "Export all registrations to exhibition");
        group.MapGet(ExportRegistrationToExhibitionToZipQuery, "{registrationToExhibitionId}/export-to-zip")
            .WithDescription("Export registration to exhibition to zip");
        group.MapGet(ExportRegistrationToExhibitionByExhibitionToZipQuery, "/export-to-zip/{exhibitionId}");
    }

    private static async Task<Results<Ok, BadRequest<string>>> BalancePaymentCommand(
        [FromServices] IWebHostEnvironment env,
        [FromServices] ISender sender, [FromRoute] int registrationToExhibitionId)
    {
        BalancePaymentCommand query = new()
        {
            RegistrationToExhibitionId = registrationToExhibitionId,
            WebAddress = GetWebAddress.WebAddress,
            RootPath = env.ContentRootPath
        };
        Result result = await sender.Send(query);
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> UpdateRegistrationToExhibition(
        [FromServices] ISender sender,
        [FromRoute] int registrationToExhibitionId,
        ChangeAdvertisementsCommand command)
    {
        if (command.RegistrationToExhibitionId != registrationToExhibitionId)
        {
            return TypedResults.BadRequest("Invalid registration registrationToExhibitionId");
        }

        Result result = await sender.Send(command);

        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Created<int>, BadRequest<string>>> CreateRegistrationToExhibition(
        [FromServices] ISender sender, [FromRoute] int exhibitionId,
        [FromBody] CreateRegistrationToExhibitionDto createRegistrationToExhibitionCommand)
    {
        if (createRegistrationToExhibitionCommand.ExhibitionId != exhibitionId)
        {
            return TypedResults.BadRequest("Invalid exhibition id");
        }

        CreateRegistrationToExhibitionCommand command = new()
        {
            RegistrationToExhibition = createRegistrationToExhibitionCommand
        };
        Result<int> result = await sender.Send(command);
        Results<Created<int>, BadRequest<string>> parsedResult = ResultHelper.ParseCreatedResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteRegistrationToExhibition(
        [FromServices] ISender sender, [FromRoute] int registrationToExhibitionId)
    {
        Result result =
            await sender.Send(new DeleteRegistrationToExhibitionCommand
            {
                RegistrationToExhibitionId = registrationToExhibitionId
            });
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteTemporaryRegistrationToExhibitionCommand(
        [FromServices] ISender sender, [FromRoute] int exhibitionId)
    {
        Result result =
            await sender.Send(
                new DeleteTemporaryRegistrationToExhibitionCommand
                {
                    ExhibitionId = exhibitionId, WebAddress = GetWebAddress.WebAddress
                });
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> FinishDelayedPaymentCommand(
        [FromServices] ISender sender, [FromServices] IWebHostEnvironment env,
        [FromRoute] int registrationToExhibitionId)
    {
        Result result = await sender.Send(new FinishDelayedPaymentCommand
        {
            RegistrationToExhibitionId = registrationToExhibitionId,
            WebAddress = GetWebAddress.WebAddress,
            RootPath = env.ContentRootPath
        });
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> FinishOnlinePaymentCommand([FromServices] ISender sender,
        [FromServices] IWebHostEnvironment env,
        [FromRoute] int registrationToExhibitionId)
    {
        Result result = await sender.Send(new FinishOnlinePaymentCommand
        {
            RegistrationToExhibitionId = registrationToExhibitionId,
            WebAddress = GetWebAddress.WebAddress,
            RootPath = env.ContentRootPath
        });
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok, BadRequest<string>>> RequestDelayedPayment([FromServices] ISender sender,
        [FromServices] IWebHostEnvironment env,
        [FromRoute] int registrationToExhibitionId,
        [AsParameters] RequestDelayedPaymenParamRequest delayedPaymentCommand)
    {
        Result result = await sender.Send(new RequestDelayedPaymentCommand
        {
            RegistrationToExhibitionId = registrationToExhibitionId,
            PaymentType = delayedPaymentCommand.PaymentType,
            Currency = delayedPaymentCommand.Currency,
            WebAddress = GetWebAddress.WebAddress,
            RootPath = env.ContentRootPath
        });
        Results<Ok, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(result);
        return parsedResult;
    }

    private static async Task<Results<Ok<SessionDto>, BadRequest<string>>> StartOnlinePayment(
        [FromServices] ISender sender,
        [FromRoute] int registrationToExhibitionId,
        StartOnlinePaymentCommand command)
    {
        if (command.RegistrationToExhibitionId != registrationToExhibitionId)
        {
            return TypedResults.BadRequest("Invalid registration registrationToExhibitionId");
        }

        Result<SessionDto> data = await sender.Send(command);
        Results<Ok<SessionDto>, BadRequest<string>> parsedResult = ResultHelper.ParseOkResult(data);
        return parsedResult;
    }

    private static async Task<Ok<string>> ExportAllRegistrationsToExhibitionQuery([FromServices] ISender sender,
        [FromRoute] int exhibitionId)
    {
        ExportAllRegistrationsToExhibitionQuery query = new() { ExhibitionId = exhibitionId };
        string data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Ok<Stream>> ExportRegistrationToExhibitionToZipQuery([FromServices] ISender sender,
        [FromServices] IWebHostEnvironment env,
        [FromRoute] int registrationToExhibitionId)
    {
        ExportRegistrationToExhibitionToZipQuery query = new()
        {
            Id = registrationToExhibitionId, RootPath = env.ContentRootPath, WebUrl = GetWebAddress.WebAddress
        };
        Stream data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Ok<Stream>> ExportRegistrationToExhibitionByExhibitionToZipQuery(
        [FromServices] ISender sender,
        [FromServices] IWebHostEnvironment env,
        [FromRoute] int exhibitionId)
    {
        ExportRegistrationToExhibitionByExhibitionToZipQuery query = new()
        {
            Id = exhibitionId, RootPath = env.ContentRootPath, WebUrl = GetWebAddress.WebAddress
        };
        Stream data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Ok<List<RegistrationToExhibitionDto>>> GetRegistrationsToExhibitionByExhibitorId(
        [FromServices] ISender sender, [FromRoute] int exhibitorId)
    {
        GetRegistrationsToExhibitionByExhibitorIdQuery query = new() { ExhibitorId = exhibitorId };
        List<RegistrationToExhibitionDto> data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Ok<List<RegistrationToExhibitionDto>>> GetCatRegistrationToExhibitionByExhibitionId(
        [FromServices] ISender sender, [FromRoute] int exhibitionId)
    {
        GetRegistrationsToExhibitionByExhibitionIdQuery query = new() { ExhibitionId = exhibitionId };
        List<RegistrationToExhibitionDto> data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Results<Ok<RegistrationToExhibitionDto>, Ok>>
        GetRegistrationToExhibitionByExhibitorIdAndExhibitionId(
            [FromServices] ISender sender,
            [FromRoute] int exhibitionId, [FromRoute] int exhibitorId)
    {
        RegistrationToExhibitionDto? data = await sender.Send(
            new GetRegistrationToExhibitionByExhibitorIdAndExhibitionIdQuery
            {
                ExhibitionId = exhibitionId, ExhibitorId = exhibitorId
            });
        if (data == null)
        {
            return TypedResults.Ok();
        }

        return TypedResults.Ok(data);
    }

    private static async Task<Ok<RegistrationToExhibitionDto>> GetRegistrationToExhibitionById(
        [FromServices] ISender sender, [FromRoute] int registrationToExhibitionId)
    {
        GetRegistrationToExhibitionByIdQuery query = new() { RegistrationToExhibitionId = registrationToExhibitionId };
        RegistrationToExhibitionDto data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Ok<bool>> HasActiveRegistrations([FromServices] ISender sender,
        [FromQuery] bool? paid, [FromRoute] int exhibitionId, [FromRoute] string userId)
    {
        HasActiveRegistrationsQuery query = new() { Paid = paid, ExhibitionId = exhibitionId, UserId = userId };
        bool data = await sender.Send(query);
        return TypedResults.Ok(data);
    }

    private static async Task<Ok<bool>> HasValidEmsQuery([FromServices] ISender sender,
        [FromQuery] int registrationToExhibitionId)
    {
        HasValidEmsQuery query = new() { RegistrationToExhibitionId = registrationToExhibitionId };
        bool data = await sender.Send(query);
        return TypedResults.Ok(data);
    }
}
