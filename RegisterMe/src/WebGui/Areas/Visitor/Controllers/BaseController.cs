#region

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Domain.Common;
using ValidationException = RegisterMe.Application.Common.Exceptions.ValidationException;

#endregion

namespace WebGui.Areas.Visitor.Controllers;

public abstract class BaseController(IAuthorizationService authorizationService, IMediator mediator) : Controller
{
    public const string CommandValidation = "cmdVal";

    protected string GetUserId()
    {
        ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
        if (claimsIdentity == null)
        {
            throw new Exception("");
        }

        string? userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            throw new Exception("");
        }

        return userId;
    }

    protected async Task<Result<T>> SendCommand<T>(IRequest<Result<T>> request, bool displayError = true,
        string? successMessage = null, string? errorMessage = null, bool displaySuccess = true)
    {
        Result<T> result;
        try
        {
            result = await mediator.Send(request);
        }
        catch (ValidationException e)
        {
            string allErrors = HandleValidationErrors(e);
            result = Result.Failure<T>(Errors.ErrorWithCustomMessage(allErrors));
        }

        HandleResult(result, displayError, successMessage, errorMessage, displaySuccess);
        return result;
    }

    protected async Task<Result> SendCommand(IRequest<Result> request, bool displayError = true,
        string? successMessage = null, string? errorMessage = null, bool displaySuccess = true)
    {
        Result result;
        try
        {
            result = await mediator.Send(request);
        }
        catch (ValidationException e)
        {
            string allErrors = HandleValidationErrors(e);
            result = Result.Failure(Errors.ErrorWithCustomMessage(allErrors));
        }

        HandleResult(result, displayError, successMessage, errorMessage, displaySuccess);
        return result;
    }

    private static string HandleValidationErrors(ValidationException e)
    {
        IEnumerable<string> errorsPerField = e.Errors.Select(x => string.Join(" ", x.Value));
        string allErrors = string.Join(" ", errorsPerField);
        if (string.IsNullOrWhiteSpace(allErrors))
        {
            allErrors = e.Message;
        }

        return allErrors;
    }

    private void HandleResult(Result result, bool displayError = true, string? successMessage = null,
        string? errorMessage = null, bool displaySuccess = true)
    {
        if (result.IsFailure)
        {
            string error = errorMessage ?? result.Error.Message;
            ModelState.AddModelError(CommandValidation, error);
            if (displayError)
            {
                TempData["error"] = error;
            }
        }

        if (result.IsSuccess && displaySuccess)
        {
            string success = successMessage ?? "Akce byla úspěšně provedena";
            TempData["success"] = success;
        }
    }

    protected async Task<T> SendQuery<T>(IRequest<T> request)
    {
        T result = await mediator.Send(request);
        return result;
    }

    private static Result<string> ServerSideValidate(Func<IActionResult> function)
    {
        IActionResult actionResult = function();

        if (actionResult is not JsonResult jsonResult)
        {
            return Result.Failure<string>(Errors.UnknownError);
        }

        return jsonResult.Value switch
        {
            true => Result.Success(""),
            string stringValue => Result.Failure<string>(Errors.ErrorWithCustomMessage(stringValue)),
            _ => Result.Failure<string>(Errors.UnknownError)
        };
    }

    protected async Task AuthorizeAsync(
        ClaimsPrincipal user,
        object? resource,
        IAuthorizationRequirement requirement)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(User, resource, Operations.Read);
        if (!authorizationResult.Succeeded)
        {
            throw new Exception("Unauthorized");
        }
    }

    protected void ServerSideValidate(string key, Func<IActionResult> function)
    {
        Result<string> x = ServerSideValidate(function);
        if (x.IsFailure)
        {
            ModelState.AddModelError(key, x.Error.Message);
        }
    }
}
