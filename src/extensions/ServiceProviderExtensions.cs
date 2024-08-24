using System.Reflection;
using LeBonCoinAlert.models.TelegramCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LeBonCoinAlert.extensions;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddTelegramCommands(this IServiceCollection services)
    {
        var commandTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TelegramCommand)) && !t.IsAbstract);

        foreach (var commandType in commandTypes)
        {
            services.AddSingleton(commandType);
        }

        return services;
    }

    public static void ResolveAllTelegramCommands(this IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var commandTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TelegramCommand)) && !t.IsAbstract);

        foreach (var commandType in commandTypes)
        {
            serviceProvider.GetRequiredService(commandType);
            logger.LogInformation($"Instantiated command: {commandType.Name}");
        }
    }
}