#region

using Microsoft.AspNetCore.Http.HttpResults;
using RegisterMe.Domain.Common;

#endregion

namespace WebApi;

public static class ResultHelper
{
    public static Results<Ok<T>, BadRequest<string>> ParseOkResult<T>(Result<T> result)
    {
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    public static Results<Created<int>, BadRequest<string>> ParseCreatedResult(Result<int> result)
    {
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }

        int id = result.Value;
        return TypedResults.Created(string.Empty, id);
    }

    public static Results<Ok, BadRequest<string>> ParseOkResult(Result result)
    {
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }

        return TypedResults.Ok();
    }
}
