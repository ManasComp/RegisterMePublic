#region

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using RegisterMe.Application.Common.Interfaces;

#endregion

namespace RegisterMe.Application.Common.Behaviours;

public class LoggingPerformanceBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    IUser user)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private const int MaxLengthOfQuickRequest = 500;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Stopwatch timer = new();
        string guid = Guid.NewGuid().ToString();
        string requestName = typeof(TRequest).Name;
        string userId = user.Id ?? "Anonymous";
        logger.LogInformation("Starting request: {Guid};{Name};{@UserId};{@Request};{@DateTimeUtc}",
            guid, requestName, userId, request, DateTime.UtcNow);

        timer.Start();

        TResponse response = await next();

        timer.Stop();

        long elapsedMilliseconds = timer.ElapsedMilliseconds;

        if (elapsedMilliseconds >= MaxLengthOfQuickRequest)
        {
            logger.LogWarning(
                "Long Running Request: {Guid};{Name};{@UserId};{ElapsedMilliseconds} milliseconds",
                guid, requestName, elapsedMilliseconds, userId);
        }

        logger.LogInformation("Completed request: {Guid};{Name};{@UserId};{@Response};{@DateTimeUtc}",
            guid, requestName, userId, response, DateTime.UtcNow);

        return response;
    }
}
