using System.Reflection;
using LeBonCoinAlert.models.TelegramCommands;
using Microsoft.Extensions.DependencyInjection;

namespace LeBonCoinAlert.extensions;

public static class ServiceProviderExtensions
{
    public static void ResolveAllTelegramCommands(this IServiceProvider serviceProvider)
    {
        var commandTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TelegramCommand)) && !t.IsAbstract);

        foreach (var commandType in commandTypes)
        {
            serviceProvider.GetRequiredService(commandType);
        }
    }
}