using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriTEC.Application.Interfaces.Auth;
using NutriTEC.Application.Interfaces.Clients;
using NutriTEC.Application.Interfaces.Users;
using NutriTEC.Infrastructure.Persistence;
using NutriTEC.Infrastructure.Repositories;
using NutriTEC.Infrastructure.Security;

namespace NutriTEC.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // SQL Server configuration is centralized here so the API only composes the infrastructure layer.
        services.AddDbContext<NutriTecDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
