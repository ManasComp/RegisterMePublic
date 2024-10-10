#region

using Microsoft.Extensions.Logging;

#endregion

namespace RegisterMe.Application.BackgroundWorkers;

public class BackgroundTaskRunner(ILogger<BackgroundTaskRunner> logger)
{
    /// <summary>
    ///     For asynchronous tasks
    /// </summary>
    public void FireRetryAndForgetTaskAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        Task.Run(async () =>
        {
            const int maxRetries = 3;
            const int delayBetweenRetries = 5000;
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                bool wasSuccessful =
                    await HandleAction(action, attempt, maxRetries, delayBetweenRetries, cancellationToken);
                if (wasSuccessful)
                {
                    break;
                }
            }
        }, cancellationToken);
    }

    private async Task<bool> HandleAction(Func<Task> action, int attempt,
        int maxRetries,
        int delayBetweenRetries, CancellationToken cancellationToken = default)
    {
        try
        {
            await action();
            return true;
        }

        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while executing task. Attempt {Attempt} of {MaxRetries}",
                attempt + 1, maxRetries);

            if (attempt == maxRetries - 1)
            {
                logger.LogError("Maximum retry attempts reached. Task failed");
            }
            else
            {
                await Task.Delay(delayBetweenRetries, cancellationToken);
            }
        }

        return false;
    }
}
