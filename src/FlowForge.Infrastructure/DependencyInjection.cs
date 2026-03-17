using FlowForge.Application.Checkpoints.Contracts;
using FlowForge.Infrastructure.Checkpoints.Json;
using Microsoft.Extensions.DependencyInjection;

namespace FlowForge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISimulationCheckpointStore, JsonSimulationCheckpointStore>();

        return services;
    }
}
