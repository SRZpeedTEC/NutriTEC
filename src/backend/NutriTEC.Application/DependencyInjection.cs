using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NutriTEC.Application.DTOs.Auth;
using NutriTEC.Application.Interfaces.Auth;
using NutriTEC.Application.Mappings;
using NutriTEC.Application.Services;
using NutriTEC.Application.Validators.Auth;

namespace NutriTEC.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application services expose business workflows and the supporting validation/mapping rules.
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IValidator<RegisterClientRequest>, RegisterClientRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddAutoMapper(_ => { }, typeof(AuthMappingProfile).Assembly);

        return services;
    }
}
