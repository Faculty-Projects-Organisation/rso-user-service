using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RSO.Core.Health;

public class ExternalAPICheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://api.lavbic.net/kraji/1000", cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded(ex.Message);
        }
    }
}
