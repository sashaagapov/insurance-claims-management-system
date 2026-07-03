using ClaimsModule.Application.Interfaces;
using ClaimsModule.Infrastructure.BackgroundJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClaimsModule.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ReserveGlPostingJob>();
        services.AddScoped<IReserveGlPostingJobQueue, ReserveGlPostingJobQueue>();

        return services;
    }
}
