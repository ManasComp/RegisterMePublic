#region

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.RegistrationToExhibition;

#endregion

namespace RegisterMe.Application.BackgroundWorkers;

public class TimedHostedService(ILogger<TimedHostedService> logger, IServiceProvider services)
    : IHostedService, IDisposable
{
    private const int RunEveryHour = 1;
    private CancellationToken _cancellationToken;
    private Timer? _timer;
    private string WebAddress { get; set; } = string.Empty;

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;
        logger.LogInformation("Automatic deletion of temporary registrations started");

        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(1), TimeSpan.FromHours(RunEveryHour));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Automatic deletion of temporary registrations is stopping");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public Task StartAsync(string webAddress, CancellationToken stoppingToken = default)
    {
        WebAddress = webAddress;
        return StartAsync(stoppingToken);
    }

    private async void DoWork(object? state)
    {
        logger.LogInformation("Automatic deletion of temporary registrations is being executed");
        using IServiceScope scope = services.CreateScope();
        IRegistrationToExhibitionService scopedProcessingService =
            scope.ServiceProvider.GetRequiredService<IRegistrationToExhibitionService>();
        IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        NotifierService notifierService = scope.ServiceProvider.GetRequiredService<NotifierService>();
        try
        {
            await using IDbContextTransaction transaction =
                await dbContext.DatabaseFacade.BeginTransactionAsync(_cancellationToken);

            try
            {
                await notifierService.NotifyRegistrations(WebAddress, _cancellationToken);
                await scopedProcessingService.DeleteTemporaryRegistrations(null, _cancellationToken);
                await transaction.CommitAsync(_cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(_cancellationToken);

                logger.LogError(ex, "Error occurred while deleting temporary registrations, transaction rolled back");
                throw;
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Operation was canceled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred executing DeleteTemporaryRegistrations");
        }
    }
}
