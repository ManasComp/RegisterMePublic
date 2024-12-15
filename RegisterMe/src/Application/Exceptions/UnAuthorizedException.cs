#region

using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exceptions;

public static class UnAuthorizedException
{
    public static void UnAuthorized(this IGuardClause guardClause,
        AuthorizationResult authorizationResult,
        [CallerArgumentExpression("authorizationResult")]
        string? parameterName = null)
    {
        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenAccessException();
        }
    }
}

public static class ResultFailedException
{
    public static void ResultFailed(this IGuardClause guardClause,
        Result result,
        [CallerArgumentExpression("result")] string? parameterName = null)
    {
        if (!result.IsSuccess)
        {
            throw new HttpRequestException(result.Error);
        }
    }
}
