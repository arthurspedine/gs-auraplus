using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AuraPlus.Web.HealthCheck;

public class ApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Aqui você pode adicionar verificações personalizadas
        // Por exemplo: verificar se serviços externos estão disponíveis
        
        var isHealthy = true;

        if (isHealthy)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("AuraPlus API está funcionando corretamente."));
        }

        return Task.FromResult(
            new HealthCheckResult(
                context.Registration.FailureStatus,
                "AuraPlus API não está saudável."));
    }
}
