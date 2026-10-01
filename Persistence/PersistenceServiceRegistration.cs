using Application.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<BaseDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("StajSistemi")));

        services.AddScoped<IStajBasvuruRepository, StajBasvuruRepository>();
        services.AddScoped<IMesajRepository, MesajRepository>();
        services.AddScoped<IProjeRepository, ProjeRepository>();
        services.AddScoped<IStajyerProjeRepository, StajyerProjeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        services.AddScoped<IUserOperationClaimRepository, UserOperationClaimRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}