using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriTEC.MongoApplication.Interfaces;
using NutriTEC.MongoInfrastructure.Persistence;
using NutriTEC.MongoInfrastructure.Repositories;
using NutriTEC.MongoInfrastructure.Settings;

namespace NutriTEC.MongoInfrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = new MongoDbSettings
        {
            ConnectionString = configuration["MongoDb:ConnectionString"]
                ?? throw new InvalidOperationException("MongoDb:ConnectionString is missing."),
            DatabaseName = configuration["MongoDb:DatabaseName"]
                ?? throw new InvalidOperationException("MongoDb:DatabaseName is missing.")
        };

        services.AddSingleton(settings);
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        return services;
    }
}
