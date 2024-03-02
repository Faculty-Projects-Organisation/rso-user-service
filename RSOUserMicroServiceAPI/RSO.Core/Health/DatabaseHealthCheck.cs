using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RSO.Core.UserModels;

namespace RSO.Core.Health;

/// <summary>
/// Class instance for the database health check.
/// </summary>
/// <param name="context">Database context for the health check.</param>
public class DatabaseHealthCheck(UserServicesRSOContext context) : IHealthCheck
{
    /// <summary>
    /// Database context for the health check.
    /// </summary>
    private readonly UserServicesRSOContext _context = context;

    /// <summary>
    /// Performs a health check for the database.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="cancellationToken">Cancelation token.</param>
    /// <returns>A <see cref="HealthCheckResult"/> based on the fact if the database can be communicated with.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            // connect to database and execute "select 1" query
            await _context.Database.OpenConnectionAsync(cancellationToken);
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}
