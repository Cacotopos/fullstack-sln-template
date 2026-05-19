using Company.Product.Application.Extensions;
using Company.Product.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Company.Product.Worker;

public sealed class Worker(ILogger<Worker> logger, IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker started at {time}", DateTimeOffset.UtcNow);

        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }

        logger.LogInformation("Worker stopped at {time}", DateTimeOffset.UtcNow);
    }

    private async Task DoWorkAsync(CancellationToken ct)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Worker>>();

        try
        {
            logger.LogInformation("Doing periodic background work at {time}", DateTimeOffset.UtcNow);
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
            logger.LogInformation("Periodic work completed at {time}", DateTimeOffset.UtcNow);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            logger.LogError(ex, "Error during background work");
        }
    }
}
