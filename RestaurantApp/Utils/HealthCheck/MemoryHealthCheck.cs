using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RestaurantApp.Utils.HealthCheck;

public class MemoryHealthCheck : IHealthCheck
{
    private readonly long _threshold;

    public MemoryHealthCheck(long threshold)
    {
        _threshold = threshold;
    }
    
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var totalMemory = GC.GetTotalMemory(false);
        var totalMemoryInMB = totalMemory / (1024 * 1024);
        var thresholdInMB = _threshold / (1024 * 1024);

        if (totalMemory < _threshold)
        {
            return Task.FromResult(HealthCheckResult.Healthy($"Memory usage is under {thresholdInMB} MB (Current: {totalMemoryInMB} MB)"));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy($"Memory usage is too high: {totalMemoryInMB} MB (Threshold: {thresholdInMB} MB)"));
    }
}