using Microsoft.Extensions.DependencyInjection;
using NutriTEC.MongoApplication.Interfaces;
using NutriTEC.MongoApplication.Services;

namespace NutriTEC.MongoApplication.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IMessageService, MessageService>();
        return services;
    }
}
