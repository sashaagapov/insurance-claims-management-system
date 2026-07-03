using ClaimsModule.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<ClaimsModuleDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IClaimsModuleDbContext>(provider =>
            provider.GetRequiredService<ClaimsModuleDbContext>());

        return services;
    }
}
