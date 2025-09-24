using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestaurantApp.DataAccess.DbContexts;

namespace RestaurantApp.Utils.HealthCheck;

public class DatabaseHealthCheck: IHealthCheck
{
    private readonly RestaurantAppDbContext _dbContext;

    public DatabaseHealthCheck(RestaurantAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            return HealthCheckResult.Healthy("Database is reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Database is not reachable: {ex.Message}");
        }
    }
}