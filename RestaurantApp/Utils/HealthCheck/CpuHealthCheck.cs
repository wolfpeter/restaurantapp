using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RestaurantApp.Utils.HealthCheck;

public class CpuHealthCheck : IHealthCheck
{
    private readonly double _cpuThreshold;
    private readonly TimeSpan _samplingDuration;

    public CpuHealthCheck(double cpuThreshold, TimeSpan samplingDuration)
    {
        _cpuThreshold = cpuThreshold;
        _samplingDuration = samplingDuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var cpuUsage = await GetCpuUsageForProcess(_samplingDuration);

        if (cpuUsage < _cpuThreshold)
        {
            return HealthCheckResult.Healthy($"CPU usage is under {_cpuThreshold}% (Current: {cpuUsage:0.##}%)");
        }

        return HealthCheckResult.Unhealthy($"CPU usage is too high: {cpuUsage:0.##}% (Threshold: {_cpuThreshold}%)");
    }

    private async Task<double> GetCpuUsageForProcess(TimeSpan duration)
    {
        var process = Process.GetCurrentProcess();

        var startCpuTime = process.TotalProcessorTime;
        var startTime = DateTime.UtcNow;

        await Task.Delay(duration);

        var endCpuTime = process.TotalProcessorTime;
        var endTime = DateTime.UtcNow;

        var cpuUsedMs = (endCpuTime - startCpuTime).TotalMilliseconds;
        var elapsedMs = (endTime - startTime).TotalMilliseconds;

        var cpuUsage = (cpuUsedMs / elapsedMs) * 100 / Environment.ProcessorCount;

        return cpuUsage;
    }
}